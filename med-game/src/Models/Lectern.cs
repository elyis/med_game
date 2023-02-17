﻿using Microsoft.EntityFrameworkCore;

namespace med_game.src.Models
{
    [Index("Name", IsUnique = true)]
    public class Lectern
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public List<Module> Modules { get; set; } = new();
    }
}
