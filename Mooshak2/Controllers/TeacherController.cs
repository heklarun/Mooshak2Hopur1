using System;
using System.Collections.Generic;
using System.Linq;
using Mooshak2.DAL;
using Mooshak2.Models;
using SecurityWebAppTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mooshak2.Services;

namespace Mooshak2.Controllers
{
    public class TeacherController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserService userService = new UserService();
        ProjectService projectService = new ProjectService();
        CourseService courseService = new CourseService();

        // GET: Teacher
        public ActionResult TeacherIndex()
        {
            return View("TeacherIndex");
        }

        public ActionResult TeacherCoursesAvailable()
        {
            return View("TeacherCoursesAvailable");
        }

        [HttpPost]
        public ActionResult CreateNewProject()
        {
            ViewBag.course = courseService.GetAllCourses();
            ViewBag.projects = projectService.GetAllProjects();

            return View();
        }

        [HttpGet]
        public ActionResult EditProject2(ProjectViewModels project)
        {
            projectService.EditProject(project);

            return RedirectToAction("TeacherIndex");
        }

        [HttpPost]
        public ActionResult EditProject(ProjectViewModels project)
        {
            projectService.EditProject(project);

            return RedirectToAction("TeacherIndex");
        }
    }
}