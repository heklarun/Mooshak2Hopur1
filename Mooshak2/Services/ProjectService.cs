using System;
using System.Collections.Generic;
using System.Linq;
using Mooshak2.Models;
using SecurityWebAppTest.Models;
using System.IO;
using System.Globalization;
using Mooshak2.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Configuration;
using System.Diagnostics;
using System.Web.Mvc;

namespace Mooshak2.Services
{
    public class ProjectService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IdentityManager man = new IdentityManager();
        private UserService userService = new UserService();
        

        //creates a Projects list called result, and puts the items in the Projects Model 
        //and returns result
        public List<Projects> GetAllProjects()
        {
            List<Projects> result = (from item in db.Project
                                     select item).ToList();
            return result;
        }

        //creates a newProject and adds all the information needed in that project
        //and saves it to the database
        public int CreateNewProject(ProjectViewModels projectToAdd)
        {

            Projects newProject = new Projects();
            newProject.projectName = projectToAdd.projectName;
            newProject.courseID = projectToAdd.courseID;
            if (projectToAdd.memberCount > 0)
            {
                newProject.memberCount = projectToAdd.memberCount - 1;
            }
            else
            {
                newProject.memberCount = 0;
            }
            DateTime openDate = DateTime.ParseExact(projectToAdd.openDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            DateTime closeDate = DateTime.ParseExact(projectToAdd.closeDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            newProject.open = openDate;
            newProject.close = closeDate;

            db.Project.Add(newProject);
            db.SaveChanges();

            int? maxID = db.Project.Max(u => (int?)u.projectID);

            int projectId = 0;
            if (maxID != null)
            {
                projectId = (int)maxID;
            }

            return projectId;

        }

        // Creates a project that is an item in the Projects model 
        //and joins course in the Course model where the course courseID is the same as the item courseID
        //and where the projectID that is sent into the function and the projectID of the item
        //add all the information needed to the ProjectViewModel project
        //and gets a list of the subprojects in the project and adds all the information needed to the SubprojectViewModels subprojects
        //checks how many responses there are to each subproject 
        // adds the subprojects list to to the project list under subprojects
        public ProjectViewModels GetProjectByID(int? projectID)
        {
            ProjectViewModels project = (from item in db.Project
                                         join course in db.Course on item.courseID equals course.courseID
                                         where item.projectID == projectID
                                         select new ProjectViewModels
                                         {
                                             projectID = item.projectID,
                                             projectName = item.projectName,
                                             courseName = course.courseName,
                                             open = item.open,
                                             close = item.close,
                                             memberCount = item.memberCount + 1,
                                             canHandIn = item.close > DateTime.Now,
                                             courseID = course.courseID
                                         }).SingleOrDefault();
            List<SubProjectsViewModels> subProjects = (from item in db.SubProjects
                                                       where item.projectID == projectID
                                                       select new SubProjectsViewModels
                                                       {
                                                           subProjectID = item.subProjectID,
                                                           projectID = item.projectID,
                                                           subProjectName = item.subProjectName,
                                                           percentage = item.percentage,
                                                           inputFileName = item.inputFileName,
                                                           outputFileName = item.outputFileName
                                                       }).ToList();
            foreach (SubProjectsViewModels sub in subProjects)
            {
                int Count = (from item in db.PartResponse where item.subProjectID == sub.subProjectID select item).Count();
                sub.nrOfResponses = Count;
            }
            project.subProjects = subProjects;


            return project;
        }


        // Creates a project that is an item in the Projects model 
        //and joins course in the Course model where the course courseID is the same as the item courseID
        //and where the projectID that is sent into the function and the projectID of the item
        //add all the information needed to the ProjectViewModel project
        //and gets a list of the subprojects in the project and adds all the information needed to the SubprojectViewModels subprojects
        //checks how many responses there are to each subproject 
        // adds the subprojects list to to the project list under subprojects
        //if it is allowed to handin in a group then it every response for the students
        //then it gets all students in the course that can be picked as groupmembers  
        //and add the ones that are picked to members list and adds them to project.groupMembers
        //then it returns project
        public ProjectViewModels GetStudentProjectByID(int? projectID, string userID)
        {
            ProjectViewModels project = (from item in db.Project
                                         join course in db.Course on item.courseID equals course.courseID
                                         where item.projectID == projectID
                                         select new ProjectViewModels
                                         {
                                             projectID = item.projectID,
                                             projectName = item.projectName,
                                             courseName = course.courseName,
                                             courseID = course.courseID,
                                             open = item.open,
                                             close = item.close,
                                             memberCount = item.memberCount + 1,
                                             canHandIn = item.close > DateTime.Now
                                         }).SingleOrDefault();
            List<SubProjectsViewModels> subProjects = (from item in db.SubProjects
                                                       where item.projectID == projectID
                                                       select new SubProjectsViewModels
                                                       {
                                                           subProjectID = item.subProjectID,
                                                           projectID = item.projectID,
                                                           subProjectName = item.subProjectName,
                                                           percentage = item.percentage,
                                                           inputFileName = item.inputFileName,
                                                           outputFileName = item.outputFileName
                                                       }).ToList();
            foreach (SubProjectsViewModels sub in subProjects)
            {
                int Count = (from item in db.PartResponse where item.subProjectID == sub.subProjectID select item).Count();
                sub.nrOfResponses = Count;
            }
            project.subProjects = subProjects;
            if (project.memberCount > 1)
            {
                ResponseViewModels response = (from item in db.Response
                                               join sRes in db.StudentResponse on item.responseID equals sRes.responseID
                                               where item.projectID == (int)projectID && sRes.userID == userID
                                               select new ResponseViewModels
                                               {
                                                   responseID = item.responseID
                                               }).SingleOrDefault();
                if (response != null)
                {
                    List<StudentsResponseViewModels> students = (from item in db.StudentResponse
                                                                 where item.responseID == response.responseID
                                                                 select new StudentsResponseViewModels
                                                                 {
                                                                     userID = item.userID
                                                                 }).ToList();

                    if (students != null && students.Count > 1)
                    {
                        List<ApplicationUser> users = new List<ApplicationUser>();

                        var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                        List<ApplicationUser> allUsers = um.Users.OrderBy(x => x.firstName).ToList();

                        List<ApplicationUser> members = new List<ApplicationUser>();
                        foreach (StudentsResponseViewModels student in students)
                        {
                            if (student.userID != userID)
                            {
                                foreach (ApplicationUser tmp in allUsers)
                                {
                                    if (tmp.Id == student.userID)
                                    {
                                        members.Add(tmp);
                                    }
                                }
                            }
                        }
                        project.groupMembers = members;

                    }
                }

            }

            return project;
        }

        //gets item from Project model where the item projectID is the same as 
        //the projectID sent into the funtion and put it into projectToEdit
        //and gets all the information in the project and puts it into projectToEdit
        //then saves it to the database
        public void EditProject(ProjectViewModels project)
        {
            Projects projectToEdit = (from item in db.Project where item.projectID == project.projectID select item).SingleOrDefault();
            projectToEdit.projectName = project.projectName;
            if (project.memberCount > 0)
            {
                projectToEdit.memberCount = project.memberCount - 1;
            }
            else
            {
                projectToEdit.memberCount = 0;
            }
            DateTime openDate = DateTime.ParseExact(project.openDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            DateTime closeDate = DateTime.ParseExact(project.closeDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            projectToEdit.open = openDate;
            projectToEdit.close = closeDate;
            db.SaveChanges();
        }

        //creates a ProjectViewModels list called result and gets all items from Project model where 
        //courseID is the same as the courseID sent into the function
        //adds all the information needed into result and returns result
        public List<ProjectViewModels> GetProjectsInCourse(int? courseID)
        {
            List<ProjectViewModels> result = (from item in db.Project
                                              where item.courseID == courseID
                                              select new ProjectViewModels
                                              {
                                                  projectID = item.projectID,
                                                  projectName = item.projectName,
                                                  courseID = item.courseID,
                                                  open = item.open,
                                                  close = item.close
                                              }).ToList();

            return result;

        }

        //creates a ProjectViewModels list called result
        //and gets all the items from the Project model where the item courseID is the same as
        //the courseID that is sent into the function and where the project is open
        //then it adds all the information needed into the result list and then returns result
        public List<ProjectViewModels> GetStudentProjectsInCourse(int? courseID)
        {
            List<ProjectViewModels> result = (from item in db.Project
                                              where item.courseID == courseID
                                              && item.open <= DateTime.Now
                                              select new ProjectViewModels
                                              {
                                                  projectID = item.projectID,
                                                  projectName = item.projectName,
                                                  courseID = item.courseID,
                                                  open = item.open,
                                                  close = item.close,
                                                  canHandIn = item.close >= DateTime.Now
                                              }).ToList();

            return result;

        }

        //create a Subprojects item and adds all the information needed in the item
        //saves to database 

        public void AddSubProject(SubProjectsViewModels subProject)
        {
            SubProjects item = new SubProjects();
            item.projectID = subProject.projectID;
            item.subProjectName = subProject.subProjectName;
            item.percentage = subProject.percentage;
            if (subProject.inputFile != null)
            {
                MemoryStream target = new MemoryStream();
                subProject.inputFile.InputStream.CopyTo(target);
                byte[] data = target.ToArray();
                var name = subProject.inputFile.FileName;
                var mime = subProject.inputFile.ContentType;
                item.inputFileName = name;
                item.inputFileContentType = mime;
                item.inputFile = data;
            }

            if (subProject.outputFile != null)
            {
                MemoryStream target = new MemoryStream();
                subProject.outputFile.InputStream.CopyTo(target);
                byte[] data = target.ToArray();
                var name = subProject.outputFile.FileName;
                var mime = subProject.outputFile.ContentType;
                item.outputFileName = name;
                item.outputFileContentType = mime;
                item.outputFile = data;
            }

            db.SubProjects.Add(item);
            db.SaveChanges();
        }

        //creates SubProjectsViewModels item called sub where the item subProjectID is the same as the
        //subprojectID sent into the function, gets all the information needed and then returns sub
        public SubProjectsViewModels DownloadInputFile(int? subProjectID)
        {
            SubProjectsViewModels sub = (from item in db.SubProjects
                                         where item.subProjectID == subProjectID
                                         select new SubProjectsViewModels
                                         {
                                             inputFileBytes = item.inputFile,
                                             inputFileName = item.inputFileName,
                                             inputContentType = item.inputFileContentType
                                         }).SingleOrDefault();
            return sub;
        }


        //creates SubProjectsViewModels item called sub where the item subProjectID is the same as the
        //subprojectID sent into the function, gets all the information needed and then returns sub
        public SubProjectsViewModels DownloadOutputFile(int? subProjectID)
        {
            SubProjectsViewModels sub = (from item in db.SubProjects
                                         where item.subProjectID == subProjectID
                                         select new SubProjectsViewModels
                                         {
                                             outputFileBytes = item.outputFile,
                                             outputFileName = item.outputFileName,
                                             inputContentType = item.inputFileContentType
                                         }).SingleOrDefault();
            return sub;
        }


        //creates SubProjectsViewModels item called sub where the item partResponseID is the same as the
        //partResponseID sent into the function, gets all the information needed and then returns sub
        public SubProjectsViewModels DownloadPartResponseFile(int? partResonseID)
        {
            SubProjectsViewModels sub = (from item in db.PartResponse
                                         where item.partResponseID == partResonseID
                                         select new SubProjectsViewModels
                                         {
                                             inputFileBytes = item.file,
                                             inputFileName = item.fileFileName,
                                             inputContentType = item.fileMimeType
                                         }).SingleOrDefault();
            return sub;
        }

        //creates a SubProjectsViewModels item called subProject where the
        //subprojectID sent into the function is the same as the subproject subprojectID
        //gets all the information necessary and returns subproject
        public SubProjectsViewModels GetSubProjectByID(int? subProjectID)
        {
            SubProjectsViewModels subProject = (from item in db.SubProjects
                                                join project in db.Project on item.projectID equals project.projectID
                                                where item.subProjectID == subProjectID
                                                select new SubProjectsViewModels
                                                {
                                                    projectID = item.projectID,
                                                    subProjectID = item.subProjectID,
                                                    subProjectName = item.subProjectName,
                                                    percentage = item.percentage,
                                                    inputFileName = item.inputFileName,
                                                    outputFileName = item.outputFileName,
                                                    projectName = project.projectName,
                                                    memberCount = project.memberCount
                                                }).SingleOrDefault();

            return subProject;
        }

        //gets the rigth subproject and removes it
        public void DeleteSubProject(int? subProjectID)
        {
            SubProjects sub = (from item in db.SubProjects
                               where item.subProjectID == subProjectID
                               select item).SingleOrDefault();
            db.SubProjects.Remove(sub);
            db.SaveChanges();

        }
        //gets the subproject that has the ssame ID as the one that is sent into the function
        //gets all the information necessary and saves the changes
        public void EditSubProject(SubProjectsViewModels sub)
        {
            SubProjects subProject = (from item in db.SubProjects where item.subProjectID == sub.subProjectID select item).SingleOrDefault();
            subProject.projectID = sub.projectID;
            subProject.subProjectName = sub.subProjectName;
            subProject.percentage = sub.percentage;
            if (sub.inputFile != null)
            {
                MemoryStream target = new MemoryStream();
                sub.inputFile.InputStream.CopyTo(target);
                byte[] data = target.ToArray();
                var name = sub.inputFile.FileName;
                var mime = sub.inputFile.ContentType;
                subProject.inputFileName = name;
                subProject.inputFileContentType = mime;
                subProject.inputFile = data;
            }

            if (sub.outputFile != null)
            {
                MemoryStream target = new MemoryStream();
                sub.outputFile.InputStream.CopyTo(target);
                byte[] data = target.ToArray();
                var name = sub.outputFile.FileName;
                var mime = sub.outputFile.ContentType;
                subProject.outputFileName = name;
                subProject.outputFileContentType = mime;
                subProject.outputFile = data;
            }

            db.SaveChanges();

        }

        public void submitSubProject(PartResponseViewModels response)
        {
            ResponseViewModels responseExists = (from item in db.StudentResponse
                                                 join res in db.Response on item.responseID equals res.responseID
                                                 where item.userID == response.userID && res.projectID == response.projectID
                                                 select new ResponseViewModels
                                                 {
                                                     responseID = item.responseID
                                                 }).SingleOrDefault();
            int responseID = 0;
            if (responseExists == null)
            {
                Response addResponse = new Response();
                addResponse.projectID = response.projectID;
                db.Response.Add(addResponse);
                db.SaveChanges();
                int? maxID = db.Response.Max(u => (int?)u.responseID);
                responseID = (int)maxID;
            }
            else
            {
                responseID = responseExists.responseID;
            }


            StudentsResponseViewModels studentResponseExists = (from item in db.StudentResponse
                                                                where item.userID == response.userID && item.responseID == responseID
                                                                select new StudentsResponseViewModels
                                                                {
                                                                    userID = item.userID
                                                                }).SingleOrDefault();
            if (studentResponseExists == null)
            {
                StudentsResponse studentRes = new StudentsResponse();
                studentRes.responseID = responseID;
                studentRes.userID = response.userID;
                db.StudentResponse.Add(studentRes);
            }

            if (response.groupMembers != null)
            {
                foreach (UsersViewModels member in response.groupMembers)
                {
                    if (member.userID != null)
                    {
                        StudentsResponse memberResponseExists = (from item in db.StudentResponse
                                                                 where item.userID == member.userID && item.responseID == responseID
                                                                 select item).SingleOrDefault();
                        if (memberResponseExists == null)
                        {
                            StudentsResponse studentRes = new StudentsResponse();
                            studentRes.responseID = responseID;
                            studentRes.userID = member.userID;
                            db.StudentResponse.Add(studentRes);
                        }
                    }
                }
            }

            PartResponse part = new PartResponse();
            part.subProjectID = response.subProjectID;
            part.responseID = responseID;
            part.date = DateTime.Now;
            part.handedIn = response.userID;

            if (response.file != null)
            {
                MemoryStream target = new MemoryStream();
                response.file.InputStream.CopyTo(target);
                byte[] data = target.ToArray();
                var name = response.file.FileName;
                var mime = response.file.ContentType;
                part.fileFileName = name;
                part.fileMimeType = mime;
                part.file = data;
            }

            int status = new Random().Next(1, 5);
            part.status = status;

            //TODO compile-a kóða

            db.PartResponse.Add(part);
            db.SaveChanges();
        }

        public List<ResponseViewModels> GetStudentResponses(string userID, int? projectID)
        {
            List<ResponseViewModels> responses = (from pRes in db.PartResponse
                                                  join stRes in db.StudentResponse on pRes.responseID equals stRes.responseID
                                                  join res in db.Response on pRes.responseID equals res.responseID
                                                  join part in db.SubProjects on pRes.subProjectID equals part.subProjectID
                                                  where res.projectID == projectID && stRes.userID == userID
                                                  select new ResponseViewModels
                                                  {
                                                      fileName = pRes.fileFileName,
                                                      partResponseID = pRes.partResponseID,
                                                      subProjectName = part.subProjectName,
                                                      handedIn = pRes.handedIn,
                                                      status = pRes.status
                                                  }).ToList();
            List<UsersViewModels> users = new List<UsersViewModels>();
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            List<ApplicationUser> allUsers = um.Users.OrderBy(x => x.firstName).ToList();

            foreach (ResponseViewModels res in responses)
            {
                if (res.handedIn != null)
                {
                    foreach (ApplicationUser us in allUsers)
                    {
                        if (us.Id == res.handedIn)
                        {
                            res.members = us.UserName;
                        }
                    }
                }

            }

            return responses;
        }
    }
}