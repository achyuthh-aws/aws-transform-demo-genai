using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyStateClaimsPortal.Web.Models.Entities
{
    public class Agency
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AgencyId { get; set; }

        [Required]
        [StringLength(10)]
        public string AgencyCode { get; set; }

        [Required]
        [StringLength(200)]
        public string AgencyName { get; set; }

        [StringLength(200)]
        public string Division { get; set; }

        [StringLength(50)]
        public string AgencyType { get; set; }

        [StringLength(100)]
        public string ContactName { get; set; }

        [StringLength(200)]
        public string ContactEmail { get; set; }

        [StringLength(20)]
        public string ContactPhone { get; set; }

        [StringLength(300)]
        public string Address { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(2)]
        public string State { get; set; }

        [StringLength(10)]
        public string ZipCode { get; set; }

        [StringLength(20)]
        public string RiskCategory { get; set; }

        public decimal? AnnualPremium { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
