using med_game.Models;
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
            
        }
    }
}
