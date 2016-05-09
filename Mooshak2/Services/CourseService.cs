using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mooshak2.Models;
using SecurityWebAppTest.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mooshak2.DAL
{
    public class CourseService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        IdentityManager man = new IdentityManager();
        CourseService courseService = new CourseService();

        public List<Courses> GetAllCourses()
        {
            List<Courses> result = (from item in db.Course
                                    select item).ToList();
            return result;
        }
        public void CreateNewCourse(CoursesViewModels courseToAdd)
        {

            var newCourse = new Courses();
            newCourse.courseName = courseToAdd.courseName;
            
                db.Course.Add(newCourse);
                db.SaveChanges();
           
        }

        public CoursesViewModels GetCourseByID(int? courseID)
        {
            CoursesViewModels course = (from item in db.Course
                                     where item.courseID == courseID
                                     select new CoursesViewModels
                                     {
                                         courseID = item.courseID,
                                         courseName = item.courseName
                                     }).SingleOrDefault();

            course.teachers = courseService.GetTeachersInCourse(course.courseID);
            course.students = courseService.GetStudentsInCourse(course.courseID);
            return course;
        }

        public void EditCourse(CoursesViewModels course)
        {
            Courses courseToEdit = db.Course.Single(u => u.courseID == course.courseID);
            courseToEdit.courseName = course.courseName;
            db.SaveChanges();

        }

        public List<UsersViewModels> GetStudentsInCourse(int? courseID)
        {
            List<UsersViewModels> users = new List<UsersViewModels>();
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            List<ApplicationUser> allUsers = um.Users.ToList();
            foreach (ApplicationUser u in allUsers)
            {
                if (man.UserIsInRole(u.Id, "Student"))
                {
                    StudentGroup studentExists = db.StudentGroup.SingleOrDefault(t => t.userID == u.Id && t.courseID == courseID);
                    if (studentExists != null)
                    {
                        UsersViewModels tmp = new UsersViewModels();
                        tmp.firstName = u.firstName;
                        tmp.lastName = u.lastName;
                        tmp.username = u.UserName;
                        tmp.userID = u.Id;
                        users.Add(tmp);
                    }
                }
            }

            return users;

        }


        public List<UsersViewModels> GetTeachersInCourse(int? courseID)
        {
            List<UsersViewModels> users = new List<UsersViewModels>();
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            List<ApplicationUser> allUsers = um.Users.ToList();
            foreach (ApplicationUser u in allUsers)
            {
                if (man.UserIsInRole(u.Id, "Teacher"))
                {
                    TeacherGroup teacherExists = db.TeacherGroup.SingleOrDefault(t => t.userID == u.Id && t.courseID == courseID);
                    if (teacherExists != null)
                    {
                        UsersViewModels tmp = new UsersViewModels();
                        tmp.firstName = u.firstName;
                        tmp.lastName = u.lastName;
                        tmp.username = u.UserName;
                        tmp.userID = u.Id;
                        users.Add(tmp);
                    }
                }
            }

            return users;

        }

    }

}