using med_game.src.Entities;
using med_game.src.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace med_game.Models
{
    [Index("Name", IsUnique = true)]
    public class Achievement
    {
        [Key]
        public int Id { get; private set; }

        [MaxLength(150)]
        public string Name { get; set; }
        public string Description { get; set; }
        public int CountPoints { get; set; }
        public int MaxCountPoints { get; set; }
        public string? Image { get; set; }

        public List<User> Users { get; set; } = new();

        public AchievementBody ToAchievementBody()
        {
            return new AchievementBody
            {
                Name = Name,
                Description = Description,
                CountPoints = CountPoints,
                MaxCountPoints = MaxCountPoints,
                UrlIcon = Image,
            };
        }
    }
}
