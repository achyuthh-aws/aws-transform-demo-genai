using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyStateClaimsPortal.Web.Models.Entities
{
    public class InjuryCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InjuryCodeId { get; set; }

        [StringLength(10)]
        public string Code { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [StringLength(50)]
        public string Category { get; set; }

        [StringLength(20)]
        public string Severity { get; set; }

        public int? TypicalDuration { get; set; }

        public bool IsActive { get; set; }
    }
}
