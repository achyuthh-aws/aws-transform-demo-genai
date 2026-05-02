using System;
using System.Collections.Generic;
using NUnit.Framework;
using AnyStateClaimsPortal.Web.BusinessLogic;

namespace AnyStateClaimsPortal.Tests.BusinessLogic
{
    [TestFixture]
    public class ClaimWorkflowEngineTests
    {
        private ClaimWorkflowEngine _engine;

        [SetUp]
        public void SetUp()
        {
            _engine = new ClaimWorkflowEngine();
        }

        [Test]
        public void CanTransition_SubmittedToUnderReview_True()
        {
            Assert.IsTrue(_engine.CanTransition("Submitted", "UnderReview"));
        }

        [Test]
        public void CanTransition_SubmittedToApproved_False()
        {
            Assert.IsFalse(_engine.CanTransition("Submitted", "Approved"));
        }

        [Test]
        public void CanTransition_UnderReviewToMedicalReview_True()
        {
            Assert.IsTrue(_engine.CanTransition("UnderReview", "MedicalReview"));
        }

        [Test]
        public void CanTransition_UnderReviewToApproved_True()
        {
            Assert.IsTrue(_engine.CanTransition("UnderReview", "Approved"));
        }

        [Test]
        public void CanTransition_UnderReviewToDenied_True()
        {
            Assert.IsTrue(_engine.CanTransition("UnderReview", "Denied"));
        }

        [Test]
        public void CanTransition_MedicalReviewToApproved_True()
        {
            Assert.IsTrue(_engine.CanTransition("MedicalReview", "Approved"));
        }

        [Test]
        public void CanTransition_ApprovedToClosed_True()
        {
            Assert.IsTrue(_engine.CanTransition("Approved", "Closed"));
        }

        [Test]
        public void CanTransition_ClosedToReopened_True()
        {
            Assert.IsTrue(_engine.CanTransition("Closed", "Reopened"));
        }

        [Test]
        public void CanTransition_ReopenedToUnderReview_True()
        {
            Assert.IsTrue(_engine.CanTransition("Reopened", "UnderReview"));
        }

        [Test]
        public void CanTransition_ClosedToApproved_False()
        {
            Assert.IsFalse(_engine.CanTransition("Closed", "Approved"));
        }

        [Test]
        public void CanTransition_DeniedToUnderReview_True()
        {
            Assert.IsTrue(_engine.CanTransition("Denied", "UnderReview"));
        }

        [Test]
        public void CanTransition_NullInputs_False()
        {
            Assert.IsFalse(_engine.CanTransition(null, null));
        }

        [Test]
        public void CanTransition_EmptyInputs_False()
        {
            Assert.IsFalse(_engine.CanTransition("", ""));
        }

        [Test]
        public void CanTransition_InvalidStatus_False()
        {
            Assert.IsFalse(_engine.CanTransition("Invalid", "Approved"));
        }

        [Test]
        public void GetAllowedTransitions_Submitted_ReturnsTwoOptions()
        {
            var result = _engine.GetAllowedTransitions("Submitted");
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void GetAllowedTransitions_UnderReview_ReturnsThreeOptions()
        {
            var result = _engine.GetAllowedTransitions("UnderReview");
            Assert.AreEqual(3, result.Count);
        }

        [Test]
        public void GetAllowedTransitions_Closed_ReturnsOneOption()
        {
            var result = _engine.GetAllowedTransitions("Closed");
            Assert.AreEqual(1, result.Count);
            Assert.Contains("Reopened", result);
        }

        [Test]
        public void GetAllowedTransitions_NullStatus_ReturnsEmpty()
        {
            var result = _engine.GetAllowedTransitions(null);
            Assert.IsEmpty(result);
        }

        [Test]
        public void GenerateClaimNumber_ValidFormat()
        {
            string claimNumber = _engine.GenerateClaimNumber();
            Assert.That(claimNumber, Does.StartWith("WC-"));
            Assert.AreEqual(12, claimNumber.Length);
        }

        [Test]
        public void GenerateClaimNumber_ContainsCurrentYear()
        {
            string claimNumber = _engine.GenerateClaimNumber();
            Assert.That(claimNumber, Does.Contain(DateTime.Now.Year.ToString()));
        }

        [Test]
        public void RequiresAdjusterAssignment_UnderReview_True()
        {
            Assert.IsTrue(_engine.RequiresAdjusterAssignment("UnderReview"));
        }

        [Test]
        public void RequiresAdjusterAssignment_Submitted_False()
        {
            Assert.IsFalse(_engine.RequiresAdjusterAssignment("Submitted"));
        }

        [Test]
        public void RequiresDenialReason_Denied_True()
        {
            Assert.IsTrue(_engine.RequiresDenialReason("Denied"));
        }

        [Test]
        public void RequiresMedicalReview_MedicalReview_True()
        {
            Assert.IsTrue(_engine.RequiresMedicalReview("MedicalReview"));
        }

        [Test]
        public void IsTerminalStatus_Closed_True()
        {
            Assert.IsTrue(_engine.IsTerminalStatus("Closed"));
        }

        [Test]
        public void IsTerminalStatus_Approved_False()
        {
            Assert.IsFalse(_engine.IsTerminalStatus("Approved"));
        }

        [Test]
        public void GetStatusDisplayClass_ReturnsCorrectClasses()
        {
            Assert.AreEqual("label-info", _engine.GetStatusDisplayClass("Submitted"));
            Assert.AreEqual("label-warning", _engine.GetStatusDisplayClass("UnderReview"));
            Assert.AreEqual("label-primary", _engine.GetStatusDisplayClass("MedicalReview"));
            Assert.AreEqual("label-success", _engine.GetStatusDisplayClass("Approved"));
            Assert.AreEqual("label-danger", _engine.GetStatusDisplayClass("Denied"));
            Assert.AreEqual("label-default", _engine.GetStatusDisplayClass("Closed"));
            Assert.AreEqual("label-warning", _engine.GetStatusDisplayClass("Reopened"));
        }
    }
}
