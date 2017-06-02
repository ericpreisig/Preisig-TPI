/********************************************************************************
*  Author : Eric-Nicolas Preisig
*  Company : ETML
*
*  File Summary : Playlist
*********************************************************************************/

using System.Collections.Generic;

namespace DTO.Entity
{
    /// <summary>
    /// The database Playlist entity
    /// </summary>
    public class Playlist : BaseEntity
    {
        #region Public Properties

        public string Name { get; set; }
        public virtual List<Track> Tracks { get; set; } = new List<Track>();

        #endregion Public Properties
    }
}