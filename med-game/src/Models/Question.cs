using med_game.src.Entities;
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
        public int CountPointsPerAnswer { get; set; } = 3;
        public long CorrectAnswerIndex { get; set; }

        public Module Module { get; set; }
        public List<Answer> Answers { get; set; } = new();

        public QuestionBody ToQuestionBody()
            => new QuestionBody
            {
                Description = Description,
                Text = Text,
                NumOfPointsPerAnswer = CountPointsPerAnswer,
                Image = Image,
                TimeSeconds = TimeSeconds,

                Type = (TypeQuestion)Enum.Parse(typeof(TypeQuestion), Type),
                RightAnswer = Answers[(int)CorrectAnswerIndex].ToAnswerOption(),
                Answers = Answers.Select(a => a.ToAnswerOption()).ToList(),
            };
    }
}
