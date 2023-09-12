using med_game.src.Domain.Entities.Shared;

namespace med_game.src.Domain.Models
{
    public class ModuleModel
    {
        public int Id { get; private set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }


        public LecternModel LecternModel { get; set; }
        public List<QuestionModel> Questions { get; set; } = new();

        public ModuleBody ToModuleBody()
        {
            return new ModuleBody
            {
                ModuleName = Name,
                Description = Description,
            };
        }
    }
}