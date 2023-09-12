using med_game.src.Domain.Entities.Response;
using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.Enums;

namespace med_game.src.Domain.Models
{
    public class QuestionModel
    {
        public long Id { get; private set; }
        public string? Text { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string Type { get; set; } = string.Empty;
        public int TimeSeconds { get; set; }
        public int CountPointsPerAnswer { get; set; } = 3;
        public long CorrectAnswerIndex { get; set; }

        public ModuleModel Module { get; set; }
        public List<AnswerModel> Answers { get; set; } = new();

        public QuestionBody ToQuestionBody()
            => new QuestionBody
            {
                description = Description,
                text = Text,
                numOfPointsPerAnswer = CountPointsPerAnswer,
                image = Image == null ? "" : @$"{Constants.webPathToQuestionIcons}{Image}",
                timeSeconds = TimeSeconds,

                type = (TypeQuestion)Enum.Parse(typeof(TypeQuestion), Type),
                RightAnswer = Answers[(int)CorrectAnswerIndex].ToAnswerOption(),
                Answers = Answers.Select(a => a.ToAnswerOptionWithWebPath()).ToList(),
            };

        public GameQuestion ToGameQuestion()
           => new GameQuestion
           {
               description = Description,
               text = Text,
               image = Image == null ? "" : @$"{Constants.webPathToQuestionIcons}{Image}",
               timeSeconds = TimeSeconds,

               type = (TypeQuestion)Enum.Parse(typeof(TypeQuestion), Type),
               RightAnswer = Answers[(int)CorrectAnswerIndex].ToAnswerOptionWithWebPath(),
               Answers = Answers.Select(a => a.ToAnswerOptionWithWebPath()).ToList(),
           };
    }
}