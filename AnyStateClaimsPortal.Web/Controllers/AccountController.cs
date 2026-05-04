using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var db = new AnyStateClaimsContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == model.Username && u.IsActive);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View(model);
                }

                int m;
                int maxAttempts = int.TryParse(ConfigurationManager.AppSettings["MaxLoginAttempts"], out m) ? m : 5;
                if (user.IsLocked || user.FailedLoginAttempts >= maxAttempts)
                {
                    ModelState.AddModelError("", "Account is locked. Contact an administrator.");
                    return View(model);
                }

                // Demo: accept any password for seeded users
                FormsAuthentication.SetAuthCookie(user.Username, false);
                user.LastLoginDate = DateTime.Now;
                user.FailedLoginAttempts = 0;
                db.SaveChanges();

                Session["UserRole"] = user.Role;
                Session["UserFullName"] = user.FullName;
                Session["UserId"] = user.UserId;
                Session["UserAgencyId"] = user.AgencyId;

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
        }

        private string ComputeHash(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
