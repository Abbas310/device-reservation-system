using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using Vanrise_Web.Data;

namespace Vanrise_Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository _repo = new UserRepository();

        
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string username, string password)
        {
            var user = _repo.GetByUsername(username);

            
            if (user != null && Crypto.VerifyHashedPassword(user.PasswordHash, password))
            {
                
                var ticket = new FormsAuthenticationTicket(1, user.Username, DateTime.Now, DateTime.Now.AddDays(1), true, user.Role);
                string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(cookie);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.LoginError = "Invalid username or password.";
            return View();
        }

        
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(string regUsername, string regPassword, string role)
        {
           
            string hash = Crypto.HashPassword(regPassword);
            string safeRole = role == "Editor" ? "Editor" : "ReadOnly";

            bool success = _repo.Add(regUsername, hash, safeRole);

            if (success)
            {
                ViewBag.RegSuccess = "Account created! You can now log in.";
            }
            else
            {
                ViewBag.RegError = "Username already exists.";
            }

            return View("Login"); 
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}