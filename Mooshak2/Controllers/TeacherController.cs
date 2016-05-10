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
    public class TeacherController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserService userService = new UserService();
        ProjectService projectService = new ProjectService();
        CourseService courseService = new CourseService();
        IdentityManager man = new IdentityManager();


        // GET: Teacher
        public ActionResult TeacherIndex()
        {
            ApplicationUser appUser = man.GetUser(User.Identity.Name);
            List<CoursesViewModels> courses = courseService.GetTeacherCourses(appUser.Id);
            if(courses.Count() > 0)
            {
                return RedirectToAction("Course", "Teacher", new { courseID = courses[0].courseID });
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult Course(int? courseID)
        {
            CoursesViewModels courseInfo = courseService.GetCourseByID(courseID);
            ViewBag.courseInfo = courseInfo;
            return View(courseInfo);
        }
        public ActionResult TeacherCoursesAvailable()
        {
            return View("TeacherCoursesAvailable");
        }

        [HttpGet]
        public ActionResult CreateNewProject()
        {
            ApplicationUser appUser = man.GetUser(User.Identity.Name);
            List<CoursesViewModels> courses = courseService.GetTeacherCourses(appUser.Id);
            ViewBag.courses = courses;
            /*ProjectViewModels project = new ProjectViewModels();
            project.courses = courses;
            return View(project);*/
            return View();
        }

        [HttpPost]
        public ActionResult CreateNewProject(ProjectViewModels project)
        {
            projectService.CreateNewProject(project);
            return RedirectToAction("TeacherIndex");
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