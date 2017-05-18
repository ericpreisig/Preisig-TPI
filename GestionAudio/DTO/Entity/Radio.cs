using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTO.Entity
{
    public class Radio : Audio
    {
        public string ShoutCastId { get; set; }
        public string Desrciption { get; set; }
    }
}
