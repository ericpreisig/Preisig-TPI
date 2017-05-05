using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTO.Entity
{
    public class Track : Audio
    {
        public TimeSpan Duration { get; set; }

        public virtual List<Playlist> Playlists { get; set; }

        public int? fkAlbum { get; set; }

        [ForeignKey("fkAlbum")]
        public virtual Album Album { get; set; }

    }
}
