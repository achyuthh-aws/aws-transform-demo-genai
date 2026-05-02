using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyStateClaimsPortal.Web.Models.Entities
{
    public class ClaimPayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }

        public int ClaimId { get; set; }

        [Column(TypeName = "date")]
        public DateTime PaymentDate { get; set; }

        [StringLength(30)]
        public string PaymentType { get; set; }

        public decimal Amount { get; set; }

        [StringLength(20)]
        public string CheckNumber { get; set; }

        [StringLength(100)]
        public string PayeeName { get; set; }

        [StringLength(30)]
        public string PayeeType { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(20)]
        public string VoucherNumber { get; set; }

        public int FiscalYear { get; set; }

        [StringLength(50)]
        public string ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual Claim Claim { get; set; }
    }
}
