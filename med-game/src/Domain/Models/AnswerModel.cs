using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.Enums;

namespace med_game.src.Domain.Models
{
    
    public class AnswerModel
    {
        public long Id { get; private set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string Type { get; set; } = string.Empty;
        public QuestionModel Question { get; set; }
        

        public AnswerOption ToAnswerOption()
            => new AnswerOption(type: (TypeAnswer)Enum.Parse(typeof(TypeAnswer), Type),
                                text: Description,
                                image: Image);

        public AnswerOption ToAnswerOptionWithWebPath()
            => new AnswerOption(type: (TypeAnswer)Enum.Parse(typeof(TypeAnswer), Type),
                                text: Description,
                                image: @$"{Constants.webPathToAnswerIcons}{Image}"
                );
    }
}