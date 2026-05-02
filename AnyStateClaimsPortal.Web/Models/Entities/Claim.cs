using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyStateClaimsPortal.Web.Models.Entities
{
    public class Claim
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClaimId { get; set; }

        [Required]
        [StringLength(20)]
        public string ClaimNumber { get; set; }

        public int EmployeeId { get; set; }

        [Column(TypeName = "date")]
        public DateTime InjuryDate { get; set; }

        public TimeSpan? InjuryTime { get; set; }

        [Column(TypeName = "date")]
        public DateTime ReportedDate { get; set; }

        [Required]
        [StringLength(4000)]
        public string InjuryDescription { get; set; }

        public int? InjuryCodeId { get; set; }

        public int? BodyPartCodeId { get; set; }

        [Required]
        [StringLength(50)]
        public string InjuryType { get; set; }

        [Required]
        [StringLength(100)]
        public string BodyPartAffected { get; set; }

        [Required]
        [StringLength(200)]
        public string IncidentLocation { get; set; }

        [StringLength(50)]
        public string LocationType { get; set; }

        [StringLength(100)]
        public string WitnessName { get; set; }

        [StringLength(20)]
        public string WitnessPhone { get; set; }

        [StringLength(2000)]
        public string WitnessStatement { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EmployerNotifiedDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        [Required]
        [StringLength(10)]
        public string Priority { get; set; }

        public int? AssignedAdjusterId { get; set; }

        public int? MedicalReviewerId { get; set; }

        public decimal? WeeklyBenefitAmount { get; set; }

        public decimal TotalPaidAmount { get; set; }

        public decimal TotalMedicalCost { get; set; }

        public decimal TotalReserveAmount { get; set; }

        [StringLength(500)]
        public string DenialReason { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ReturnToWorkDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? MaxMedicalImprovementDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ClosedDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ReopenedDate { get; set; }

        public bool IsLitigated { get; set; }

        [StringLength(100)]
        public string AttorneyName { get; set; }

        [StringLength(20)]
        public string AttorneyPhone { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [StringLength(50)]
        public string ModifiedBy { get; set; }

        public virtual Employee Employee { get; set; }

        [ForeignKey("AssignedAdjusterId")]
        public virtual User AssignedAdjuster { get; set; }

        [ForeignKey("MedicalReviewerId")]
        public virtual User MedicalReviewer { get; set; }

        [ForeignKey("InjuryCodeId")]
        public virtual InjuryCode InjuryCodeNav { get; set; }

        [ForeignKey("BodyPartCodeId")]
        public virtual BodyPartCode BodyPartCodeNav { get; set; }

        public virtual ICollection<ClaimNote> ClaimNotes { get; set; }

        public virtual ICollection<ClaimStatusHistory> ClaimStatusHistories { get; set; }

        public virtual ICollection<ClaimDocument> ClaimDocuments { get; set; }

        public virtual ICollection<ClaimPayment> ClaimPayments { get; set; }

        public virtual ICollection<MedicalTreatment> MedicalTreatments { get; set; }
    }
}
