using med_game.Models;
using med_game.src.models;
using med_game.src.Models;
using Microsoft.EntityFrameworkCore;

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
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<Friend>(f =>
            {
                f.HasKey(f => new { f.AuthorId, f.SubscriberId });
                f.HasOne(f => f.Author).WithMany(u => u.Friends);
                f.HasOne(f => f.Subscriber).WithMany()
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<FriendRequest>(f =>
            {
                f.HasKey(f => new {f.AuthorId, f.SubscriberId });
                f.HasOne(f => f.Subscriber).WithMany(u => u.Subscribers);
                f.HasOne(f => f.Author).WithMany()
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }
    }
}
