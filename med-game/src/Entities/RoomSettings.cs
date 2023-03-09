﻿using med_game.src.Entities.Request;

namespace med_game.src.Entities
{
    public class RoomSettings
    {
        public int LecternId { get; set; }
        public int? ModuleId { get; set; }
        public TypeBattle Type { get; set; }

        public bool Equals(RoomSettings roomSettings)
        {
            if(roomSettings.LecternId == LecternId && 
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
