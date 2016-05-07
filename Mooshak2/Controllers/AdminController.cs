using Mooshak2.DAL;
using Mooshak2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mooshak2.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserService userService = new UserService();
        CourseService courseService = new CourseService();
        // GET: Admin
        public ActionResult AdminIndex()
        {
            return View("AdminIndex");
        }

        [HttpGet]
        public ActionResult CreateNewCourse()
        {
            return View("CreateNewCourse");
        }



        [HttpGet]
        public ActionResult CreateNewUser()
        {
            return View("CreateNewUser");
        }
        [HttpPost]
        public ActionResult CreateNewUser(UsersViewModels user)
        {
            userService.CreateNewUser(user);
            return RedirectToAction("CreateNewUser");
        }
        [HttpPost]
        public ActionResult CreateNewCourse(CoursesViewModels course)
        {
            courseService.CreateNewCourse(course);
            return RedirectToAction("CreateNewCourse");
        }
  
    }
}