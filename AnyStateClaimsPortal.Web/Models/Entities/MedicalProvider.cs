using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyStateClaimsPortal.Web.Models.Entities
{
    public class MedicalProvider
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProviderId { get; set; }

        [StringLength(200)]
        public string ProviderName { get; set; }

        [StringLength(50)]
        public string ProviderType { get; set; }

        [StringLength(10)]
        public string NPI { get; set; }

        [StringLength(10)]
        public string TaxId { get; set; }

        [StringLength(300)]
        public string Address { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(2)]
        public string State { get; set; }

        [StringLength(10)]
        public string ZipCode { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(20)]
        public string Fax { get; set; }

        public bool IsPreferred { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
