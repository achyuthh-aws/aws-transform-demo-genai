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
    public class MedicalController : Controller
    {
        public ActionResult Index(int claimId)
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

                    var treatments = db.MedicalTreatments
                        .Include("Provider")
                        .Where(t => t.ClaimId == claimId)
                        .OrderByDescending(t => t.TreatmentDate)
                        .ToList()
                        .Select(t => new TreatmentViewModel
                        {
                            TreatmentId = t.TreatmentId,
                            TreatmentDate = t.TreatmentDate,
                            TreatmentType = t.TreatmentType,
                            Description = t.Description,
                            Diagnosis = t.Diagnosis,
                            ICDCode = t.ICDCode,
                            CPTCode = t.CPTCode,
                            BilledAmount = t.BilledAmount ?? 0,
                            ApprovedAmount = t.ApprovedAmount ?? 0,
                            Status = t.Status,
                            Notes = t.Notes,
                            NextAppointment = t.NextAppointment,
                            ProviderName = t.Provider != null ? t.Provider.ProviderName : "",
                            ProviderType = t.Provider != null ? t.Provider.ProviderType : "",
                            ProviderPhone = t.Provider != null ? t.Provider.Phone : ""
                        })
                        .ToList();

                    var model = new MedicalSummaryViewModel();
                    model.ClaimId = claimId;
                    model.ClaimNumber = claim.ClaimNumber;
                    model.Treatments = treatments;
                    model.TotalBilled = treatments.Sum(t => t.BilledAmount);
                    model.TotalApproved = treatments.Sum(t => t.ApprovedAmount);

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }

        [HttpGet]
        public ActionResult AddTreatment(int claimId)
        {
            try
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
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTreatment(MedicalTreatment model)
        {
            try
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
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }
    }
}
