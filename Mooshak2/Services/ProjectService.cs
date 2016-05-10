using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mooshak2.Models;
using SecurityWebAppTest.Models;
using System.IO;
using System.Globalization;

namespace Mooshak2.Services
{
    public class ProjectService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        IdentityManager man = new IdentityManager();

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
                                             close = item.close
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

        public void EditProject(ProjectViewModels project)
        {
            Projects projectToEdit = (from item in db.Project where item.projectID == project.projectID select item).SingleOrDefault();
            projectToEdit.projectName = project.projectName;
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
                                                projectName = project.projectName
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
    }
}