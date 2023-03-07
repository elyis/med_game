using System.Text.Json.Serialization;

namespace med_game.src.Entities.Request
{
    public class RoomSetting
    {
        [JsonPropertyName("nameDepartment")]
        public string LecternName { get; set; }

        [JsonPropertyName("nameModule")]
        public string? ModuleName { get; set; } = null;

        public TypeBattle type;
    }

    [JsonConverter(typeof(TypeBattle))]
    public enum TypeBattle
    {
        Rating,
        Simple
    }
}
