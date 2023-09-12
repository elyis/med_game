using med_game.src.Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore;

namespace med_game.src.Domain.Models
{
    [Index("Name", IsUnique = true)]
    public class LecternModel
    {
        public int Id { get; private set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public List<ModuleModel> Modules { get; set; } = new();

        public LecternBody ToLecternBody()
        {
            return new LecternBody
            {
                Name = Name,
                Description = Description
            };
        }
    }
}