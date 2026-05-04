using System;
using System.Web.Mvc;
using AnyStateClaimsPortal.Web.DataAccess;

namespace AnyStateClaimsPortal.Web.Controllers
{
    [Authorize]
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
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            return View();
        }
    }
}
