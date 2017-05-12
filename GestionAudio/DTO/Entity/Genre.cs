using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTO.Entity
{
    public class Genre : BaseEntity
    {
        public string Name { get; set; }
        public int ListenedTimes { get; set; }
        public virtual ICollection<Track> Tracks { get; set; }
    }
}
