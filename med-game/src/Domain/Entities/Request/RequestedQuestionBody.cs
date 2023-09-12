using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.Enums;

namespace med_game.src.Domain.Entities.Request
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
        public List<AnswerOption> ListOfAnswer { get; set; }

        public QuestionBody ToQuestionBody()
            => new QuestionBody 
            { 
                text= Text, 
                description = Description,
                image = Image, 
                type = TypeQuestion, 
                timeSeconds = TimeSeconds,
                RightAnswer = RightAnswer, 
                numOfPointsPerAnswer = NumOfPointsPerAnswer, 
                Answers = ListOfAnswer 
            };

        public QuestionProperties ToQuestionProperties()
        {
            return new QuestionProperties
            {
                Description = Description,
                Image = Image,
                Text = Text,
                Type = TypeQuestion
            };
        }
    }
}