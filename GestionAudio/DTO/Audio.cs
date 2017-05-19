using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using DTO.Entity;
using NAudio.Wave;
using Shared;

namespace DTO
{
    public class Audio : BaseEntity

    {
        public string Name { get; set; }

        public long? fkGenre { get; set; }

        [ForeignKey("fkGenre")]
        public Genre Genre { get; set; }

        public bool IsFavorite { get; set; }

        public string Path { get; set; }

        [NotMapped]
        public BitmapImage Picture => this is Radio ? new BitmapImage(new Uri((this as Radio).LogoUrl)) : (this as Track).Album.Picture;


        [NotMapped]
        private WaveStream _file;
        

        [NotMapped]
        public WaveStream File
        {
            get { return _file ?? (_file = this is Track ? MusicFile.LoadMusic(Path) : MusicFile.LoadRadio(Path, (this as Radio).Format)); }
            set
            {
                if(value==null)
                    MusicFile.StopRadio();
                _file=value;
            }
        }
    }
}