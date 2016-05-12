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
using System.IO;

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
        [Authorize]
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
            ProjectViewModels project = new ProjectViewModels();
            project.openDate = DateTime.Today.ToString("dd.MM.yyyy");
            project.closeDate = DateTime.Today.AddDays(14).ToString("dd.MM.yyyy");
            return View(project);
        }

        [HttpPost]
        public ActionResult CreateNewProject(ProjectViewModels project)
        {
            int projectID = projectService.CreateNewProject(project);
            if(projectID > 0)
            {
                return RedirectToAction("Project", "Teacher", new { projectID = projectID });
            }
            else
            {
                return RedirectToAction("TeacherIndex");
            }
        }

        [HttpGet]
        public ActionResult EditProject(int? projectID)
        {
            ApplicationUser appUser = man.GetUser(User.Identity.Name);
            List<CoursesViewModels> courses = courseService.GetTeacherCourses(appUser.Id);
            ViewBag.courses = courses;
            ProjectViewModels project = projectService.GetProjectByID(projectID);
            project.openDate = project.open.ToString("dd.MM.yyyy");
            project.closeDate = project.close.ToString("dd.MM.yyyy");
            return View(project);
        }

        [HttpPost]
        public ActionResult EditProject(ProjectViewModels project)
        {
            projectService.EditProject(project);

            return RedirectToAction("TeacherIndex");
        }

        [HttpGet]
        public ActionResult Project(int? projectID)
        {
            if(projectID != null)
            {
                ProjectViewModels pro = projectService.GetProjectByID(projectID);
                List<UsersViewModels> students = courseService.GetStudentsInCourse(pro.courseID);
                ViewBag.students = students;
                return View(pro);
            }
            else
            {
                return RedirectToAction("TeacherIndex");
            }
        }

        [HttpGet] 
        public ActionResult AddSubProject(int? projectID)
        {
            if(projectID != null)
            {
                SubProjectsViewModels subProject = new SubProjectsViewModels();
                subProject.projectID = (int)projectID;
                ProjectViewModels pro = projectService.GetProjectByID(projectID);
                ViewBag.project = pro;
                return View(subProject);
            }
            else
            {
                return RedirectToAction("TeacherIndex");
            }
            
        }
        [HttpPost]
        public ActionResult AddSubProject(SubProjectsViewModels sub)
        {
            projectService.AddSubProject(sub);
            return RedirectToAction("Project", "Teacher", new { projectID = sub.projectID });

        }

        [HttpGet]
        public ActionResult DownloadInputFile(int subProjectID)
        {
            SubProjectsViewModels sub = projectService.DownloadInputFile(subProjectID);
            MemoryStream ms = new MemoryStream(sub.inputFileBytes);

            Response.ContentType = sub.inputContentType;
            Response.AddHeader("content-disposition", "attachment;filename="+sub.inputFileName);
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(sub.inputFileBytes, 0, sub.inputFileBytes.Length);
            Response.OutputStream.Flush();
            Response.End();

            return new FileStreamResult(Response.OutputStream, sub.inputContentType);
           
        }

        [HttpGet]
        public ActionResult DownloadInputFileInline(int subProjectID)
        {
            SubProjectsViewModels sub = projectService.DownloadInputFile(subProjectID);
            MemoryStream ms = new MemoryStream(sub.inputFileBytes);

            Response.ContentType = sub.inputContentType;
            Response.AddHeader("content-disposition", "inline;filename=" + sub.inputFileName);
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(sub.inputFileBytes, 0, sub.inputFileBytes.Length);
            Response.OutputStream.Flush();
            Response.End();

            return new FileStreamResult(Response.OutputStream, sub.inputContentType);

        }

        [HttpGet]
        public ActionResult DownloadOutputFile(int subProjectID)
        {
            SubProjectsViewModels sub = projectService.DownloadInputFile(subProjectID);
            MemoryStream ms = new MemoryStream(sub.inputFileBytes);

            Response.ContentType = sub.inputContentType;
            Response.AddHeader("content-disposition", "attachment;filename=" + sub.inputFileName);
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(sub.inputFileBytes, 0, sub.inputFileBytes.Length);
            Response.OutputStream.Flush();
            Response.End();

            return new FileStreamResult(Response.OutputStream, sub.inputContentType);

        }

        [HttpGet]
        public ActionResult DownloadOutputFileInline(int subProjectID)
        {
            SubProjectsViewModels sub = projectService.DownloadInputFile(subProjectID);
            MemoryStream ms = new MemoryStream(sub.inputFileBytes);

            Response.ContentType = sub.inputContentType;
            Response.AddHeader("content-disposition", "inline;filename=" + sub.inputFileName);
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(sub.inputFileBytes, 0, sub.inputFileBytes.Length);
            Response.OutputStream.Flush();
            Response.End();

            return new FileStreamResult(Response.OutputStream, sub.inputContentType);

        }
        [HttpGet]
        public ActionResult DeleteSubProject(int? subProjectID)
        {
            SubProjectsViewModels sub = projectService.GetSubProjectByID(subProjectID);
            return View(sub);
        }
        [HttpPost]
        public ActionResult DeleteSubProject(SubProjectsViewModels subProject)
        {
            projectService.DeleteSubProject(subProject.subProjectID);
            return RedirectToAction("Project", "Teacher", new { projectID = subProject.projectID });
        }

        [HttpGet]
        public ActionResult EditSubProject(int? subProjectID)
        {
            if (subProjectID != null)
            {
                SubProjectsViewModels sub = projectService.GetSubProjectByID(subProjectID);
                return View(sub);
            }
            else
            {
                return RedirectToAction("TeacherIndex");
            }
        }

        [HttpPost]
        public ActionResult EditSubProject(SubProjectsViewModels sub)
        {
            projectService.EditSubProject(sub);
            return RedirectToAction("Project", "Teacher", new { projectID = sub.projectID });
        }

        public ActionResult ViewStudentResponses(string value, int? projectID)
        {
            ProjectViewModels pro = projectService.GetStudentProjectByID(projectID, value);
            ViewBag.project = pro;
            List<ResponseViewModels> responses = projectService.GetStudentResponses(value, projectID);
            ViewBag.responses = responses;
            return PartialView("StudentResponsePartial");
        }

        [HttpGet]
        public ActionResult DownloadFile(int partResponseID)
        {
            SubProjectsViewModels sub = projectService.DownloadPartResponseFile(partResponseID);
            MemoryStream ms = new MemoryStream(sub.inputFileBytes);

            Response.ContentType = sub.inputContentType;
            Response.AddHeader("content-disposition", "attachment;filename=" + sub.inputFileName);
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(sub.inputFileBytes, 0, sub.inputFileBytes.Length);
            Response.OutputStream.Flush();
            Response.End();

            return new FileStreamResult(Response.OutputStream, sub.inputContentType);

        }

        [HttpGet]
        public ActionResult DownloadFileInline(int partResponseID)
        {
            SubProjectsViewModels sub = projectService.DownloadPartResponseFile(partResponseID);
            MemoryStream ms = new MemoryStream(sub.inputFileBytes);

            Response.ContentType = sub.inputContentType;
            Response.AddHeader("content-disposition", "inline;filename=" + sub.inputFileName);
            Response.Buffer = true;
            Response.Clear();
            Response.OutputStream.Write(sub.inputFileBytes, 0, sub.inputFileBytes.Length);
            Response.OutputStream.Flush();
            Response.End();

            return new FileStreamResult(Response.OutputStream, sub.inputContentType);

        }

    }
}