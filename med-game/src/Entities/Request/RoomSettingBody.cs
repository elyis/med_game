using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace med_game.src.Entities.Request
{
    public class RoomSettingBody
    {
        [JsonProperty(PropertyName = "nameDepartment")]
        public string LecternName { get; set; }

        [JsonProperty(PropertyName = "nameModule")]
        public string? ModuleName { get; set; } = null;

        public TypeBattle Type { get; set; }
        public int CountPlayers = 2;
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TypeBattle
    {
        Rating,
        Simple
    }
}
