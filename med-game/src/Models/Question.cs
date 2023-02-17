using Microsoft.EntityFrameworkCore;

namespace med_game.src.Models
{
    [Index("Type")]
    public class Question
    {
        public long Id { get; private set; }
        public string? Text { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string Type { get; set; }
        public int TimeSeconds { get; set; }
        public int CountPointsPerAnswer { get; set; }
        public long CorrectAnswerIndex { get; set; }

        public Module Module { get; set; }
        public List<Answer> Answers { get; set; } = new();
    }
}
