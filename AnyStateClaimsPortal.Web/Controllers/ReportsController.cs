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
    public class ReportsController : Controller
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

                    var agencyGroups = claims
                        .Where(c => c.Employee != null && c.Employee.Agency != null)
                        .GroupBy(c => c.Employee.Agency);

                    var agencyReports = new List<AgencyReportItem>();
                    foreach (var g in agencyGroups)
                    {
                        var item = new AgencyReportItem();
                        item.AgencyId = g.Key.AgencyId;
                        item.AgencyCode = g.Key.AgencyCode;
                        item.AgencyName = g.Key.AgencyName;
                        item.AgencyType = g.Key.AgencyType;
                        item.RiskCategory = g.Key.RiskCategory;
                        item.AnnualPremium = g.Key.AnnualPremium ?? 0;
                        item.TotalClaims = g.Count();
                        item.SubmittedCount = g.Count(c => c.Status == "Submitted");
                        item.UnderReviewCount = g.Count(c => c.Status == "UnderReview");
                        item.MedicalReviewCount = g.Count(c => c.Status == "MedicalReview");
                        item.ApprovedCount = g.Count(c => c.Status == "Approved");
                        item.DeniedCount = g.Count(c => c.Status == "Denied");
                        item.ClosedCount = g.Count(c => c.Status == "Closed");
                        item.LitigatedCount = g.Count(c => c.IsLitigated);
                        item.TotalPaidAmount = g.Sum(c => c.TotalPaidAmount);
                        item.TotalMedicalCost = g.Sum(c => c.TotalMedicalCost);
                        item.TotalIncurredCost = item.TotalPaidAmount + item.TotalMedicalCost;
                        item.AvgWeeklyBenefit = g.Where(c => c.WeeklyBenefitAmount.HasValue).Any()
                            ? g.Where(c => c.WeeklyBenefitAmount.HasValue).Average(c => c.WeeklyBenefitAmount.Value)
                            : 0;
                        item.LossRatio = item.AnnualPremium > 0
                            ? item.TotalIncurredCost / item.AnnualPremium
                            : 0;
                        agencyReports.Add(item);
                    }

                    var model = new ReportViewModel();
                    model.AgencyReports = agencyReports;
                    model.FiscalYear = DateTime.Today.Year;

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }
    }
}
