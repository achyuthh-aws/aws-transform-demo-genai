using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyStateClaimsPortal.Web.Models.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [StringLength(50)]
        public string Username { get; set; }

        [StringLength(256)]
        public string PasswordHash { get; set; }

        [StringLength(64)]
        public string Salt { get; set; }

        [StringLength(100)]
        public string FullName { get; set; }

        [StringLength(200)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(30)]
        public string Role { get; set; }

        public int? AgencyId { get; set; }

        public bool IsActive { get; set; }

        public bool IsLocked { get; set; }

        public int FailedLoginAttempts { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? PasswordChangedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public virtual Agency Agency { get; set; }
    }
}
