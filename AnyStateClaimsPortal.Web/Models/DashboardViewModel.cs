using System;
using System.Collections.Generic;

namespace AnyStateClaimsPortal.Web.Models
{
    public class DashboardViewModel
    {
        public List<StatusSummary> StatusSummaries { get; set; }
        public List<RecentClaimViewModel> RecentClaims { get; set; }
        public List<AgencySummary> AgencySummaries { get; set; }
        public List<MonthlyTrendItem> MonthlyTrends { get; set; }
        public List<InjuryTypeItem> InjuryTypes { get; set; }
        public int TotalClaims { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalReserves { get; set; }
    }

    public class StatusSummary
    {
        public string Status { get; set; }
        public int ClaimCount { get; set; }
        public decimal TotalWeeklyBenefits { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalReserves { get; set; }
    }

    public class RecentClaimViewModel
    {
        public int ClaimId { get; set; }
        public string ClaimNumber { get; set; }
        public DateTime InjuryDate { get; set; }
        public string InjuryType { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string EmployeeName { get; set; }
        public string AgencyName { get; set; }
        public string AgencyCode { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class AgencySummary
    {
        public string AgencyName { get; set; }
        public string AgencyCode { get; set; }
        public string RiskCategory { get; set; }
        public int ClaimCount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalMedical { get; set; }
    }

    public class MonthlyTrendItem
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int ClaimCount { get; set; }
        public decimal TotalPaid { get; set; }
    }

    public class InjuryTypeItem
    {
        public string InjuryType { get; set; }
        public string BodyPart { get; set; }
        public int Occurrences { get; set; }
        public decimal AvgBenefit { get; set; }
    }
}
