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
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                using (var db = new AnyStateClaimsContext())
                {
                    var claims = db.Claims
                        .Include("Employee")
                        .Include("Employee.Agency")
                        .ToList();

                    var model = new DashboardViewModel();

                    model.TotalClaims = claims.Count;
                    model.TotalPaid = claims.Sum(c => c.TotalPaidAmount);
                    model.TotalReserves = claims.Sum(c => c.TotalReserveAmount);

                    model.StatusSummaries = claims
                        .GroupBy(c => c.Status)
                        .Select(g => new StatusSummary
                        {
                            Status = g.Key,
                            ClaimCount = g.Count(),
                            TotalWeeklyBenefits = g.Sum(c => c.WeeklyBenefitAmount ?? 0),
                            TotalPaid = g.Sum(c => c.TotalPaidAmount),
                            TotalReserves = g.Sum(c => c.TotalReserveAmount)
                        })
                        .ToList();

                    model.RecentClaims = claims
                        .OrderByDescending(c => c.CreatedDate)
                        .Take(15)
                        .Select(c => new RecentClaimViewModel
                        {
                            ClaimId = c.ClaimId,
                            ClaimNumber = c.ClaimNumber,
                            InjuryDate = c.InjuryDate,
                            InjuryType = c.InjuryType,
                            Status = c.Status,
                            Priority = c.Priority,
                            EmployeeName = c.Employee != null ? c.Employee.FullName : "",
                            AgencyName = (c.Employee != null && c.Employee.Agency != null) ? c.Employee.Agency.AgencyName : "",
                            AgencyCode = (c.Employee != null && c.Employee.Agency != null) ? c.Employee.Agency.AgencyCode : "",
                            CreatedDate = c.CreatedDate
                        })
                        .ToList();

                    model.AgencySummaries = claims
                        .Where(c => c.Employee != null && c.Employee.Agency != null)
                        .GroupBy(c => c.Employee.Agency)
                        .Select(g => new AgencySummary
                        {
                            AgencyName = g.Key.AgencyName,
                            AgencyCode = g.Key.AgencyCode,
                            RiskCategory = g.Key.RiskCategory,
                            ClaimCount = g.Count(),
                            TotalPaid = g.Sum(c => c.TotalPaidAmount),
                            TotalMedical = g.Sum(c => c.TotalMedicalCost)
                        })
                        .ToList();

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            return View();
        }
    }
}
