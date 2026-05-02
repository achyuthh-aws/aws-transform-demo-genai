using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyStateClaimsPortal.Web.Models.Entities
{
    public class BodyPartCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BodyPartCodeId { get; set; }

        [StringLength(10)]
        public string Code { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        [StringLength(50)]
        public string BodyRegion { get; set; }
    }
}
