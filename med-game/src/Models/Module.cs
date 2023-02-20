using med_game.src.Entities;

namespace med_game.src.Models
{
    public class Module
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public string? Description { get; set; }


        public Lectern Lectern { get; set; }
        public List<Question> Questions { get; set; }

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
