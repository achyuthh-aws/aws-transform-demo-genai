using System;
using System.Collections.Generic;
using System.Threading;

namespace AnyStateClaimsPortal.Web.BusinessLogic
{
    public class ClaimWorkflowEngine
    {
        private static int _claimSequence;

        private static readonly Dictionary<string, List<string>> _transitions = new Dictionary<string, List<string>>
        {
            { "Submitted", new List<string> { "UnderReview", "Denied" } },
            { "UnderReview", new List<string> { "MedicalReview", "Approved", "Denied" } },
            { "MedicalReview", new List<string> { "Approved", "Denied", "UnderReview" } },
            { "Approved", new List<string> { "Closed" } },
            { "Denied", new List<string> { "UnderReview", "Closed" } },
            { "Closed", new List<string> { "Reopened" } },
            { "Reopened", new List<string> { "UnderReview" } }
        };

        public bool CanTransition(string currentStatus, string newStatus)
        {
            if (currentStatus == null || newStatus == null)
            {
                return false;
            }
            if (!_transitions.ContainsKey(currentStatus))
            {
                return false;
            }
            return _transitions[currentStatus].Contains(newStatus);
        }

        public List<string> GetAllowedTransitions(string currentStatus)
        {
            if (currentStatus != null && _transitions.ContainsKey(currentStatus))
            {
                return _transitions[currentStatus];
            }
            return new List<string>();
        }

        public string GenerateClaimNumber()
        {
            int seq = Interlocked.Increment(ref _claimSequence);
            return string.Format("WC-{0}-{1:D4}", DateTime.Now.Year, seq);
        }

        public bool RequiresAdjusterAssignment(string status)
        {
            return status == "UnderReview";
        }

        public bool RequiresDenialReason(string status)
        {
            return status == "Denied";
        }

        public bool RequiresMedicalReview(string status)
        {
            return status == "MedicalReview";
        }

        public bool IsTerminalStatus(string status)
        {
            return status == "Closed";
        }

        public string GetStatusDisplayClass(string status)
        {
            if (status == null)
            {
                return "label-default";
            }
            switch (status)
            {
                case "Submitted":
                    return "label-info";
                case "UnderReview":
                    return "label-warning";
                case "MedicalReview":
                    return "label-primary";
                case "Approved":
                    return "label-success";
                case "Denied":
                    return "label-danger";
                case "Closed":
                    return "label-default";
                case "Reopened":
                    return "label-warning";
                default:
                    return "label-default";
            }
        }
    }
}
