using Microsoft.EntityFrameworkCore;

namespace med_game.src.Models
{
    [Index("Type")]
    public class Answer
    {
        public long Id { get; private set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string Type { get; set; }


        public List<Question> Questions { get; set; } = new();
    }
}
