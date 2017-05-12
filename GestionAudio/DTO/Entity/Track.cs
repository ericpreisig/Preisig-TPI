using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTO.Entity
{
    public class Track : Audio
    {
        //in ms
        public int Duration { get; set; }

        public virtual List<Playlist> Playlists { get; set; }

        public long? fkAlbum { get; set; }

        [ForeignKey("fkAlbum")]
        public virtual Album Album { get; set; }

        [NotMapped]
        public TimeSpan DurationTime => TimeSpan.FromMilliseconds(Duration);        
    }
}
