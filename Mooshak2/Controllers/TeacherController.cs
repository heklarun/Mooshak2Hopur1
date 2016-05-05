using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mooshak2.Controllers
{
    public class TeacherController : Controller
    {
        // GET: Teacher
        public ActionResult TeacherIndex()
        {
            return View("TeacherIndex");
        }

        public ActionResult CreateNewProject()
        {
            return View("CreateNewProject");
        }

        public ActionResult TeacherCoursesAvailable()
        {
            return View("TeacherCoursesAvailable");
        }
    }
}