using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Entity
{
    public class Album : BaseEntity
    {
        public string Name { get; set; }
        public string PictureLink { get; set; }
        public DateTime DateCreation { get; set; }

        public int? fkArtist { get; set; }

        [ForeignKey("fkArtist")]
        public virtual Artist Artist { get; set; }

        public virtual ICollection<Track> Tracks { get; set; }

        [NotMapped]
        public Bitmap Picture { get; set; }
    }
}
