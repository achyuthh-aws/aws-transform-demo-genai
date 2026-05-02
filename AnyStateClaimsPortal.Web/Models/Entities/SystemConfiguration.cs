using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyStateClaimsPortal.Web.Models.Entities
{
    public class SystemConfiguration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ConfigId { get; set; }

        [StringLength(100)]
        public string ConfigKey { get; set; }

        [StringLength(500)]
        public string ConfigValue { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(20)]
        public string DataType { get; set; }

        [StringLength(50)]
        public string Category { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
