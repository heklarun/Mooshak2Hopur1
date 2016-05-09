using Mooshak2.DAL;
using Mooshak2.Models;
using SecurityWebAppTest.Models;
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
        public ActionResult AdminIndex(int? courseID)
        {

            //Test get all users from correct asp.net tables
            //var tempUsers = service.GetAllUsers();
            if (courseID != null)
            {
                ViewBag.users = userService.GetAllUsers();
                List<Courses> allCourses = courseService.GetAllCourses();
                ViewBag.allCourses = allCourses;
                CoursesViewModels course = courseService.GetCourseByID(courseID);
                ViewBag.course = course;

            }
          /*  List<UsersViewModels> allTeachers = courseService.GetTeachersInCourse(courseID);
            ViewBag.allTeachers = allTeachers;
            List<UsersViewModels> allStudents = courseService.GetStudentsInCourse(courseID);
            ViewBag.allStudents = allStudents;*/
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
        public ActionResult EditUser(string username)
        {
            if(username == null)
            {
                return RedirectToAction("AdminIndex");
            }
            else
            {
                UsersViewModels user = userService.GetUserById(username);
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
                CoursesViewModels course = courseService.GetCourseByID(courseID);
                ViewBag.course = course;
                List<UsersViewModels> users = userService.GetAllTeachers(courseID);
                ViewBag.users = users; 
                return View(users);

            }
        }

        [HttpPost]
        public ActionResult TeacherGroup(int? courseID, List<UsersViewModels> users)
        {
            if (courseID != null)
            {
                userService.AddTeachersToGroup(courseID, users);
            }
            return RedirectToAction("AdminIndex");
        }

        
        [HttpGet]
        public ActionResult StudentGroup(int? courseID)
        {
            if (courseID == null)
            {
                return RedirectToAction("AdminIndex");
            }
            else
            {
                CoursesViewModels course = courseService.GetCourseByID(courseID);
                ViewBag.course = course;
                List<UsersViewModels> users = userService.GetAllStudents(courseID);
                ViewBag.users = users;
                return View(users);

            }
        }

        [HttpPost]
        public ActionResult StudentGroup(int? courseID, List<UsersViewModels> users)
        {
            if (courseID != null)
            {
                userService.AddStudentsToGroup(courseID, users);
            }
            return RedirectToAction("AdminIndex");
        }

    }
}