using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyStateClaimsPortal.Web.Models.Entities
{
    public class MedicalTreatment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TreatmentId { get; set; }

        public int ClaimId { get; set; }

        public int ProviderId { get; set; }

        [Column(TypeName = "date")]
        public DateTime TreatmentDate { get; set; }

        [StringLength(50)]
        public string TreatmentType { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        [StringLength(500)]
        public string Diagnosis { get; set; }

        [StringLength(10)]
        public string ICDCode { get; set; }

        [StringLength(10)]
        public string CPTCode { get; set; }

        public decimal? BilledAmount { get; set; }

        public decimal? ApprovedAmount { get; set; }

        [StringLength(20)]
        public string Status { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NextAppointment { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual Claim Claim { get; set; }

        public virtual MedicalProvider Provider { get; set; }
    }
}
