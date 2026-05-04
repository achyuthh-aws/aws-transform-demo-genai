using System;
using System.Web.Mvc;
using AnyStateClaimsPortal.Web.DataAccess;

namespace AnyStateClaimsPortal.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                var repo = new ClaimsRepository();
                var dashboard = repo.GetDashboardData();
                return View(dashboard);
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }

        public ActionResult Error()
        {
            return Content("Error page reached");
        }
    }
}
