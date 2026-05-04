using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AnyStateClaimsPortal.Web.Models;
using AnyStateClaimsPortal.Web.Models.Entities;

namespace AnyStateClaimsPortal.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private bool IsAdmin()
        {
            return Session["UserRole"] != null && Session["UserRole"].ToString() == "Administrator";
        }

        public ActionResult Index()
        {
            try
            {
                if (!IsAdmin()) { return RedirectToAction("Index", "Home"); }
                using (var db = new AnyStateClaimsContext())
                {
                    var users = db.Users.Include("Agency").ToList();
                    var auditLogs = db.AuditLogs.OrderByDescending(a => a.ChangedDate).Take(50).ToList();

                    var model = new AdminDashboardViewModel();
                    model.TotalUsers = users.Count;
                    model.ActiveUsers = users.Count(u => u.IsActive);
                    model.LockedUsers = users.Count(u => u.IsLocked);
                    model.Users = users.Select(u => new UserListItem
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
                    }).ToList();
                    model.RecentAudit = auditLogs.Select(a => new AuditLogItem
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
                    }).ToList();

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleUserLock(int userId)
        {
            try
            {
                using (var db = new AnyStateClaimsContext())
                {
                    var user = db.Users.Find(userId);
                    if (user != null)
                    {
                        user.IsLocked = !user.IsLocked;
                        if (!user.IsLocked)
                        {
                            user.FailedLoginAttempts = 0;
                        }
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }

        [HttpGet]
        public ActionResult SystemConfig()
        {
            try
            {
                using (var db = new AnyStateClaimsContext())
                {
                    var configs = db.SystemConfigurations.OrderBy(c => c.Category).ThenBy(c => c.ConfigKey).ToList();
                    return View(configs);
                }
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateConfig(int configId, string configValue)
        {
            try
            {
                using (var db = new AnyStateClaimsContext())
                {
                    var config = db.SystemConfigurations.Find(configId);
                    if (config != null)
                    {
                        config.ConfigValue = configValue;
                        config.ModifiedBy = User.Identity.Name;
                        config.ModifiedDate = DateTime.Now;
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("SystemConfig");
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }
    }
}
