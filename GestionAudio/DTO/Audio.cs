using DTO.Entity;
using NAudio.Wave;
using Shared;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media.Imaging;

namespace DTO
{
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

        public long? fkGenre { get; set; }

        [ForeignKey("fkGenre")]
        public Genre Genre { get; set; }

        public bool IsFavorite { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        [NotMapped]
        public BitmapImage Picture => this is Radio ? new BitmapImage(new Uri((this as Radio).LogoUrl)) : (this as Track).Album.Picture;

        #endregion Public Properties
    }
}