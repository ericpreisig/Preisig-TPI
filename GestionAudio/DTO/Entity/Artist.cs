using System.Collections.Generic;

namespace DTO.Entity
{

    /// <summary>
    /// The database Artist entity
    /// </summary>
    public class Artist : BaseEntity
    {
        #region Public Properties

        public virtual ICollection<Album> Albums { get; set; }
        public string Name { get; set; }

        #endregion Public Properties
    }
}