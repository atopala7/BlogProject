using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        BlogManager man = new BlogManager();

        // GET: Home/Index
        // Displays the splash page
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Home/ListBloggers
        // Lists all the bloggers, with links to their blogs
        [AllowAnonymous]
        public ActionResult ListBloggers()
        {
            var allusers = man.getBloggers();

            return View(allusers);
        }


        // For testing purposes
        public ActionResult ViewClaims()
        {
            ViewBag.ClaimsIdentity = Thread.CurrentPrincipal.Identity;

            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [Authorize]
        public ActionResult Contact()
        {
            var identity = User as ClaimsPrincipal;

            if (identity.HasClaim("http://http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "blogger"))
            {
                ViewBag.Message = "Your contact page.";

                return View();

            }
            return RedirectToAction("Login", "Account");
        }
    }
}