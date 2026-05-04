using System;
using NUnit.Framework;
using AnyStateClaimsPortal.Web.BusinessLogic;
using AnyStateClaimsPortal.Web.Models;
using NUnit.Framework.Legacy;


namespace AnyStateClaimsPortal.Tests.BusinessLogic
{
    [TestFixture]
    public class ValidationServiceTests
    {
        private ValidationService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new ValidationService();
        }

        [Test]
        public void ValidateClaim_FutureInjuryDate_ReturnsError()
        {
            var model = new ClaimViewModel { InjuryDate = DateTime.Today.AddDays(1), InjuryDescription = "A valid description that is long enough" };
            var errors = _service.ValidateClaimSubmission(model);
            Assert.That(errors, Has.Some.Contains("future"));
        }

        [Test]
        public void ValidateClaim_OldInjuryDate_ReturnsError()
        {
            var model = new ClaimViewModel { InjuryDate = DateTime.Today.AddYears(-2), InjuryDescription = "A valid description that is long enough" };
            var errors = _service.ValidateClaimSubmission(model);
            Assert.That(errors, Has.Some.Contains("1 year"));
        }

        [Test]
        public void ValidateClaim_ShortDescription_ReturnsError()
        {
            var model = new ClaimViewModel { InjuryDate = DateTime.Today, InjuryDescription = "Short" };
            var errors = _service.ValidateClaimSubmission(model);
            Assert.That(errors, Has.Some.Contains("20 characters"));
        }

        [Test]
        public void ValidateClaim_ValidClaim_ReturnsNoErrors()
        {
            var model = new ClaimViewModel { InjuryDate = DateTime.Today, InjuryDescription = "This is a valid description with enough characters" };
            var errors = _service.ValidateClaimSubmission(model);
            ClassicAssert.IsEmpty(errors);
        }

        [Test]
        public void ValidateTransition_InvalidTransition_ReturnsError()
        {
            var model = new ClaimViewModel();
            var errors = _service.ValidateStatusTransition("Submitted", "Approved", model);
            Assert.That(errors, Has.Some.Contains("Cannot transition"));
        }

        [Test]
        public void ValidateTransition_DeniedWithoutReason_ReturnsError()
        {
            var model = new ClaimViewModel { DenialReason = null };
            var errors = _service.ValidateStatusTransition("UnderReview", "Denied", model);
            Assert.That(errors, Has.Some.Contains("denial reason"));
        }

        [Test]
        public void ValidatePayment_ZeroAmount_ReturnsError()
        {
            var model = new PaymentViewModel { Amount = 0 };
            var errors = _service.ValidatePayment(model);
            Assert.That(errors, Has.Some.Contains("greater than zero"));
        }

        [Test]
        public void ValidatePayment_NegativeAmount_ReturnsError()
        {
            var model = new PaymentViewModel { Amount = -100 };
            var errors = _service.ValidatePayment(model);
            Assert.That(errors, Has.Some.Contains("greater than zero"));
        }

        [Test]
        public void ValidatePayment_ValidPayment_ReturnsNoErrors()
        {
            var model = new PaymentViewModel { Amount = 500 };
            var errors = _service.ValidatePayment(model);
            ClassicAssert.IsEmpty(errors);
        }
    }
}
