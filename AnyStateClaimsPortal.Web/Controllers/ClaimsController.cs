using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AnyStateClaimsPortal.Web.BusinessLogic;
using AnyStateClaimsPortal.Web.DataAccess;
using AnyStateClaimsPortal.Web.Models;
using AnyStateClaimsPortal.Web.Models.Entities;

namespace AnyStateClaimsPortal.Web.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        private readonly ClaimWorkflowEngine _workflow = new ClaimWorkflowEngine();
        private readonly ValidationService _validation = new ValidationService();

        public ActionResult Index(ClaimSearchViewModel search)
        {
            if (search == null) search = new ClaimSearchViewModel();

            var repo = new ClaimsRepository();
            var result = repo.SearchClaims(
                search.SearchTerm, search.Status, search.AgencyId, search.InjuryType,
                search.Priority, search.DateFrom, search.DateTo,
                search.AdjusterId, search.IsLitigated, search.PageNumber, search.PageSize);

            search.Results = result.Claims;
            search.TotalCount = result.TotalCount;

            using (var db = new AnyStateClaimsContext())
            {
                search.Agencies = new SelectList(db.Agencies.Where(a => a.IsActive).OrderBy(a => a.AgencyName).ToList(), "AgencyId", "AgencyName");
                search.Statuses = new SelectList(new[] { "Submitted", "UnderReview", "MedicalReview", "Approved", "Denied", "Closed", "Reopened" });
                search.InjuryTypes = new SelectList(new[] { "Medical", "Temporary", "Permanent", "Fatal" });
                search.Priorities = new SelectList(new[] { "Low", "Medium", "High", "Critical" });
                search.Adjusters = new SelectList(db.Users.Where(u => u.IsActive && u.Role == "ClaimsAdjuster").OrderBy(u => u.FullName).ToList(), "UserId", "FullName");
            }

            return View(search);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var model = new ClaimViewModel();
            PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClaimViewModel model)
        {
            var errors = _validation.ValidateClaimSubmission(model);
            foreach (var e in errors)
                ModelState.AddModelError("", e);

            if (!ModelState.IsValid)
            {
                PopulateDropdowns(model);
                return View(model);
            }

            using (var db = new AnyStateClaimsContext())
            {
                var claim = new Claim
                {
                    ClaimNumber = _workflow.GenerateClaimNumber(),
                    EmployeeId = model.EmployeeId,
                    InjuryDate = model.InjuryDate,
                    ReportedDate = DateTime.Today,
                    InjuryDescription = model.InjuryDescription,
                    InjuryType = model.InjuryType,
                    BodyPartAffected = model.BodyPartAffected,
                    IncidentLocation = model.IncidentLocation,
                    LocationType = model.LocationType,
                    WitnessName = model.WitnessName,
                    WitnessPhone = model.WitnessPhone,
                    WitnessStatement = model.WitnessStatement,
                    Status = "Submitted",
                    Priority = model.Priority ?? "Medium",
                    CreatedBy = User.Identity.Name,
                    CreatedDate = DateTime.Now
                };

                db.Claims.Add(claim);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = claim.ClaimId });
            }
        }

        public ActionResult Details(int id)
        {
            using (var db = new AnyStateClaimsContext())
            {
                var claim = db.Claims
                    .Include(c => c.Employee)
                    .Include(c => c.Employee.Agency)
                    .Include(c => c.AssignedAdjuster)
                    .Include(c => c.MedicalReviewer)
                    .Include(c => c.ClaimNotes)
                    .FirstOrDefault(c => c.ClaimId == id);

                if (claim == null) return HttpNotFound();

                var model = MapToViewModel(claim);

                var medRepo = new MedicalRepository();
                ViewBag.Treatments = medRepo.GetTreatmentHistory(id);

                var payRepo = new PaymentRepository();
                ViewBag.Payments = payRepo.GetPaymentsByClaimId(id);

                ViewBag.AllowedStatuses = _workflow.GetAllowedTransitions(claim.Status);
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            using (var db = new AnyStateClaimsContext())
            {
                var claim = db.Claims
                    .Include(c => c.Employee)
                    .Include(c => c.Employee.Agency)
                    .FirstOrDefault(c => c.ClaimId == id);

                if (claim == null) return HttpNotFound();

                var model = MapToViewModel(claim);
                PopulateDropdowns(model);
                ViewBag.AllowedStatuses = _workflow.GetAllowedTransitions(claim.Status);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ClaimViewModel model)
        {
            using (var db = new AnyStateClaimsContext())
            {
                var claim = db.Claims.Find(id);
                if (claim == null) return HttpNotFound();

                if (claim.Status != model.Status)
                {
                    var errors = _validation.ValidateStatusTransition(claim.Status, model.Status, model);
                    foreach (var e in errors)
                        ModelState.AddModelError("", e);
                }

                if (!ModelState.IsValid)
                {
                    PopulateDropdowns(model);
                    ViewBag.AllowedStatuses = _workflow.GetAllowedTransitions(claim.Status);
                    return View(model);
                }

                claim.InjuryDescription = model.InjuryDescription;
                claim.InjuryType = model.InjuryType;
                claim.BodyPartAffected = model.BodyPartAffected;
                claim.IncidentLocation = model.IncidentLocation;
                claim.LocationType = model.LocationType;
                claim.Priority = model.Priority;
                claim.AssignedAdjusterId = model.AssignedAdjusterId;
                claim.MedicalReviewerId = model.MedicalReviewerId;
                claim.DenialReason = model.DenialReason;
                claim.IsLitigated = model.IsLitigated;
                claim.ReturnToWorkDate = model.ReturnToWorkDate;
                claim.Status = model.Status;
                claim.ModifiedBy = User.Identity.Name;
                claim.ModifiedDate = DateTime.Now;

                if (model.Status == "Approved" && claim.WeeklyBenefitAmount == null)
                {
                    var repo = new ClaimsRepository();
                    claim.WeeklyBenefitAmount = repo.CalculateWeeklyBenefit(claim.EmployeeId, claim.InjuryType);
                }

                if (!string.IsNullOrWhiteSpace(model.NewNoteText))
                {
                    db.ClaimNotes.Add(new ClaimNote
                    {
                        ClaimId = id,
                        NoteText = model.NewNoteText,
                        NoteType = model.NewNoteType ?? "General",
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now
                    });
                }

                db.SaveChanges();
                return RedirectToAction("Details", new { id });
            }
        }

        private void PopulateDropdowns(ClaimViewModel model)
        {
            using (var db = new AnyStateClaimsContext())
            {
                model.Employees = new SelectList(
                    db.Employees.Where(e => e.IsActive).OrderBy(e => e.LastName).ToList(),
                    "EmployeeId", "FullName");
                model.Adjusters = new SelectList(
                    db.Users.Where(u => u.IsActive && u.Role == "ClaimsAdjuster").OrderBy(u => u.FullName).ToList(),
                    "UserId", "FullName");
                model.MedicalReviewers = new SelectList(
                    db.Users.Where(u => u.IsActive && u.Role == "MedicalReviewer").OrderBy(u => u.FullName).ToList(),
                    "UserId", "FullName");
                model.InjuryTypes = new SelectList(new[] { "Medical", "Temporary", "Permanent", "Fatal" });
                model.Priorities = new SelectList(new[] { "Low", "Medium", "High", "Critical" });
                model.LocationTypes = new SelectList(new[] { "Office", "Field", "Warehouse", "Vehicle", "Other" });
            }
        }

        private ClaimViewModel MapToViewModel(Claim c)
        {
            return new ClaimViewModel
            {
                ClaimId = c.ClaimId,
                ClaimNumber = c.ClaimNumber,
                EmployeeId = c.EmployeeId,
                InjuryDate = c.InjuryDate,
                InjuryDescription = c.InjuryDescription,
                InjuryType = c.InjuryType,
                BodyPartAffected = c.BodyPartAffected,
                IncidentLocation = c.IncidentLocation,
                LocationType = c.LocationType,
                WitnessName = c.WitnessName,
                WitnessPhone = c.WitnessPhone,
                WitnessStatement = c.WitnessStatement,
                Status = c.Status,
                Priority = c.Priority,
                AssignedAdjusterId = c.AssignedAdjusterId,
                MedicalReviewerId = c.MedicalReviewerId,
                WeeklyBenefitAmount = c.WeeklyBenefitAmount,
                TotalPaidAmount = c.TotalPaidAmount,
                TotalMedicalCost = c.TotalMedicalCost,
                TotalReserveAmount = c.TotalReserveAmount,
                DenialReason = c.DenialReason,
                IsLitigated = c.IsLitigated,
                ReturnToWorkDate = c.ReturnToWorkDate,
                CreatedDate = c.CreatedDate,
                EmployeeName = c.Employee?.FullName,
                AgencyName = c.Employee?.Agency?.AgencyName,
                AdjusterName = c.AssignedAdjuster?.FullName,
                Notes = c.ClaimNotes?.OrderByDescending(n => n.CreatedDate).Select(n => new ClaimNoteViewModel
                {
                    NoteText = n.NoteText,
                    NoteType = n.NoteType,
                    IsConfidential = n.IsConfidential,
                    CreatedBy = n.CreatedBy,
                    CreatedDate = n.CreatedDate
                }).ToList()
            };
        }
    }
}
