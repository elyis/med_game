using med_game.src.Entities;
using med_game.src.Entities.Response;
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
                description = Description,
                text = Text,
                numOfPointsPerAnswer = CountPointsPerAnswer,
                image = Image == null ? "" : @$"{Constants.webPathToQuestionIcons}{Image}",
                timeSeconds = TimeSeconds,

                type = (TypeQuestion)Enum.Parse(typeof(TypeQuestion), Type),
                rightAnswer = Answers[(int)CorrectAnswerIndex].ToAnswerOption(),
                answers = Answers.Select(a => a.ToAnswerOption()).ToList(),
            };

        public GameQuestion ToGameQuestion()
           => new GameQuestion
           {
               description = Description,
               text = Text,
               image = Image == null ? "" : @$"{Constants.webPathToQuestionIcons}{Image}",
               timeSeconds = TimeSeconds,

               type = (TypeQuestion)Enum.Parse(typeof(TypeQuestion), Type),
               rightAnswer = Answers[(int)CorrectAnswerIndex].ToAnswerOption(),
               answers = Answers.Select(a => a.ToAnswerOption()).ToList(),
           };
    }
}
