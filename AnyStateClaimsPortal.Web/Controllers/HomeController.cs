using System.Web.Mvc;
using AnyStateClaimsPortal.Web.DataAccess;

namespace AnyStateClaimsPortal.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var repo = new ClaimsRepository();
            var dashboard = repo.GetDashboardData();
            return View(dashboard);
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}
