using NUnit.Framework;
using AnyStateClaimsPortal.Web.Controllers;

namespace AnyStateClaimsPortal.Tests.Controllers
{
    [TestFixture]
    public class ControllerInstantiationTests
    {
        [Test]
        public void HomeController_CanInstantiate()
        {
            Assert.IsNotNull(new HomeController());
        }

        [Test]
        public void ClaimsController_CanInstantiate()
        {
            Assert.IsNotNull(new ClaimsController());
        }

        [Test]
        public void ReportsController_CanInstantiate()
        {
            Assert.IsNotNull(new ReportsController());
        }

        [Test]
        public void PaymentsController_CanInstantiate()
        {
            Assert.IsNotNull(new PaymentsController());
        }

        [Test]
        public void MedicalController_CanInstantiate()
        {
            Assert.IsNotNull(new MedicalController());
        }

        [Test]
        public void AdminController_CanInstantiate()
        {
            Assert.IsNotNull(new AdminController());
        }
    }
}
