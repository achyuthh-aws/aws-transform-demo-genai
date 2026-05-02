using System;
using System.Collections.Generic;
using AnyStateClaimsPortal.Web.Models;

namespace AnyStateClaimsPortal.Web.BusinessLogic
{
    public class ValidationService
    {
        private readonly ClaimWorkflowEngine _workflow = new ClaimWorkflowEngine();

        public List<string> ValidateClaimSubmission(ClaimViewModel model)
        {
            var errors = new List<string>();
            if (model.InjuryDate > DateTime.Today)
                errors.Add("Injury date cannot be in the future.");
            if (model.InjuryDate < DateTime.Today.AddYears(-1))
                errors.Add("Injury date cannot be more than 1 year ago.");
            if (string.IsNullOrWhiteSpace(model.Description) || model.Description.Trim().Length < 20)
                errors.Add("Description must be at least 20 characters.");
            return errors;
        }

        public List<string> ValidateStatusTransition(string oldStatus, string newStatus, ClaimViewModel model)
        {
            var errors = new List<string>();
            if (!_workflow.CanTransition(oldStatus, newStatus))
                errors.Add($"Cannot transition from '{oldStatus}' to '{newStatus}'.");
            if (_workflow.RequiresDenialReason(newStatus) && string.IsNullOrWhiteSpace(model.DenialReason))
                errors.Add("A denial reason is required.");
            if (_workflow.RequiresAdjusterAssignment(newStatus) && (model.AdjusterId == null || model.AdjusterId == 0))
                errors.Add("An adjuster must be assigned for review.");
            return errors;
        }

        public List<string> ValidatePayment(PaymentViewModel model)
        {
            var errors = new List<string>();
            if (model.Amount <= 0)
                errors.Add("Payment amount must be greater than zero.");
            return errors;
        }
    }
}
