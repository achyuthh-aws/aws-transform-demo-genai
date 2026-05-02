using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyStateClaimsPortal.Web.Models.Entities
{
    public class ClaimDocument
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentId { get; set; }

        public int ClaimId { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(50)]
        public string FileType { get; set; }

        public long FileSizeBytes { get; set; }

        [StringLength(50)]
        public string DocumentType { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [StringLength(500)]
        public string StoragePath { get; set; }

        [StringLength(50)]
        public string UploadedBy { get; set; }

        public DateTime UploadedDate { get; set; }

        public virtual Claim Claim { get; set; }
    }
}
