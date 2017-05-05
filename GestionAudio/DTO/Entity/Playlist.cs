using System;
using System.Collections.Generic;

namespace DTO.Entity
{
    public class Playlist
    {
        public string Name { get; set; }
        public virtual List<Track> Tracks { get; set; }
    }
}
