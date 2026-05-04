using NUnit.Framework;
using AnyStateClaimsPortal.Web.Controllers;
using NUnit.Framework.Legacy;


namespace AnyStateClaimsPortal.Tests.Controllers
{
    [TestFixture]
    public class ControllerInstantiationTests
    {
        [Test]
        public void HomeController_CanInstantiate()
        {
            ClassicAssert.IsNotNull(new HomeController());
        }

        [Test]
        public void ClaimsController_CanInstantiate()
        {
            ClassicAssert.IsNotNull(new ClaimsController());
        }

        [Test]
        public void ReportsController_CanInstantiate()
        {
            ClassicAssert.IsNotNull(new ReportsController());
        }

        [Test]
        public void PaymentsController_CanInstantiate()
        {
            ClassicAssert.IsNotNull(new PaymentsController());
        }

        [Test]
        public void MedicalController_CanInstantiate()
        {
            ClassicAssert.IsNotNull(new MedicalController());
        }

        [Test]
        public void AdminController_CanInstantiate()
        {
            ClassicAssert.IsNotNull(new AdminController());
        }
    }
}
