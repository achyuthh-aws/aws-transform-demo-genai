using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyStateClaimsPortal.Web.Models.Entities
{
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AuditId { get; set; }

        [StringLength(100)]
        public string TableName { get; set; }

        public int RecordId { get; set; }

        [StringLength(10)]
        public string Action { get; set; }

        [StringLength(100)]
        public string FieldName { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        [StringLength(50)]
        public string ChangedBy { get; set; }

        public DateTime ChangedDate { get; set; }

        [StringLength(45)]
        public string IPAddress { get; set; }
    }
}
