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
            filterContext.Controller.ViewBag.TeacherCourses = courses;
        }
    }
}