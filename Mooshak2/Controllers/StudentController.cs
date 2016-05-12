using Microsoft.AspNet.Identity;
using Mooshak2.DAL;
using Mooshak2.Models;
using Mooshak2.Services;
using SecurityWebAppTest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Mooshak2.Controllers
{
    public class StudentController : BaseController
    {
        private IdentityManager man = new IdentityManager();
        private CourseService courseService = new CourseService();
        private ProjectService projectService = new ProjectService();
        // GET: Student
        [Authorize]
        public ActionResult StudentIndex()
        {
            ApplicationUser appUser = man.GetUser(User.Identity.Name);
            List<CoursesViewModels> courses = courseService.GetStudentCourses(appUser.Id);
            if (courses.Count() > 0)
            {
                return RedirectToAction("StudentCourse", "Student", new { courseID = courses[0].courseID });
            }
            else
            {
                return View("StudentIndex");
            }
        }

      //  public ActionResult StudentCoursesAvailable()
        //{
          //  return View("StudentCoursesAvailable");
        //}

        public ActionResult StudentAnswerView()
        {
            return View("StudentAnswerView");
        }

        public ActionResult StudentCoursesAvailable()
        {
            string username = User.Identity.GetUserName(); //ná í notendanafn

            CourseService service = new CourseService();

            var model = service.GetAllCourses();

            return View(model);
        }

        [HttpGet]
        public ActionResult StudentCourse(int? courseID)
        {
            CoursesViewModels courseInfo = courseService.GetStudentCourseByID(courseID);
            ViewBag.courseInfo = courseInfo;
            return View(courseInfo);
        }

        [HttpGet]
        public ActionResult StudentProject(int? projectID)
        {
            if (projectID != null)
            {
                ApplicationUser appUser = man.GetUser(User.Identity.Name);
                ProjectViewModels pro = projectService.GetStudentProjectByID(projectID, appUser.Id);
                List<ResponseViewModels> responses = projectService.GetStudentResponses(appUser.Id, projectID);
                ViewBag.responses = responses;
                return View(pro);
            }
            else
            {
                return RedirectToAction("StudentIndex");
            }

        }

        public ActionResult SubmitSubProject(int? value)
        {
            if(value != null)
            {
                SubProjectsViewModels sub = projectService.GetSubProjectByID(value);
                ApplicationUser appUser = man.GetUser(User.Identity.Name);
                ProjectViewModels pro = projectService.GetStudentProjectByID(sub.projectID, appUser.Id);
                List<UsersViewModels> students =  courseService.GetStudentsInCourseExceptMe(pro.courseID, appUser.Id);

                PartResponseViewModels response = new PartResponseViewModels();
                response.subProjectID = sub.subProjectID;
                response.subProjectName = sub.subProjectName;
                response.projectID = sub.projectID;
                response.nrGroupMembers = pro.groupMembers != null ? pro.groupMembers.Count: 0;
                List<UsersViewModels> groupMembers = new List<UsersViewModels>();
                for(int i = 0; i < sub.memberCount; i++)
                {
                    UsersViewModels tmp = new UsersViewModels();
                    groupMembers.Add(tmp);
                }
                response.groupMembers = groupMembers;
                response.students = students;
                return PartialView("ProjectPartPartial", response);
            }
            else
            {
                return RedirectToAction("StudentIndex") ;
            }
        }
        [HttpPost]
        public ActionResult SubProjectSubmit(PartResponseViewModels response)
        {
            ApplicationUser appUser = man.GetUser(User.Identity.Name);
            response.userID = appUser.Id;
            projectService.submitSubProject(response);
            return RedirectToAction("StudentProject", "Student", new { projectID = response.projectID });
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