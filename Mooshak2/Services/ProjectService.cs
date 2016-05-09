using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mooshak2.Models;
using SecurityWebAppTest.Models;

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

        public void CreateNewProject(ProjectViewModels projectToAdd)
        {

            var newProject = new Projects();
            newProject.projectName = projectToAdd.projectName;

            db.Project.Add(newProject);
            db.SaveChanges();

        }

        public ProjectViewModels GetProjectByID(int? projectID)
        {
            ProjectViewModels project = (from item in db.Project
                                         where item.projectID == projectID
                                         select new ProjectViewModels
                                         {
                                             projectID = item.projectID,
                                             projectName = item.projectName
                                             //ATH bæta við
                                         }).SingleOrDefault();
            return project;
        }

        public void EditProject(ProjectViewModels project)
        {
            Projects projectToEdit = db.Project.Single(x => x.projectID == project.projectID);
            //projectToEdit.projectParts = project.projectParts;
            projectToEdit.projectName = project.projectName; //ATH Vantar fyrir próstentu og það dæmi, þarf að útfæra í model fyrst
            //projectToEdit.projectPrecent = project.projectPrecent;+
            //projectToEdit.inputFile = project.inputFile;
            //projectToEdit.outputFile = project.outputFile;
            db.SaveChanges();
        }

        public List<Projects> GetProjectsInCourse(int? courseID)
        {
            List<Projects> result = (from item in db.Project
                                     select item).ToList();

            //Vantar loopu sem fer í gegnum course Id og birtir þau verkefni í áfanganum
            return result;


        }
    }
}