using DTO.Entity;
using NAudio.Wave;
using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows.Media.Imaging;

namespace DTO
{
    /// <summary>
    /// The link between radio and track
    /// </summary>
    public class Audio : BaseEntity
    {
        #region Private Fields

        [NotMapped]
        private WaveStream _file;

        #endregion Private Fields

        #region Public Properties

        [NotMapped]
        public WaveStream File
        {
            get { return _file ?? (_file = this is Track ? MusicFile.LoadMusic(Path) : MusicFile.LoadRadio(Path, (this as Radio).Format)); }
            set
            {
                if (value == null)
                    MusicFile.StopRadio();
                _file = value;
            }
        }

        public virtual List<Genre> Genres { get; set; }

        public bool IsFavorite { get; set; }
        public string Name { get; set; }

        [Index(IsUnique = true)]
        public string Path { get; set; }

        [NotMapped]
        public BitmapImage Picture => this is Radio ? new BitmapImage(new Uri((this as Radio).LogoUrl)) : (this as Track).Album.Picture;

        [NotMapped]
        public string GenresString => string.Join(", ", Genres.Select(a=>a.Name));

        #endregion Public Properties
    }
}