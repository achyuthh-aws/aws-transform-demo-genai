using System.Collections.Generic;
using System.Web.Mvc;

namespace AnyStateClaimsPortal.Web.Models
{
    public class ReportViewModel
    {
        public List<AgencyReportItem> AgencyReports { get; set; }
        public List<FinancialSummaryItem> FinancialSummaries { get; set; }
        public List<AgingBucketItem> AgingBuckets { get; set; }
        public int FiscalYear { get; set; }
        public int? SelectedAgencyId { get; set; }
        public SelectList Agencies { get; set; }
        public SelectList FiscalYears { get; set; }
    }

    public class AgencyReportItem
    {
        public int AgencyId { get; set; }
        public string AgencyCode { get; set; }
        public string AgencyName { get; set; }
        public string AgencyType { get; set; }
        public string RiskCategory { get; set; }
        public decimal AnnualPremium { get; set; }
        public int TotalClaims { get; set; }
        public int SubmittedCount { get; set; }
        public int UnderReviewCount { get; set; }
        public int MedicalReviewCount { get; set; }
        public int ApprovedCount { get; set; }
        public int DeniedCount { get; set; }
        public int ClosedCount { get; set; }
        public int LitigatedCount { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal TotalMedicalCost { get; set; }
        public decimal TotalIncurredCost { get; set; }
        public decimal AvgWeeklyBenefit { get; set; }
        public decimal LossRatio { get; set; }
    }

    public class FinancialSummaryItem
    {
        public string PaymentType { get; set; }
        public int PaymentCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AvgAmount { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
    }

    public class MonthlyPaymentItem
    {
        public int Month { get; set; }
        public decimal BenefitPayments { get; set; }
        public decimal MedicalPayments { get; set; }
        public decimal OtherPayments { get; set; }
        public decimal TotalPayments { get; set; }
    }

    public class AgencyCostItem
    {
        public string AgencyCode { get; set; }
        public string AgencyName { get; set; }
        public string RiskCategory { get; set; }
        public int ClaimCount { get; set; }
        public decimal TotalPayments { get; set; }
        public decimal AnnualPremium { get; set; }
        public decimal LossRatio { get; set; }
    }

    public class AgingBucketItem
    {
        public string AgingBucket { get; set; }
        public int ClaimCount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalReserves { get; set; }
    }
}
