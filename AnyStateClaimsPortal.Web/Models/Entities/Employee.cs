using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyStateClaimsPortal.Web.Models.Entities
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(20)]
        public string EmployeeNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        public char? MiddleInitial { get; set; }

        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(4)]
        public string SSNLast4 { get; set; }

        [Column(TypeName = "date")]
        public DateTime HireDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? TerminationDate { get; set; }

        [Required]
        [StringLength(100)]
        public string JobTitle { get; set; }

        [StringLength(10)]
        public string JobClassCode { get; set; }

        [StringLength(100)]
        public string Department { get; set; }

        public decimal AnnualSalary { get; set; }

        public decimal? HourlyRate { get; set; }

        [StringLength(20)]
        public string PayFrequency { get; set; }

        public int AgencyId { get; set; }

        [StringLength(100)]
        public string SupervisorName { get; set; }

        [StringLength(200)]
        public string SupervisorEmail { get; set; }

        public int NumberOfDependents { get; set; }

        [StringLength(100)]
        public string EmergencyContactName { get; set; }

        [StringLength(20)]
        public string EmergencyContactPhone { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        public virtual Agency Agency { get; set; }

        public virtual ICollection<Claim> Claims { get; set; }
    }
}
