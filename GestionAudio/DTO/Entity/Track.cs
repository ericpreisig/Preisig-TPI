﻿/********************************************************************************
*  Author : Eric-Nicolas Preisig
*  Company : ETML
*
*  File Summary : Track
*********************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTO.Entity
{
    /// <summary>
    /// The database Track entity
    /// </summary>
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
        public int ListenedTimes { get; set; }
        public virtual List<Playlist> Playlists { get; set; }

        #endregion Public Properties
    }
}