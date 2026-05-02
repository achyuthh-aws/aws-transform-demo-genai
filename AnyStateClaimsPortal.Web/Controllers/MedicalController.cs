using System;
using System.Linq;
using System.Web.Mvc;
using AnyStateClaimsPortal.Web.DataAccess;
using AnyStateClaimsPortal.Web.Models;
using AnyStateClaimsPortal.Web.Models.Entities;

namespace AnyStateClaimsPortal.Web.Controllers
{
    [Authorize]
    public class MedicalController : Controller
    {
        public ActionResult Index(int claimId)
        {
            var repo = new MedicalRepository();
            using (var db = new AnyStateClaimsContext())
            {
                var claim = db.Claims.Find(claimId);
                if (claim == null) return HttpNotFound();

                var model = new MedicalSummaryViewModel
                {
                    ClaimId = claimId,
                    ClaimNumber = claim.ClaimNumber,
                    Treatments = repo.GetTreatmentHistory(claimId),
                    TotalBilled = claim.TotalMedicalCost,
                    TotalApproved = claim.TotalPaidAmount
                };
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult AddTreatment(int claimId)
        {
            using (var db = new AnyStateClaimsContext())
            {
                ViewBag.ClaimId = claimId;
                ViewBag.Providers = new SelectList(
                    db.MedicalProviders.Where(p => p.IsActive).OrderBy(p => p.ProviderName).ToList(),
                    "ProviderId", "ProviderName");
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTreatment(MedicalTreatment model)
        {
            if (!ModelState.IsValid)
            {
                using (var db = new AnyStateClaimsContext())
                {
                    ViewBag.ClaimId = model.ClaimId;
                    ViewBag.Providers = new SelectList(
                        db.MedicalProviders.Where(p => p.IsActive).OrderBy(p => p.ProviderName).ToList(),
                        "ProviderId", "ProviderName");
                }
                return View(model);
            }

            using (var db = new AnyStateClaimsContext())
            {
                model.Status = "Submitted";
                model.CreatedBy = User.Identity.Name;
                model.CreatedDate = DateTime.Now;
                db.MedicalTreatments.Add(model);
                db.SaveChanges();
            }

            return RedirectToAction("Index", new { claimId = model.ClaimId });
        }
    }
}
