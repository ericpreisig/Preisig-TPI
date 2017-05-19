using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTO.Entity
{
    public class Track : Audio
    {
        #region Public Properties

        [ForeignKey("fkAlbum")]
        public virtual Album Album { get; set; }

        //in ms
        public int Duration { get; set; }

        [NotMapped]
        public TimeSpan DurationTime => TimeSpan.FromMilliseconds(Duration);

        public long? fkAlbum { get; set; }
        public virtual List<Playlist> Playlists { get; set; }

        #endregion Public Properties
    }
}