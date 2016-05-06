using Mooshak2.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Mooshak2.Controllers
{
    public class CourseController : Controller
    {

        [Authorize(Roles = "Admin")]
        // GET: Course
        public ActionResult Index()
        {
            string username = User.Identity.GetUserName(); //ná í notendanafn

            CourseService service = new CourseService();

            var model = service.GetAllCourses();

            return View(model);
        }
    }
}