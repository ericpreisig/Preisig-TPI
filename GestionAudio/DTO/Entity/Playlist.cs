using System.Collections.Generic;

namespace DTO.Entity
{
    public class Playlist : BaseEntity
    {
        #region Public Properties

        public string Name { get; set; }
        public virtual List<Track> Tracks { get; set; } = new List<Track>();

        #endregion Public Properties
    }
}