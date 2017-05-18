using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Entity;

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
        private int _file;

        [NotMapped]
        public int File => _file != 0 ? _file : ( _file = this is Track ?Shared.MusicFile.LoadMusic(Path) : Shared.MusicFile.LoadRadio(Path));

    }
}