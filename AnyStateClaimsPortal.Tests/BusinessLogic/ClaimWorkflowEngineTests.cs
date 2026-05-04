using System;
using System.Collections.Generic;
using NUnit.Framework;
using AnyStateClaimsPortal.Web.BusinessLogic;
using NUnit.Framework.Legacy;


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
            ClassicAssert.IsTrue(_engine.CanTransition("Submitted", "UnderReview"));
        }

        [Test]
        public void CanTransition_SubmittedToApproved_False()
        {
            ClassicAssert.IsFalse(_engine.CanTransition("Submitted", "Approved"));
        }

        [Test]
        public void CanTransition_UnderReviewToMedicalReview_True()
        {
            ClassicAssert.IsTrue(_engine.CanTransition("UnderReview", "MedicalReview"));
        }

        [Test]
        public void CanTransition_UnderReviewToApproved_True()
        {
            ClassicAssert.IsTrue(_engine.CanTransition("UnderReview", "Approved"));
        }

        [Test]
        public void CanTransition_UnderReviewToDenied_True()
        {
            ClassicAssert.IsTrue(_engine.CanTransition("UnderReview", "Denied"));
        }

        [Test]
        public void CanTransition_MedicalReviewToApproved_True()
        {
            ClassicAssert.IsTrue(_engine.CanTransition("MedicalReview", "Approved"));
        }

        [Test]
        public void CanTransition_ApprovedToClosed_True()
        {
            ClassicAssert.IsTrue(_engine.CanTransition("Approved", "Closed"));
        }

        [Test]
        public void CanTransition_ClosedToReopened_True()
        {
            ClassicAssert.IsTrue(_engine.CanTransition("Closed", "Reopened"));
        }

        [Test]
        public void CanTransition_ReopenedToUnderReview_True()
        {
            ClassicAssert.IsTrue(_engine.CanTransition("Reopened", "UnderReview"));
        }

        [Test]
        public void CanTransition_ClosedToApproved_False()
        {
            ClassicAssert.IsFalse(_engine.CanTransition("Closed", "Approved"));
        }

        [Test]
        public void CanTransition_DeniedToUnderReview_True()
        {
            ClassicAssert.IsTrue(_engine.CanTransition("Denied", "UnderReview"));
        }

        [Test]
        public void CanTransition_NullInputs_False()
        {
            ClassicAssert.IsFalse(_engine.CanTransition(null, null));
        }

        [Test]
        public void CanTransition_EmptyInputs_False()
        {
            ClassicAssert.IsFalse(_engine.CanTransition("", ""));
        }

        [Test]
        public void CanTransition_InvalidStatus_False()
        {
            ClassicAssert.IsFalse(_engine.CanTransition("Invalid", "Approved"));
        }

        [Test]
        public void GetAllowedTransitions_Submitted_ReturnsTwoOptions()
        {
            var result = _engine.GetAllowedTransitions("Submitted");
            ClassicAssert.AreEqual(2, result.Count);
        }

        [Test]
        public void GetAllowedTransitions_UnderReview_ReturnsThreeOptions()
        {
            var result = _engine.GetAllowedTransitions("UnderReview");
            ClassicAssert.AreEqual(3, result.Count);
        }

        [Test]
        public void GetAllowedTransitions_Closed_ReturnsOneOption()
        {
            var result = _engine.GetAllowedTransitions("Closed");
            ClassicAssert.AreEqual(1, result.Count);
            ClassicAssert.Contains("Reopened", result);
        }

        [Test]
        public void GetAllowedTransitions_NullStatus_ReturnsEmpty()
        {
            var result = _engine.GetAllowedTransitions(null);
            ClassicAssert.IsEmpty(result);
        }

        [Test]
        public void GenerateClaimNumber_ValidFormat()
        {
            string claimNumber = _engine.GenerateClaimNumber();
            Assert.That(claimNumber, Does.StartWith("WC-"));
            ClassicAssert.AreEqual(12, claimNumber.Length);
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
            ClassicAssert.IsTrue(_engine.RequiresAdjusterAssignment("UnderReview"));
        }

        [Test]
        public void RequiresAdjusterAssignment_Submitted_False()
        {
            ClassicAssert.IsFalse(_engine.RequiresAdjusterAssignment("Submitted"));
        }

        [Test]
        public void RequiresDenialReason_Denied_True()
        {
            ClassicAssert.IsTrue(_engine.RequiresDenialReason("Denied"));
        }

        [Test]
        public void RequiresMedicalReview_MedicalReview_True()
        {
            ClassicAssert.IsTrue(_engine.RequiresMedicalReview("MedicalReview"));
        }

        [Test]
        public void IsTerminalStatus_Closed_True()
        {
            ClassicAssert.IsTrue(_engine.IsTerminalStatus("Closed"));
        }

        [Test]
        public void IsTerminalStatus_Approved_False()
        {
            ClassicAssert.IsFalse(_engine.IsTerminalStatus("Approved"));
        }

        [Test]
        public void GetStatusDisplayClass_ReturnsCorrectClasses()
        {
            ClassicAssert.AreEqual("label-info", _engine.GetStatusDisplayClass("Submitted"));
            ClassicAssert.AreEqual("label-warning", _engine.GetStatusDisplayClass("UnderReview"));
            ClassicAssert.AreEqual("label-primary", _engine.GetStatusDisplayClass("MedicalReview"));
            ClassicAssert.AreEqual("label-success", _engine.GetStatusDisplayClass("Approved"));
            ClassicAssert.AreEqual("label-danger", _engine.GetStatusDisplayClass("Denied"));
            ClassicAssert.AreEqual("label-default", _engine.GetStatusDisplayClass("Closed"));
            ClassicAssert.AreEqual("label-warning", _engine.GetStatusDisplayClass("Reopened"));
        }
    }
}
