using med_game.src.Domain.Enums;
using Newtonsoft.Json;

namespace med_game.src.Domain.Entities.Request
{
    public class RoomSettingBody
    {
        [JsonProperty(PropertyName = "nameDepartment")]
        public string LecternName { get; set; }

        [JsonProperty(PropertyName = "nameModule")]
        public string? ModuleName { get; set; } = null;

        public TypeBattle Type { get; set; }
        public int CountPlayers { get; set; } = 2;
    }
}