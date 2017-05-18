using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTO.Entity
{
    public class Context : BaseEntity
    {
        public bool IsMusicPlaying { get; set; }

        public long? fkTrack { get; set; }
        public long? fkRadio { get; set; }

        [ForeignKey("fkTrack")]
        public Track Track { get; set; }

        [ForeignKey("fkRadio")]
        public Radio Radio { get; set; }

        [NotMapped]
        public int IsLooping { get; set; }

        [NotMapped]
        public bool IsRandom { get; set; }

        //in ms
        public int ActualTime { get; set; }

        [NotMapped]
        public Audio ActualAudio => Track ?? (Audio)Radio;

    }
}
