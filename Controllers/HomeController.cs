using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Vanrise_Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult Clients()
        {
            return View();
        }
        public ActionResult PhoneNumbers()
        {
            return View();
        }
        public ActionResult Reservations()
        {
            return View();
        }
        public ActionResult ClientsReport()
        {
            return View();
        }

        public ActionResult DevicesReport()
        {
            return View();
        }

       

    }
}
