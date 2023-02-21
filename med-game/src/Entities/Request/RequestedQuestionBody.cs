
namespace med_game.src.Entities.Request
{
    public class RequestedQuestionBody
    {
        public string LecternName { get; set; }
        public string ModuleName { get; set; }
        public TypeQuestion TypeQuestion { get; set;}
        public int TimeSeconds { get; set; }
        public string? Text { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public int NumOfPointsPerAnswer { get; set; } = 3;

        public AnswerOption RightAnswer { get; set; }
        public List<AnswerOption> ListOfAnswers { get; set; }

        public QuestionBody ToQuestionBody()
            => new QuestionBody 
            { 
                Text= Text, 
                Description = Description,
                Image = Image, 
                Type = TypeQuestion, 
                TimeSeconds = TimeSeconds,
                RightAnswer = RightAnswer, 
                NumOfPointsPerAnswer = NumOfPointsPerAnswer, 
                Answers = ListOfAnswers 
            };
    }
}
