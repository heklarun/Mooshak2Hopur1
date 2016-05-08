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
        [HttpGet]
        public ActionResult AdminIndex()
        {
            ViewBag.users = userService.GetAllUsers();
            ViewBag.courses = courseService.GetAllCourses();
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

        [HttpGet]
        public ActionResult EditUser(int? userID)
        {
            if(userID == null)
            {
                return RedirectToAction("AdminIndex");
            }
            else
            {
                UsersViewModels user = userService.GetUserByUserID(userID);
                return View(user);

            }
        }
        [HttpPost]
        public ActionResult EditUser(UsersViewModels user)
        {
            userService.EditUser(user);
            return RedirectToAction("AdminIndex");
        }


        [HttpGet]
        public ActionResult EditCourse(int? courseID)
        {
            if (courseID == null)
            {
                return RedirectToAction("AdminIndex");
            }
            else
            {
                CoursesViewModels course = courseService.GetCourseByID(courseID);
                return View(course);

            }
        }
        [HttpPost]
        public ActionResult EditCourse(CoursesViewModels course)
        {
            courseService.EditCourse(course);
            return RedirectToAction("AdminIndex");
        }

        [HttpGet]
        public ActionResult TeacherGroup(int? courseID)
        {
            if (courseID == null)
            {
                return RedirectToAction("AdminIndex");
            }
            else
            {
                List<UsersViewModels> users = userService.GetAllTeachers(courseID);
                ViewBag.users = users; 
                return View(users);

            }
        }
        [HttpPost]
        public ActionResult TeacherGroup(int? courseID, List<UsersViewModels> users)
        {
            if(courseID != null)
            {
               userService.AddTeachersToGroup(courseID, users);
            }
            return RedirectToAction("AdminIndex");
        }

    }
}