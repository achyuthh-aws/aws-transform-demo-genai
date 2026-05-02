using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyStateClaimsPortal.Web.Models.Entities
{
    public class ClaimNote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NoteId { get; set; }

        public int ClaimId { get; set; }

        [StringLength(4000)]
        public string NoteText { get; set; }

        [StringLength(30)]
        public string NoteType { get; set; }

        public bool IsConfidential { get; set; }

        [StringLength(50)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual Claim Claim { get; set; }
    }
}
