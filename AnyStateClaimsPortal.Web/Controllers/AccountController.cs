using System;
using System.Linq;
using System.Security.Claims;
using AnyStateClaimsPortal.Web.Models;
using AnyStateClaimsPortal.Web.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


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
                    var claims = new[] { new System.Security.Claims.Claim(ClaimTypes.Name, user.Username) };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).GetAwaiter().GetResult();

                    user.LastLoginDate = DateTime.Now;
                    user.FailedLoginAttempts = 0;
                    db.SaveChanges();

                    HttpContext.Session.SetString("UserRole", user.Role ?? string.Empty);
                    HttpContext.Session.SetString("UserFullName", user.FullName ?? string.Empty);
                    HttpContext.Session.SetString("UserId", user.UserId.ToString());
                    HttpContext.Session.SetString("UserAgencyId", user.AgencyId != null ? user.AgencyId.ToString() : string.Empty);

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
                HttpContext.Session.Clear();
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).GetAwaiter().GetResult();
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                return Content("ERROR: " + ex.ToString());
            }
        }
    }
}
