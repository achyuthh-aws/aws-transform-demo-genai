using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AnyStateClaimsPortal.Web.Models;
using AnyStateClaimsPortal.Web.Models.Entities;

namespace AnyStateClaimsPortal.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            using (var db = new AnyStateClaimsContext())
            {
                var users = db.Users.Include(u => u.Agency).ToList();
                var auditLogs = db.AuditLogs.OrderByDescending(a => a.ChangedDate).Take(50).ToList();

                var model = new AdminDashboardViewModel
                {
                    TotalUsers = users.Count,
                    ActiveUsers = users.Count(u => u.IsActive),
                    LockedUsers = users.Count(u => u.IsLocked),
                    Users = users.Select(u => new UserListItem
                    {
                        UserId = u.UserId,
                        Username = u.Username,
                        FullName = u.FullName,
                        Email = u.Email,
                        Role = u.Role,
                        AgencyName = u.Agency != null ? u.Agency.AgencyName : null,
                        IsActive = u.IsActive,
                        IsLocked = u.IsLocked,
                        LastLoginDate = u.LastLoginDate
                    }).ToList(),
                    RecentAudit = auditLogs.Select(a => new AuditLogItem
                    {
                        AuditId = (int)a.AuditId,
                        TableName = a.TableName,
                        RecordId = a.RecordId,
                        Action = a.Action,
                        FieldName = a.FieldName,
                        OldValue = a.OldValue,
                        NewValue = a.NewValue,
                        ChangedBy = a.ChangedBy,
                        ChangedDate = a.ChangedDate
                    }).ToList()
                };

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleUserLock(int userId)
        {
            using (var db = new AnyStateClaimsContext())
            {
                var user = db.Users.Find(userId);
                if (user != null)
                {
                    user.IsLocked = !user.IsLocked;
                    if (!user.IsLocked) user.FailedLoginAttempts = 0;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult SystemConfig()
        {
            using (var db = new AnyStateClaimsContext())
            {
                var configs = db.SystemConfigurations.OrderBy(c => c.Category).ThenBy(c => c.ConfigKey).ToList();
                return View(configs);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateConfig(int configId, string configValue)
        {
            using (var db = new AnyStateClaimsContext())
            {
                var config = db.SystemConfigurations.Find(configId);
                if (config != null)
                {
                    config.ConfigValue = configValue;
                    config.ModifiedBy = User.Identity.Name;
                    config.ModifiedDate = System.DateTime.Now;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("SystemConfig");
        }
    }
}
