﻿using System.ComponentModel.DataAnnotations;

namespace med_game.src.Entities.Request
{
    public class AchievementBody
    {
        [MaxLength(150)]
        public string Name { get; set; }
        public string Description { get; set; }
        public int CountPoints { get; set; } = 0;
        public int MaxCountPoints { get; set; }
        public string? urlIcon { get; set; }
    }
}
