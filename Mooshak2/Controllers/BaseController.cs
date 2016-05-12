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
    public abstract class BaseController : Controller
    {
        private IdentityManager man = new IdentityManager();
        private CourseService courseService = new CourseService();

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ApplicationUser appUser = man.GetUser(User.Identity.Name);
            List<CoursesViewModels> courses = courseService.GetTeacherCourses(appUser.Id);
            List<CoursesViewModels> studentCourses = courseService.GetStudentCourses(appUser.Id);
            filterContext.Controller.ViewBag.TeacherCourses = courses;
            filterContext.Controller.ViewBag.StudentCourses = studentCourses;
            bool isAdmin = man.UserNameIsInRole(appUser.UserName, "Admin");
            bool isTeacher = man.UserNameIsInRole(appUser.UserName, "Teacher");
            bool isStudent = man.UserNameIsInRole(appUser.UserName, "Student");
            var accesses = 0;
            if(isAdmin== true)
            {
                accesses++;
            }
            if (isTeacher == true)
            {
                accesses++;
            }
            if (isStudent == true)
            {
                accesses++;
            }
            filterContext.Controller.ViewBag.isAdmin = isAdmin;
            filterContext.Controller.ViewBag.isTeacher = isTeacher;
            filterContext.Controller.ViewBag.isStudent = isStudent;
            filterContext.Controller.ViewBag.accesses = accesses;

        }
    }
}