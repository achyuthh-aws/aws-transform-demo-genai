using System;
using System.Linq;
using System.Web.Mvc;
using AnyStateClaimsPortal.Web.DataAccess;
using AnyStateClaimsPortal.Web.Models;
using AnyStateClaimsPortal.Web.Models.Entities;

namespace AnyStateClaimsPortal.Web.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        public ActionResult Index(int? fiscalYear, int? agencyId)
        {
            int year = fiscalYear ?? DateTime.Today.Year;
            var repo = new ReportRepository();

            var model = new ReportViewModel
            {
                FiscalYear = year,
                SelectedAgencyId = agencyId,
                AgencyReports = repo.GetAgencyClaimsReport(),
                AgingBuckets = repo.GetOpenClaimsAging()
            };

            var financial = repo.GetFinancialSummary(year, agencyId);
            ViewBag.FinancialSummary = financial;

            using (var db = new AnyStateClaimsContext())
            {
                var currentYear = DateTime.Today.Year;
                model.FiscalYears = new SelectList(
                    Enumerable.Range(currentYear - 5, 6).Reverse().Select(y => new { Id = y, Name = y.ToString() }),
                    "Id", "Name", year);
                model.Agencies = new SelectList(
                    db.Agencies.Where(a => a.IsActive).OrderBy(a => a.AgencyName).ToList(),
                    "AgencyId", "AgencyName");
            }

            return View(model);
        }
    }
}
