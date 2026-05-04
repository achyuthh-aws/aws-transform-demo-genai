using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AnyStateClaimsPortal.Web.Models
{
    public class ClaimViewModel
    {
        public int ClaimId { get; set; }
        public string ClaimNumber { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime InjuryDate { get; set; }

        [Required]
        [StringLength(4000, MinimumLength = 20)]
        public string InjuryDescription { get; set; }

        [Required]
        public string InjuryType { get; set; }

        [Required]
        public string BodyPartAffected { get; set; }

        [Required]
        public string IncidentLocation { get; set; }

        public string LocationType { get; set; }
        public string WitnessName { get; set; }
        public string WitnessPhone { get; set; }
        public string WitnessStatement { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public int? AssignedAdjusterId { get; set; }
        public int? MedicalReviewerId { get; set; }
        public decimal? WeeklyBenefitAmount { get; set; }
        public decimal? TotalPaidAmount { get; set; }
        public decimal? TotalMedicalCost { get; set; }
        public decimal? TotalReserveAmount { get; set; }
        public string DenialReason { get; set; }
        public bool IsLitigated { get; set; }
        public DateTime? ReturnToWorkDate { get; set; }
        public string EmployeeName { get; set; }
        public string AgencyName { get; set; }
        public string AdjusterName { get; set; }
        public DateTime CreatedDate { get; set; }

        public SelectList Employees { get; set; }
        public SelectList Adjusters { get; set; }
        public SelectList MedicalReviewers { get; set; }
        public SelectList InjuryTypes { get; set; }
        public SelectList Priorities { get; set; }
        public SelectList LocationTypes { get; set; }

        public List<ClaimNoteViewModel> Notes { get; set; }
        public string NewNoteText { get; set; }
        public string NewNoteType { get; set; }
    }

    public class ClaimNoteViewModel
    {
        public string NoteText { get; set; }
        public string NoteType { get; set; }
        public bool IsConfidential { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class ClaimSearchViewModel
    {
        public string SearchTerm { get; set; }
        public string Status { get; set; }
        public int? AgencyId { get; set; }
        public string InjuryType { get; set; }
        public string Priority { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int? AdjusterId { get; set; }
        public bool? IsLitigated { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public List<ClaimListItemViewModel> Results { get; set; }

        public SelectList Statuses { get; set; }
        public SelectList Agencies { get; set; }
        public SelectList InjuryTypes { get; set; }
        public SelectList Priorities { get; set; }
        public SelectList Adjusters { get; set; }
    }

    public class ClaimListItemViewModel
    {
        public int ClaimId { get; set; }
        public string ClaimNumber { get; set; }
        public DateTime InjuryDate { get; set; }
        public string InjuryType { get; set; }
        public string BodyPartAffected { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public decimal? WeeklyBenefitAmount { get; set; }
        public decimal? TotalPaidAmount { get; set; }
        public decimal? TotalMedicalCost { get; set; }
        public bool IsLitigated { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeNumber { get; set; }
        public string AgencyName { get; set; }
        public string AgencyCode { get; set; }
        public string AdjusterName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
