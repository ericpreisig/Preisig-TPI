using System.ComponentModel.DataAnnotations.Schema;

namespace DTO.Entity
{
    /// <summary>
    /// The base entity, shared by all entities
    /// </summary>
    public class BaseEntity
    {
        #region Public Properties

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        #endregion Public Properties
    }
}