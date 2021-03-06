﻿using Microsoft.AspNet.Identity;
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

        // creates an appuser and creates a list of courses for appuser ID by calling the GetStudentCourses function
        //for that ID
        //if the number of courses is larger then 0 it redirects to the StudentCourse View
        //otherwise it displays the StudentIndex View


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
        
        //Gets the course Information by calling the GetStudentCourseBtID function in CourseService
        //for a particular courseID and displays the Studentcourse view for a particular course
        [HttpGet]
        public ActionResult StudentCourse(int? courseID)
        {
            CoursesViewModels courseInfo = courseService.GetStudentCourseByID(courseID);
            ViewBag.courseInfo = courseInfo;
            return View(courseInfo);
        }
        //If the projectID is not null it calls the GetStudentProjectByID function in ProjectService
        //for a particular user
        //and gets  list of responses by calling the GetStudentResponses function in ProjectService
        //for a particular user in a particular project
        //then displays the StudentProject View
        //if the projectID is null it redirects to the StudentIndex view
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
        //if the value is not null it calls the GetSubProjectByID function in Projectservice
        //it creates an appUser and calls GetStudentProjectByID for that user in ProjectService
        //it creates að list of students and calls the GetStudentsInCourseExceptMe function in CourseService
        //Creates a response and gets the subprojectID, subProjectName, projecgtID and groupmembers if they are allowed
        //and alse gets students except the user itself
        //then it returns the ProjectPartPartial View
        //if the value is null then it redirects straight to the StudentIndex view
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

        //it calls the submitSubProject function in Project Service
        //and then it redirects to the StudentProject View
        [HttpPost]
        public ActionResult SubProjectSubmit(PartResponseViewModels response)
        {
            ApplicationUser appUser = man.GetUser(User.Identity.Name);
            response.userID = appUser.Id;

              projectService.submitSubProject(response);
              return RedirectToAction("StudentProject", "Student", new { projectID = response.projectID });
        }

        //calls the DownloadPartResponseFile in ProjectService
        //it lets the user download the file he has submitted
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
        //Does the same as the function DownloadFile but instead of letting you download the file it lets you
        //view it online
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