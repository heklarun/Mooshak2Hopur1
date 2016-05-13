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
        ApplicationDbContext db = new ApplicationDbContext();
        IdentityManager man = new IdentityManager();
        private UserService userService = new UserService();
        ProjectService projectService = new ProjectService();
        CourseService courseService = new CourseService();

        public List<Projects> GetAllProjects()
        {
            List<Projects> result = (from item in db.Project
                                     select item).ToList();
            return result;
        }

        public int CreateNewProject(ProjectViewModels projectToAdd)
        {

            Projects newProject = new Projects();
            newProject.projectName = projectToAdd.projectName;
            newProject.courseID = projectToAdd.courseID;
            if(projectToAdd.memberCount > 0)
            {
                newProject.memberCount = projectToAdd.memberCount-1;
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

            int? maxID = db.Project.Max(u => (int?)u.projectID) ;

            int projectId = 0;
            if (maxID != null) {
                projectId = (int)maxID;
            }

            return projectId;

        }

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
            foreach(SubProjectsViewModels sub in subProjects)
            {
                int Count =  (from item in db.PartResponse where item.subProjectID == sub.subProjectID select item).Count();
                sub.nrOfResponses = Count;
            }
            project.subProjects = subProjects;


            return project;
        }

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

            if(project.memberCount > 1)
            {
                ResponseViewModels response = (from item in db.Response
                                               join sRes in db.StudentResponse on item.responseID equals sRes.responseID
                                               where item.projectID == (int)projectID && sRes.userID == userID
                                               select new ResponseViewModels
                                               {
                                                   responseID = item.responseID
                                               }).SingleOrDefault();
                if(response != null)
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

        public SubProjectsViewModels DownloadOutputFile(int? subProjectID)
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
        public void DeleteSubProject(int? subProjectID)
        {
            SubProjects sub = (from item in db.SubProjects where item.subProjectID ==subProjectID
                               select item).SingleOrDefault();
            db.SubProjects.Remove(sub);
            db.SaveChanges();

        }

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

            if(response.groupMembers != null)
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
                if(res.handedIn != null)
                {
                    foreach(ApplicationUser us in allUsers)
                    {
                        if(us.Id == res.handedIn)
                        {
                            res.members = us.UserName;
                        }
                    }
                }
                
            }

            return responses;
        }

        public ActionResult CompileCode(FormCollection data, int? partResponseID)
        {
            // To simplify matters, we declare the code here.
            // The code would of course come from the student!   
            ApplicationUser appUser = man.GetUser(User.Identity.Name);
            SubProjectsViewModels sub = projectService.DownloadPartResponseFile(partResponseID);
            MemoryStream ms = new MemoryStream(sub.inputFileBytes);

            var sr = new StreamReader(ms);
            string myStr = sr.ReadToEnd();

            var code = myStr;

            // Set up our working folder, and the file names/paths.
            // In this example, this is all hardcoded, but in a
            // real life scenario, there should probably be individual
            // folders for each user/assignment/milestone.
            string serverPath = Server.MapPath("~");
            var workingFolder = ConfigurationManager.AppSettings["workingFolder"];  // Hvað á að fara hér inn? ef við erum að nota gagnagrunstengingu

            string filePathFull = serverPath + workingFolder + "\\" + appUser.UserName; //það sem exeFilePath var

            if (!Directory.Exists(filePathFull))
            {
                Directory.CreateDirectory(filePathFull);
            }

            var username = appUser;
            var cppFileName = partResponseID + "_" + appUser.UserName + ".cpp";  //Aðgerð til að ná í cpp skrá

            //var exeFilePath = workingFolder + "Hello.exe";  // Hvað er exe file?
            // Write the code to a file, such that the compiler
            // can find it:
            System.IO.File.WriteAllText(workingFolder + cppFileName, code);

            // In this case, we use the C++ compiler (cl.exe) which ships
            // with Visual Studio. It is located in this folder:
            var compilerFolder = ConfigurationManager.AppSettings["compilerFolder"];
            // There is a bit more to executing the compiler than
            // just calling cl.exe. In order for it to be able to know
            // where to find #include-d files (such as <iostream>),
            // we need to add certain folders to the PATH.
            // There is a .bat file which does that, and it is
            // located in the same folder as cl.exe, so we need to execute
            // that .bat file first.

            // Using this approach means that:
            // * the computer running our web application must have
            //   Visual Studio installed. This is an assumption we can
            //   make in this project.
            // * Hardcoding the path to the compiler is not an optimal
            //   solution. A better approach is to store the path in
            //   web.config, and access that value using ConfigurationManager.AppSettings.

            // Execute the compiler:
            Process compiler = new Process();
            compiler.StartInfo.FileName = "cmd.exe";
            compiler.StartInfo.WorkingDirectory = workingFolder;
            compiler.StartInfo.RedirectStandardInput = true;
            compiler.StartInfo.RedirectStandardOutput = true;
            compiler.StartInfo.UseShellExecute = false;

            compiler.Start();
            compiler.StandardInput.WriteLine("\"" + compilerFolder + "vcvars32.bat" + "\"");
            compiler.StandardInput.WriteLine("cl.exe /nologo /EHsc " + cppFileName);
            compiler.StandardInput.WriteLine("exit");

            string output = compiler.StandardOutput.ReadToEnd();
            compiler.WaitForExit();
            compiler.Close();

            // Check if the compile succeeded, and if it did,
            // we try to execute the code:
            if (System.IO.File.Exists(filePathFull))
            {
                var processInfoExe = new ProcessStartInfo(filePathFull, "");
                processInfoExe.UseShellExecute = false;
                processInfoExe.RedirectStandardOutput = true;
                processInfoExe.RedirectStandardError = true;
                processInfoExe.CreateNoWindow = true;
                using (var processExe = new Process())
                {
                    processExe.StartInfo = processInfoExe;
                    processExe.Start();
                    // In this example, we don't try to pass any input
                    // to the program, but that is of course also
                    // necessary. We would do that here, using
                    processExe.StandardInput.WriteLine(); //Það sem var kommentað
                    /*processExe.Start();
                    processExe.StandardInput.WriteLine("\"" + compilerFolder + "vcvars32.bat" + "\"");
                    processExe.StandardInput.WriteLine("cl.exe /nologo /EHsc " + cppFileName);
                    processExe.StandardInput.WriteLine("exit"); */
                    // to above.

                    // We then read the output of the program:
                    var lines = new List<string>();
                    while (!processExe.StandardOutput.EndOfStream)
                    {
                        lines.Add(processExe.StandardOutput.ReadLine());
                    }

                    ViewBag.Output = lines;
                }
            }
            else
            {
                //ef skrá er ekki til hvað þá?
            }

            // TODO: We might want to clean up after the process, there
            // may be files we should delete etc.
            // Delete þeim skrám sem búnar hafa verið til í kjöæfar þess að keyra kóðann
            // Búa til fall sem tékkar á tímanum sem forritið er að keyrast. Ef 10 sec + þá executea


            return View();
        }

}