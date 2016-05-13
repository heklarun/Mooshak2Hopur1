using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mooshak2.DAL;
using Mooshak2.Models;
using SecurityWebAppTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mooshak2.Controllers
{
    
    public class AdminController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        UserService userService = new UserService();
        CourseService courseService = new CourseService();

        //Creates að list of all courses by callinu the GetAllCourses() function in CourseService
        //then Displays the AdminIndex View
        [Authorize]
        [HttpGet]
        public ActionResult AdminIndex(int? courseID)
        {
            List<CoursesViewModels> courses = courseService.GetAllCourses();
            ViewBag.courses = courses;
            return View();
        }
        //Displays the CreateNewCourse View
        [HttpGet]
        public ActionResult CreateNewCourse()
        {
            return View("CreateNewCourse");
        }

        
        /*Birtir 
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
        }*/
        //Calls the CreateNewCourse function in CourseService
        //Redirects to the AdminIndex view when a new course has been created
        [HttpPost]
        public ActionResult CreateNewCourse(CoursesViewModels course)
        {
            courseService.CreateNewCourse(course);
            return RedirectToAction("AdminIndex");
        }
        //If the username is null it redirects straight to the AdminIndex View
        //Otherwise it calls the GetUserById() function and displays the EditUser View for that user
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
        //Uses a built-in function to uppdate the user
        //Then redirects to the AllUsers View
        [HttpPost]
        public async Task<ActionResult> EditUser(UsersViewModels user)
        {
            var userContext = new ApplicationDbContext();
            var userStore = new UserStore<ApplicationUser>(userContext);
            var userManager = new UserManager<ApplicationUser>(userStore);
            var userToUpdate = await userManager.FindByIdAsync(user.userID);
            userToUpdate.Email = user.email;
            userToUpdate.firstName = user.firstName;
            userToUpdate.lastName = user.lastName;
            userToUpdate.UserName = user.username;
            
            var result = await userManager.UpdateAsync(userToUpdate);
            userService.EditUser(user);
            return RedirectToAction("AllUsers");
        }
        //If courseID is null then it redirects straight to the AdminIndex View
        //Otherwise it calls the GetCourseByID()function in CourseService and 
        //displays the EditCourse View for that course 
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
        //Calls the EditCourse function in CourseService
        //then redirects to the AdminIndex View
        [HttpPost]
        public ActionResult EditCourse(CoursesViewModels course)
        {
            courseService.EditCourse(course);
            return RedirectToAction("AdminIndex", "Admin", new { courseID = course.courseID });
        }
        //If the courseID is null it redirects straight to the AdminIndex view
        //Otherwise it calls the GetCourseByID function in CourseService an the GetAllTeachers function
        //in UserService for a particular courseID
        //Then Displays the TeacherGroup view
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
        //if the courseID is not null it calls the AddTeachersToGroup function in UserService
        //and then redirects to the AdminIndex view
        [HttpPost]
        public ActionResult TeacherGroup(int? courseID, List<UsersViewModels> users)
        {
            if (courseID != null)
            {
                userService.AddTeachersToGroup(courseID, users);
            }
            return RedirectToAction("AdminIndex", "Admin", new { courseID = courseID });
        }

        //if the courseID is null it redirects straight to the AdminIndex view
        //Otherwise it calls the GetCourseByID function in CourseService an the GetAllStudents function
        //in UserService for a particular courseID
        //Then Displays the StudentGroup view 
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
        //if the courseID is not null it calls the AddStudentsToGroup function in UserService
        //and then redirects to the AdminIndex view
        [HttpPost]
        public ActionResult StudentGroup(int? courseID, List<UsersViewModels> users)
        {
            if (courseID != null)
            {
                userService.AddStudentsToGroup(courseID, users);
            }
            return RedirectToAction("AdminIndex", "Admin", new { courseID = courseID });
        }

        //if the courseID is null it redirects straight to the AdminIndex view
        //Otherwise it calls GetCourseByID in CourseService for a particular courseID to get a particular course
        //And then displays the DeleteCourse View for that particular course
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
            
        }

        // calls the DeleteCourse funtion in CourseService for a particular courseID
        //then redirects to the AdminIndex view
        [HttpPost]
        public ActionResult DeleteCourse(CoursesViewModels course)
        {
            courseService.DeleteCourse(course.courseID);
            return RedirectToAction("AdminIndex");
        }

       
      /* held þetta fall geri ekkert
        public ActionResult Index()
        {

            CourseService service = new CourseService();

            var model = service.GetAllCourses();

            return View(model);
        }*/

       //creates a list of All users by calling the GetAllUsers function in UserService
       //then in displays the AllUsers View
        [HttpGet]
        public ActionResult AllUsers()
        {
           
            List<UsersViewModels> allUsers = userService.GetAllUsers();
            ViewBag.allUsers = allUsers;
           
            return View("AllUsers");
        }

    }
}