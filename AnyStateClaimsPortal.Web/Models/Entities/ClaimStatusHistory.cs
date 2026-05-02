using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyStateClaimsPortal.Web.Models.Entities
{
    public class ClaimStatusHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HistoryId { get; set; }

        public int ClaimId { get; set; }

        [StringLength(20)]
        public string OldStatus { get; set; }

        [StringLength(20)]
        public string NewStatus { get; set; }

        [StringLength(50)]
        public string ChangedBy { get; set; }

        public DateTime ChangedDate { get; set; }

        [StringLength(500)]
        public string Remarks { get; set; }

        [StringLength(45)]
        public string IPAddress { get; set; }

        public virtual Claim Claim { get; set; }
    }
}
