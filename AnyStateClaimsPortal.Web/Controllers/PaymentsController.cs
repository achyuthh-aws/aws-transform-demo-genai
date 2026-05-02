using System;
using System.Linq;
using System.Web.Mvc;
using AnyStateClaimsPortal.Web.DataAccess;
using AnyStateClaimsPortal.Web.Models;
using AnyStateClaimsPortal.Web.Models.Entities;

namespace AnyStateClaimsPortal.Web.Controllers
{
    [Authorize(Roles = "Administrator,ClaimsAdjuster")]
    public class PaymentsController : Controller
    {
        public ActionResult Index(int claimId)
        {
            var repo = new PaymentRepository();
            var payments = repo.GetPaymentsByClaimId(claimId);
            ViewBag.ClaimId = claimId;
            return View(payments);
        }

        [HttpGet]
        public ActionResult Create(int claimId)
        {
            using (var db = new AnyStateClaimsContext())
            {
                var claim = db.Claims.Find(claimId);
                if (claim == null) return HttpNotFound();

                var model = new PaymentViewModel
                {
                    ClaimId = claimId,
                    ClaimNumber = claim.ClaimNumber,
                    PaymentDate = DateTime.Today,
                    FiscalYear = DateTime.Today.Year,
                    PaymentTypes = new SelectList(new[] { "Medical", "Indemnity", "Legal", "Rehabilitation", "Other" }),
                    PayeeTypes = new SelectList(new[] { "Employee", "Provider", "Attorney", "Vendor" })
                };
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.PaymentTypes = new SelectList(new[] { "Medical", "Indemnity", "Legal", "Rehabilitation", "Other" });
                model.PayeeTypes = new SelectList(new[] { "Employee", "Provider", "Attorney", "Vendor" });
                return View(model);
            }

            using (var db = new AnyStateClaimsContext())
            {
                db.ClaimPayments.Add(new ClaimPayment
                {
                    ClaimId = model.ClaimId,
                    PaymentDate = model.PaymentDate,
                    PaymentType = model.PaymentType,
                    Amount = model.Amount,
                    CheckNumber = model.CheckNumber,
                    PayeeName = model.PayeeName,
                    PayeeType = model.PayeeType,
                    Description = model.Description,
                    FiscalYear = model.FiscalYear,
                    Status = "Pending",
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now
                });
                db.SaveChanges();
            }

            return RedirectToAction("Index", new { claimId = model.ClaimId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessBatch()
        {
            var repo = new PaymentRepository();
            var result = repo.ProcessPaymentBatch(User.Identity.Name);
            TempData["Success"] = $"Processed {result.Count} payments totaling {result.Total:C}.";
            return RedirectToAction("Index", "Home");
        }
    }
}
