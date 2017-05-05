using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Entity;
using NAudio.Wave;

namespace DTO
{
    public class Audio
    {
        public string Name { get; set; }

        [NotMapped]
        private string _path;

        public int? fkGenre { get; set; }

        [ForeignKey("fkGenre")]
        public Genre Genre { get; set; }

        public bool IsFavortie { get; set; }

        public string Path
        {
            get { return _path; }
            set
            {
                File = Shared.MusicFile.LoadMusic(value);
                _path = value;
            }
        }

        [NotMapped]
        public WaveOut File { get; set; }

        [NotMapped]
        public TimeSpan ActualTime { get; set; }
    }
}
