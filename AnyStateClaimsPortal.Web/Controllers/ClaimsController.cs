using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AnyStateClaimsPortal.Web.BusinessLogic;
using AnyStateClaimsPortal.Web.Models;
using AnyStateClaimsPortal.Web.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace AnyStateClaimsPortal.Web.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        private readonly ClaimWorkflowEngine _workflow = new ClaimWorkflowEngine();

        public ActionResult Index(ClaimSearchViewModel search)
        {
            try
            {
                if (search == null)
                {
                    search = new ClaimSearchViewModel();
                }

                using (var db = new AnyStateClaimsContext())
                {
                    var query = db.Claims
                        .Include("Employee")
                        .Include("Employee.Agency")
                        .Include("AssignedAdjuster")
                        .AsQueryable();

                    if (!string.IsNullOrEmpty(search.SearchTerm))
                    {
                        string term = search.SearchTerm;
                        query = query.Where(c =>
                            c.ClaimNumber.Contains(term) ||
                            c.Employee.FirstName.Contains(term) ||
                            c.Employee.LastName.Contains(term) ||
                            c.InjuryDescription.Contains(term));
                    }

                    if (!string.IsNullOrEmpty(search.Status))
                    {
                        query = query.Where(c => c.Status == search.Status);
                    }

                    if (search.AgencyId.HasValue)
                    {
                        query = query.Where(c => c.Employee.AgencyId == search.AgencyId.Value);
                    }

                    if (!string.IsNullOrEmpty(search.InjuryType))
                    {
                        query = query.Where(c => c.InjuryType == search.InjuryType);
                    }

                    var claims = query.OrderByDescending(c => c.CreatedDate).ToList();

                    search.TotalCount = claims.Count;
                    search.Results = claims.Select(c => new ClaimListItemViewModel
                    {
                        ClaimId = c.ClaimId,
                        ClaimNumber = c.ClaimNumber,
                        InjuryDate = c.InjuryDate,
                        InjuryType = c.InjuryType,
                        BodyPartAffected = c.BodyPartAffected,
                        Status = c.Status,
                        Priority = c.Priority,
                        WeeklyBenefitAmount = c.WeeklyBenefitAmount,
                        TotalPaidAmount = c.TotalPaidAmount,
                        TotalMedicalCost = c.TotalMedicalCost,
                        IsLitigated = c.IsLitigated,
                        EmployeeName = c.Employee != null ? c.Employee.FullName : "",
                        EmployeeNumber = c.Employee != null ? c.Employee.EmployeeNumber : "",
                        AgencyName = (c.Employee != null && c.Employee.Agency != null) ? c.Employee.Agency.AgencyName : "",
                        AgencyCode = (c.Employee != null && c.Employee.Agency != null) ? c.Employee.Agency.AgencyCode : "",
                        AdjusterName = c.AssignedAdjuster != null ? c.AssignedAdjuster.FullName : "",
                        CreatedDate = c.CreatedDate
                    }).ToList();

                    search.Agencies = new SelectList(db.Agencies.Where(a => a.IsActive).OrderBy(a => a.AgencyName).ToList(), "AgencyId", "AgencyName");
                    search.Statuses = new SelectList(new[] { "Submitted", "UnderReview", "MedicalReview", "Approved", "Denied", "Closed", "Reopened" });
                    search.InjuryTypes = new SelectList(new[] { "Medical", "Temporary", "Permanent", "Fatal" });
                    search.Priorities = new SelectList(new[] { "Low", "Normal", "High", "Urgent" });
                    search.Adjusters = new SelectList(db.Users.Where(u => u.IsActive && u.Role == "ClaimsAdjuster").OrderBy(u => u.FullName).ToList(), "UserId", "FullName");
                }

                return View(search);
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                var model = new ClaimViewModel();
                PopulateDropdowns(model);
                return View(model);
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClaimViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    PopulateDropdowns(model);
                    return View(model);
                }

                using (var db = new AnyStateClaimsContext())
                {
                    var rng = new Random();
                    var claim = new Claim
                    {
                        ClaimNumber = string.Format("WC-{0}-{1:D4}", DateTime.Now.Year, rng.Next(1, 9999)),
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
                        Priority = model.Priority ?? "Normal",
                        TotalPaidAmount = 0,
                        TotalMedicalCost = 0,
                        TotalReserveAmount = 0,
                        CreatedBy = User.Identity.Name,
                        CreatedDate = DateTime.Now
                    };

                    db.Claims.Add(claim);
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = claim.ClaimId });
                }
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }

        public ActionResult Details(int id)
        {
            try
            {
                using (var db = new AnyStateClaimsContext())
                {
                    var claim = db.Claims
                        .Include("Employee")
                        .Include("Employee.Agency")
                        .Include("AssignedAdjuster")
                        .Include("MedicalReviewer")
                        .Include("ClaimNotes")
                        .Include("ClaimPayments")
                        .FirstOrDefault(c => c.ClaimId == id);

                    if (claim == null)
                    {
                        return NotFound();
                    }

                    var model = MapToViewModel(claim);
                    ViewBag.AllowedStatuses = new SelectList(_workflow.GetAllowedTransitions(claim.Status));
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            try
            {
                using (var db = new AnyStateClaimsContext())
                {
                    var claim = db.Claims
                        .Include("Employee")
                        .Include("Employee.Agency")
                        .Include("ClaimNotes")
                        .FirstOrDefault(c => c.ClaimId == id);

                    if (claim == null)
                    {
                        return NotFound();
                    }

                    var model = MapToViewModel(claim);
                    PopulateDropdowns(model);
                    ViewBag.AllowedStatuses = new SelectList(_workflow.GetAllowedTransitions(claim.Status));
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
        public ActionResult Edit(int id, ClaimViewModel model)
        {
            try
            {
                using (var db = new AnyStateClaimsContext())
                {
                    var claim = db.Claims.Find(id);
                    if (claim == null)
                    {
                        return NotFound();
                    }

                    if (claim.Status != model.Status && !_workflow.CanTransition(claim.Status, model.Status))
                    {
                        ModelState.AddModelError("", string.Format("Cannot transition from {0} to {1}.", claim.Status, model.Status));
                    }

                    if (!ModelState.IsValid)
                    {
                        PopulateDropdowns(model);
                        ViewBag.AllowedStatuses = new SelectList(_workflow.GetAllowedTransitions(claim.Status));
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
                    return RedirectToAction("Details", new { id = id });
                }
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
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
                model.Priorities = new SelectList(new[] { "Low", "Normal", "High", "Urgent" });
                model.LocationTypes = new SelectList(new[] { "Office", "Field", "Warehouse", "Vehicle", "Other" });
            }
        }

        private ClaimViewModel MapToViewModel(Claim c)
        {
            var vm = new ClaimViewModel();
            vm.ClaimId = c.ClaimId;
            vm.ClaimNumber = c.ClaimNumber;
            vm.EmployeeId = c.EmployeeId;
            vm.InjuryDate = c.InjuryDate;
            vm.InjuryDescription = c.InjuryDescription;
            vm.InjuryType = c.InjuryType;
            vm.BodyPartAffected = c.BodyPartAffected;
            vm.IncidentLocation = c.IncidentLocation;
            vm.LocationType = c.LocationType;
            vm.WitnessName = c.WitnessName;
            vm.WitnessPhone = c.WitnessPhone;
            vm.WitnessStatement = c.WitnessStatement;
            vm.Status = c.Status;
            vm.Priority = c.Priority;
            vm.AssignedAdjusterId = c.AssignedAdjusterId;
            vm.MedicalReviewerId = c.MedicalReviewerId;
            vm.WeeklyBenefitAmount = c.WeeklyBenefitAmount;
            vm.TotalPaidAmount = c.TotalPaidAmount;
            vm.TotalMedicalCost = c.TotalMedicalCost;
            vm.TotalReserveAmount = c.TotalReserveAmount;
            vm.DenialReason = c.DenialReason;
            vm.IsLitigated = c.IsLitigated;
            vm.ReturnToWorkDate = c.ReturnToWorkDate;
            vm.CreatedDate = c.CreatedDate;
            vm.EmployeeName = c.Employee != null ? c.Employee.FullName : "";
            vm.AgencyName = (c.Employee != null && c.Employee.Agency != null) ? c.Employee.Agency.AgencyName : "";
            vm.AdjusterName = c.AssignedAdjuster != null ? c.AssignedAdjuster.FullName : "";

            if (c.ClaimNotes != null)
            {
                vm.Notes = c.ClaimNotes.OrderByDescending(n => n.CreatedDate).Select(n => new ClaimNoteViewModel
                {
                    NoteText = n.NoteText,
                    NoteType = n.NoteType,
                    IsConfidential = n.IsConfidential,
                    CreatedBy = n.CreatedBy,
                    CreatedDate = n.CreatedDate
                }).ToList();
            }
            else
            {
                vm.Notes = new List<ClaimNoteViewModel>();
            }

            return vm;
        }
    }
}
