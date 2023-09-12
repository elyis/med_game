using med_game.src.Domain.Enums;

namespace med_game.src.Domain.Entities.Game
{
    public class RoomSettings
    {
        public int LecternId { get; set; }
        public int? ModuleId { get; set; }
        public TypeBattle Type { get; set; }
        public int CountPlayers { get; set; }

        public bool Equals(RoomSettings roomSettings)
        {
            if (roomSettings.LecternId == LecternId &&
               roomSettings.ModuleId == ModuleId &&
               roomSettings.Type == Type)
                return true;
            return false;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as RoomSettings);
        }
    }
}