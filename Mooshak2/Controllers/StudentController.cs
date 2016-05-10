using Microsoft.AspNet.Identity;
using Mooshak2.DAL;
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
        [Authorize(Roles = "Student")]
        public ActionResult StudentIndex()
        {
            return View("StudentIndex");
        }

      //  public ActionResult StudentCoursesAvailable()
        //{
          //  return View("StudentCoursesAvailable");
        //}

        public ActionResult StudentAnswerView()
        {
            return View("StudentAnswerView");
        }

        public ActionResult StudentCoursesAvailable()
        {
            string username = User.Identity.GetUserName(); //ná í notendanafn

            CourseService service = new CourseService();

            var model = service.GetAllCourses();

            return View(model);
        }
    }
}