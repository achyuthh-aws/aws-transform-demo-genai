using System;
using System.Collections.Generic;

namespace AnyStateClaimsPortal.Web.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int LockedUsers { get; set; }
        public List<UserListItem> Users { get; set; }
        public List<AuditLogItem> RecentAudit { get; set; }
    }

    public class UserListItem
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string AgencyName { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }

    public class AuditLogItem
    {
        public int AuditId { get; set; }
        public string TableName { get; set; }
        public int RecordId { get; set; }
        public string Action { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangedDate { get; set; }
    }
}
