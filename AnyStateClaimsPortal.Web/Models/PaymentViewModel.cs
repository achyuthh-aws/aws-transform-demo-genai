using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace AnyStateClaimsPortal.Web.Models
{
    public class PaymentViewModel
    {
        public int PaymentId { get; set; }
        public int ClaimId { get; set; }
        public string ClaimNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }

        [Required]
        public string PaymentType { get; set; }

        [Required]
        [Range(0.01, 999999)]
        public decimal Amount { get; set; }

        public string CheckNumber { get; set; }

        [Required]
        public string PayeeName { get; set; }

        public string PayeeType { get; set; }
        public string Description { get; set; }
        public int FiscalYear { get; set; }
        public string Status { get; set; }

        public SelectList PaymentTypes { get; set; }
        public SelectList PayeeTypes { get; set; }
    }

    public class PaymentListItem
    {
        public int PaymentId { get; set; }
        public int ClaimId { get; set; }
        public string ClaimNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentType { get; set; }
        public decimal Amount { get; set; }
        public string CheckNumber { get; set; }
        public string PayeeName { get; set; }
        public string PayeeType { get; set; }
        public string PaymentStatus { get; set; }
        public int FiscalYear { get; set; }
        public string EmployeeName { get; set; }
        public string AgencyName { get; set; }
    }
}
