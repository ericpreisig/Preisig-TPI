using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTO.Entity
{
    /// <summary>
    /// The database Radio entity
    /// </summary>
    public class Radio : Audio
    {
        #region Public Properties

        public string Desrciption { get; set; }
        public string Format { get; set; }
        public string LogoUrl { get; set; }
        public DateTime LastListen { get; set; }

        public string ShoutCastId { get; set; }

        #endregion Public Properties
    }
}