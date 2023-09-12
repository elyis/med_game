using med_game.src.Domain.Entities.Game;
using med_game.src.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace med_game.src.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
            
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<ModuleModel> Modules { get; set; }
        public DbSet<LecternModel> Lecterns { get; set; }
        public DbSet<QuestionModel> Questions { get; set; }
        public DbSet<AnswerModel> Answers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var connectionString = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FriendRelationModel>(f =>
            {
                f.HasKey(f => new { f.UserId, f.FriendId });

                f.HasOne(f => f.User)
                    .WithMany(u => u.FriendsAcceptedByMe)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                f.HasOne(f => f.Friend)
                    .WithMany(u => u.FriendsAcceptedMe)
                    .HasForeignKey(f => f.FriendId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SubscriberRelationModel>(f =>
            {
                f.HasKey(f => new { f.UserId, f.SubscriberId });

                f.HasOne(f => f.Subscriber)
                    .WithMany(u => u.Subscriptions)
                    .HasForeignKey(f => f.SubscriberId)
                    .OnDelete(DeleteBehavior.Restrict);

                f.HasOne(f => f.User)
                    .WithMany(u => u.Subscribers)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }

    public static class GlobalVariables
    {
        public static readonly ConcurrentDictionary<string, GamingLobby> GamingLobbies = new();
        public static readonly ConcurrentDictionary<long, WebSocket> EnemyWebSocketSessions = new();
    }
}
