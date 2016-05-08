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
        public ActionResult AdminIndex()
        {

            //Test get all users from correct asp.net tables
            UserService service = new UserService();
            var tempUsers = service.GetAllUsers();
            /*
            ViewBag.users = userService.GetAllUsers();
            ViewBag.courses = courseService.GetAllCourses();*/
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
          /*  IdentityManager manager = new IdentityManager();
            ApplicationUser newUser = new ApplicationUser
            {
                UserName = user.username,
                Email = "test@test.is",
                lastName = user.lastName,
                firstName = user.firstName
            };
            bool result = manager.CreateUser(newUser, user.password);
            if (result)
            {
                if (user.isAdmin == true)
                {
                    manager.AddUserToRole(newUser.Id, "Admin");
                }
                if (user.isTeacher == true)
                {
                    manager.AddUserToRole(newUser.Id, "Teacher");
                }
                if (user.isStudent == true)
                {
                    manager.AddUserToRole(newUser.Id, "Student");
                }
            }
            else
            {

            }*/

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
        public ActionResult StudentGroup(int? courseID, List<UsersViewModels> users)
        {
            if(courseID != null)
            {
               userService.AddStudentsToGroup(courseID, users);
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
                List<UsersViewModels> users = userService.GetAllStudents(courseID);
                ViewBag.users = users;
                return View(users);

            }
        }

    }
}