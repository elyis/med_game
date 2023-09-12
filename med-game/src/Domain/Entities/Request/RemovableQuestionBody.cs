using med_game.src.Domain.Entities.Shared;
using med_game.src.Domain.Enums;

namespace med_game.src.Domain.Entities.Request
{
    public class RemovableQuestionBody
    {
        public string LecternName { get; set; }
        public string ModuleName { get; set; }
        public string? Text { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public TypeQuestion TypeQuestion { get; set; }

        public QuestionProperties ToQuestionProperties(){
            return new QuestionProperties{
                Description = Description,
                Image = Image,
                Text = Text,
                Type = TypeQuestion
            };
        }
    }
}