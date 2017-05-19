using System.ComponentModel.DataAnnotations.Schema;

namespace DTO.Entity
{
    public class BaseEntity
    {
        #region Public Properties

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        #endregion Public Properties
    }
}