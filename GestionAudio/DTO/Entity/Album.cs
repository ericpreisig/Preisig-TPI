using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DTO.Entity
{
    /// <summary>
    /// The database Album entity
    /// </summary>
    public class Album : BaseEntity
    {
        #region Public Properties

        [ForeignKey("fkArtist")]
        public virtual Artist Artist { get; set; }

        public DateTime? DateCreation { get; set; }
        public long? fkArtist { get; set; }
        public string Name { get; set; }

        [NotMapped]
        public BitmapImage Picture => MusicFile.GetImage(Tracks.FirstOrDefault().Path, PictureLink);

        public string PictureLink { get; set; }
        public virtual ICollection<Track> Tracks { get; set; }

        #endregion Public Properties
    }
}