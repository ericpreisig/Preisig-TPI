using System.ComponentModel.DataAnnotations.Schema;

namespace DTO.Entity
{
    public class Context : BaseEntity
    {
        #region Public Properties

        [NotMapped]
        public Audio ActualAudio => Track ?? (Audio)Radio;

        //in ms
        public int ActualTime { get; set; }

        public long? fkRadio { get; set; }
        public long? fkTrack { get; set; }

        public bool IsMusicPlaying { get; set; }

        public bool IsMusicPlayingOnStart { get; set; }

        [NotMapped]
        public int IsLooping { get; set; }

        [NotMapped]
        public bool IsRandom { get; set; }

        [ForeignKey("fkRadio")]
        public virtual Radio Radio { get; set; }

        [ForeignKey("fkTrack")]
        public virtual Track Track { get; set; }

        #endregion Public Properties
    }
}