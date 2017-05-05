using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Entity
{
    public class Artist : BaseEntity
    {
        public string Name { get; set; }
        public virtual ICollection<Album> Albums { get; set; }
    }
}
