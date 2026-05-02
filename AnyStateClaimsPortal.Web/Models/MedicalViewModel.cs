using System;
using System.Collections.Generic;

namespace AnyStateClaimsPortal.Web.Models
{
    public class TreatmentViewModel
    {
        public int TreatmentId { get; set; }
        public DateTime TreatmentDate { get; set; }
        public string TreatmentType { get; set; }
        public string Description { get; set; }
        public string Diagnosis { get; set; }
        public string ICDCode { get; set; }
        public string CPTCode { get; set; }
        public decimal BilledAmount { get; set; }
        public decimal ApprovedAmount { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public DateTime? NextAppointment { get; set; }
        public string ProviderName { get; set; }
        public string ProviderType { get; set; }
        public string ProviderPhone { get; set; }
    }

    public class MedicalSummaryViewModel
    {
        public int ClaimId { get; set; }
        public string ClaimNumber { get; set; }
        public List<TreatmentViewModel> Treatments { get; set; }
        public decimal TotalBilled { get; set; }
        public decimal TotalApproved { get; set; }
    }
}
