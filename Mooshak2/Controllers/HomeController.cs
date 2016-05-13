using SecurityWebAppTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mooshak2.Controllers
{
    public class HomeController : Controller
    {
        private IdentityManager man = new IdentityManager();
        //if user is in role Admin it redirects to the AdminIndex View
        //if user is in role Teacher it redirects to the TeacherIndex View
        //if user is in role Student it redirects to the StudentIndex View
        //else it redirects to the Login View
        public ActionResult Index()
        {
            if (man.UserNameIsInRole(User.Identity.Name, "Admin"))
            {
                return RedirectToAction("AdminIndex", "Admin");
            }
            else if (man.UserNameIsInRole(User.Identity.Name, "Teacher"))
            {
                return RedirectToAction("TeacherIndex", "Teacher");

                }
            else if(man.UserNameIsInRole(User.Identity.Name, "Student"))
            {
                return RedirectToAction("StudentIndex", "Student");
            }
            else
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "" });
            }
            

            //return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}