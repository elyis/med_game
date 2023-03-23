using med_game.Models;
using med_game.src.Entities.Game;
using med_game.src.models;
using med_game.src.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace med_game.src.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Lectern> Lecterns { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Question> Questions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var connectionString = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<Friends>(f =>
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

            modelBuilder.Entity<FriendRequest>(f =>
            {
                f.HasKey(f => new {f.UserId, f.SubscriberId });

                f.HasOne(f => f.Subscriber)
                    .WithMany(u => u.FriendRequestFromMe)
                    .HasForeignKey(f => f.SubscriberId)
                    .OnDelete(DeleteBehavior.Restrict);

                f.HasOne(f => f.User)
                    .WithMany(u => u.FriendRequestToMe)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }

    public static class GlobalVariables
    {
        public static readonly ConcurrentDictionary<string, GamingLobby> GamingLobbies = new();
    }
}
