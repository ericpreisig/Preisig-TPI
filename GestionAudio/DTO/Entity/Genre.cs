using System.Collections.Generic;

namespace DTO.Entity
{
    /// <summary>
    /// The database Genre entity
    /// </summary>
    public class Genre : BaseEntity
    {
        #region Public Properties

        public int ListenedTimes { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Track> Tracks { get; set; } = new List<Track>();

        #endregion Public Properties
    }
}