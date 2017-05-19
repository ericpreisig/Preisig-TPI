using System;
using System.ComponentModel.DataAnnotations.Schema;
using NAudio.Wave;

namespace DTO.Entity
{
    public class Radio : Audio
    {
        public string ShoutCastId { get; set; }
        public string Desrciption { get; set; }
        public string LogoUrl { get; set; }
        public string Format { get; set; }

        [NotMapped]
        public string RadioPlayingTrack { get; set; }

    }
}
