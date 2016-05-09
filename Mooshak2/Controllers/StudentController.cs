using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Mooshak2.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult StudentIndex()
        {
            return View("StudentIndex");
        }

        public ActionResult StudentCoursesAvailable()
        {
            return View("StudentCoursesAvailable");
        }
        public ActionResult StudentAnswerView()
        {
            return View("StudentAnswerView");
        }

    }
}