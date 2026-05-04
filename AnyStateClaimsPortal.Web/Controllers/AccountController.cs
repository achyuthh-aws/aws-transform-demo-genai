using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using AnyStateClaimsPortal.Web.Models;
using AnyStateClaimsPortal.Web.Models.Entities;

namespace AnyStateClaimsPortal.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                var model = new LoginViewModel();
                model.ReturnUrl = returnUrl;
                return View(model);
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                using (var db = new AnyStateClaimsContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.Username == model.Username && u.IsActive);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                        return View(model);
                    }

                    if (user.IsLocked)
                    {
                        ModelState.AddModelError("", "Account is locked. Contact an administrator.");
                        return View(model);
                    }

                    // Demo: accept any password
                    FormsAuthentication.SetAuthCookie(user.Username, false);
                    user.LastLoginDate = DateTime.Now;
                    user.FailedLoginAttempts = 0;
                    db.SaveChanges();

                    Session["UserRole"] = user.Role;
                    Session["UserFullName"] = user.FullName;
                    Session["UserId"] = user.UserId;
                    Session["UserAgencyId"] = user.AgencyId;

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }

        public ActionResult Logout()
        {
            try
            {
                Session.Clear();
                FormsAuthentication.SignOut();
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }
    }
}
