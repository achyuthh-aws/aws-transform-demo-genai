using System;
using System.Collections.Generic;
using System.Linq;
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
            return _transitions.ContainsKey(currentStatus) && _transitions[currentStatus].Contains(newStatus);
        }

        public List<string> GetAllowedTransitions(string currentStatus)
        {
            return _transitions.ContainsKey(currentStatus) ? _transitions[currentStatus] : new List<string>();
        }

        public string GenerateClaimNumber()
        {
            int seq = Interlocked.Increment(ref _claimSequence);
            return $"WC-{DateTime.Now.Year}-{seq:D4}";
        }

        public bool RequiresAdjusterAssignment(string status) => status == "UnderReview";

        public bool RequiresDenialReason(string status) => status == "Denied";

        public bool RequiresMedicalReview(string status) => status == "MedicalReview";

        public bool IsTerminalStatus(string status) => status == "Closed";

        public string GetStatusDisplayClass(string status)
        {
            switch (status)
            {
                case "Submitted": return "label-info";
                case "UnderReview": return "label-warning";
                case "MedicalReview": return "label-primary";
                case "Approved": return "label-success";
                case "Denied": return "label-danger";
                case "Closed": return "label-default";
                case "Reopened": return "label-warning";
                default: return "label-default";
            }
        }
    }
}
