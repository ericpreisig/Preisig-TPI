using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTO.Entity
{
    public class Context : BaseEntity
    {
        public bool IsMusicPlaying { get; set; }

        public int? fkTrack { get; set; }
        public int? fkRadio { get; set; }

        [ForeignKey("fkTrack")]
        public Track Track { get; set; }

        [ForeignKey("fkRadio")]
        public Radio Radio { get; set; }

        public TimeSpan ActualTime { get; set; }


    }
}
