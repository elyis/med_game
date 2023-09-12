using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace med_game.src.Domain.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Department
    {
        Anatomy,
    }
}