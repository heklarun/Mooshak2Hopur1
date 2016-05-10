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
        ApplicationDbContext db = new ApplicationDbContext();
        UserService userService = new UserService();
        CourseService courseService = new CourseService();
        // GET: Admin
        [HttpGet]
        public ActionResult AdminIndex(int? courseID)
        {
            List<CoursesViewModels> courses = courseService.GetAllCourses();
            ViewBag.courses = courses;
            return View();
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
          

            return RedirectToAction("AdminIndex");
        }
        [HttpPost]
        public ActionResult CreateNewCourse(CoursesViewModels course)
        {
            courseService.CreateNewCourse(course);
            return RedirectToAction("AdminIndex");
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
            return RedirectToAction("AdminIndex", "Admin", new { courseID = course.courseID });
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
            return RedirectToAction("AdminIndex", "Admin", new { courseID = courseID });
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
            return RedirectToAction("AdminIndex", "Admin", new { courseID = courseID });
        }

        
        [HttpGet]
        public ActionResult DeleteCourse(int? courseID)
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
            //courseService.DeleteCourse(courseID);
        }
        [HttpPost]
        public ActionResult DeleteCourse(CoursesViewModels course)
        {
            courseService.DeleteCourse(course.courseID);
            return RedirectToAction("AdminIndex");
        }

        //Það sem patti gerði er hér fyrir neðan
        [Authorize(Roles = "Admin")]
        // GET: Course
        public ActionResult Index()
        {
           // string username = User.Identity.GetUserName(); //ná í notendanafn

            CourseService service = new CourseService();

            var model = service.GetAllCourses();

            return View(model);
        }
    }
}