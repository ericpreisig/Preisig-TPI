using System.Collections.Generic;

namespace DTO.Entity
{
    public class Genre : BaseEntity
    {
        #region Public Properties

        public int ListenedTimes { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Track> Tracks { get; set; }

        #endregion Public Properties
    }
}