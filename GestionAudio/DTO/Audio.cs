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
    public class Audio : BaseEntity
    {
        public string Name { get; set; }

        [NotMapped]
        private string _path;

        public long? fkGenre { get; set; }

        [ForeignKey("fkGenre")]
        public Genre Genre { get; set; }

        public bool IsFavorite { get; set; }

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
        public AudioFileReader File { get; set; }
    }
}
