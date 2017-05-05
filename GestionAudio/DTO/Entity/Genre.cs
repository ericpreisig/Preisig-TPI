using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTO.Entity
{
    public class Genre
    {
        public string Name { get; set; }
        public int ListenedTime { get; set; }
        public virtual ICollection<Track> Tracks { get; set; }
    }
}
