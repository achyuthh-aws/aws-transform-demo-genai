using NUnit.Framework;
using AnyStateClaimsPortal.Web.BusinessLogic;

namespace AnyStateClaimsPortal.Tests.BusinessLogic
{
    [TestFixture]
    public class BenefitCalculationEngineTests
    {
        private BenefitCalculationEngine _engine;
        private const decimal AnnualSalary = 52000m; // $1000/week

        [SetUp]
        public void SetUp()
        {
            _engine = new BenefitCalculationEngine();
        }

        [Test]
        public void Calculate_MedicalInjury_Returns60Percent()
        {
            var result = _engine.CalculateBenefit(AnnualSalary, "Medical", 0, 0);
            Assert.AreEqual(0.60m, result.BenefitRate);
        }

        [Test]
        public void Calculate_TemporaryInjury_Returns66Point67Percent()
        {
            var result = _engine.CalculateBenefit(AnnualSalary, "Temporary", 0, 0);
            Assert.AreEqual(0.6667m, result.BenefitRate);
        }

        [Test]
        public void Calculate_PermanentInjury_Returns70Percent()
        {
            var result = _engine.CalculateBenefit(AnnualSalary, "Permanent", 0, 0);
            Assert.AreEqual(0.70m, result.BenefitRate);
        }

        [Test]
        public void Calculate_FatalInjury_Returns75Percent()
        {
            var result = _engine.CalculateBenefit(AnnualSalary, "Fatal", 0, 0);
            Assert.AreEqual(0.75m, result.BenefitRate);
        }

        [Test]
        public void Calculate_WithDependents_AddsSupplement()
        {
            var result = _engine.CalculateBenefit(AnnualSalary, "Medical", 2, 0);
            Assert.AreEqual(0.04m, result.DependentSupplement);
        }

        [Test]
        public void Calculate_MaxDependentSupplement_CapsAt10Percent()
        {
            var result = _engine.CalculateBenefit(AnnualSalary, "Medical", 6, 0);
            Assert.AreEqual(0.10m, result.DependentSupplement);
        }

        [Test]
        public void Calculate_LongevityBonus_5Years()
        {
            var result = _engine.CalculateBenefit(AnnualSalary, "Medical", 0, 5);
            Assert.AreEqual(0.005m, result.LongevityBonus);
        }

        [Test]
        public void Calculate_LongevityBonus_10Years()
        {
            var result = _engine.CalculateBenefit(AnnualSalary, "Medical", 0, 10);
            Assert.AreEqual(0.010m, result.LongevityBonus);
        }

        [Test]
        public void Calculate_LongevityBonus_20Years()
        {
            var result = _engine.CalculateBenefit(AnnualSalary, "Medical", 0, 20);
            Assert.AreEqual(0.02m, result.LongevityBonus);
        }

        [Test]
        public void Calculate_MaxBenefitCap_Applied()
        {
            // Very high salary should hit the max cap: 1025 * 1.5 = 1537.50
            var result = _engine.CalculateBenefit(520000m, "Fatal", 5, 20);
            Assert.AreEqual(1537.50m, result.WeeklyBenefit);
        }

        [Test]
        public void Calculate_MinBenefitCap_Applied()
        {
            // Very low salary should hit the min cap: 1025 * 0.15 = 153.75
            var result = _engine.CalculateBenefit(5200m, "Medical", 0, 0);
            Assert.AreEqual(153.75m, result.WeeklyBenefit);
        }

        [Test]
        public void Calculate_CombinedRateWithDependentsAndLongevity()
        {
            // Medical 0.60 + 2 deps (0.04) + 10 years (0.01) = 0.65
            var result = _engine.CalculateBenefit(AnnualSalary, "Medical", 2, 10);
            Assert.AreEqual(0.65m, result.BenefitRate);
            Assert.AreEqual(650.00m, result.WeeklyBenefit);
        }
    }
}
