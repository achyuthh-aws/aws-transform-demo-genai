using System;
using System.Collections.Generic;
using System.Linq;
using AnyStateClaimsPortal.Web.Models;
using AnyStateClaimsPortal.Web.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace AnyStateClaimsPortal.Web.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        public ActionResult Index(int claimId)
        {
            try
            {
                using (var db = new AnyStateClaimsContext())
                {
                    var payments = db.ClaimPayments
                        .Where(p => p.ClaimId == claimId)
                        .OrderByDescending(p => p.PaymentDate)
                        .ToList()
                        .Select(p => new PaymentListItem
                        {
                            PaymentId = p.PaymentId,
                            ClaimId = p.ClaimId,
                            PaymentDate = p.PaymentDate,
                            PaymentType = p.PaymentType,
                            Amount = p.Amount,
                            CheckNumber = p.CheckNumber,
                            PayeeName = p.PayeeName,
                            PayeeType = p.PayeeType,
                            PaymentStatus = p.Status,
                            FiscalYear = p.FiscalYear
                        })
                        .ToList();

                    ViewBag.ClaimId = claimId;
                    return View(payments);
                }
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }

        [HttpGet]
        public ActionResult Create(int claimId)
        {
            try
            {
                using (var db = new AnyStateClaimsContext())
                {
                    var claim = db.Claims.Find(claimId);
                    if (claim == null)
                    {
                        return NotFound();
                    }

                    var model = new PaymentViewModel();
                    model.ClaimId = claimId;
                    model.ClaimNumber = claim.ClaimNumber;
                    model.PaymentDate = DateTime.Today;
                    model.FiscalYear = DateTime.Today.Year;
                    model.PaymentTypes = new SelectList(new[] { "Medical", "Indemnity", "Legal", "Rehabilitation", "Other" });
                    model.PayeeTypes = new SelectList(new[] { "Employee", "Provider", "Attorney", "Vendor" });
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
        public ActionResult Create(PaymentViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.PaymentTypes = new SelectList(new[] { "Medical", "Indemnity", "Legal", "Rehabilitation", "Other" });
                    model.PayeeTypes = new SelectList(new[] { "Employee", "Provider", "Attorney", "Vendor" });
                    return View(model);
                }

                using (var db = new AnyStateClaimsContext())
                {
                    var payment = new ClaimPayment();
                    payment.ClaimId = model.ClaimId;
                    payment.PaymentDate = model.PaymentDate;
                    payment.PaymentType = model.PaymentType;
                    payment.Amount = model.Amount;
                    payment.CheckNumber = model.CheckNumber;
                    payment.PayeeName = model.PayeeName;
                    payment.PayeeType = model.PayeeType;
                    payment.Description = model.Description;
                    payment.FiscalYear = model.FiscalYear;
                    payment.Status = "Pending";
                    payment.CreatedBy = User.Identity.Name;
                    payment.CreatedDate = DateTime.Now;

                    db.ClaimPayments.Add(payment);
                    db.SaveChanges();
                }

                return RedirectToAction("Index", new { claimId = model.ClaimId });
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }
    }
}
