using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mooshak2.Models;
using Mooshak2.Services;
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
        private ApplicationDbContext db = new ApplicationDbContext();
        IdentityManager man = new IdentityManager();
        private ProjectService projectService = new ProjectService();

        public List<CoursesViewModels> GetAllCourses()
        {
            List<CoursesViewModels> courses = new List<CoursesViewModels>();
            List<Courses> result = (from item in db.Course
                                    select item).ToList();
            foreach(Courses course in result)
            {
                List<UsersViewModels> students = GetStudentsInCourse(course.courseID);
                List<UsersViewModels> teachers = GetTeachersInCourse(course.courseID);
                CoursesViewModels tmp = new CoursesViewModels();
                tmp.courseID = course.courseID;
                tmp.courseName = course.courseName;
                tmp.students = students;
                tmp.teachers = teachers;
                courses.Add(tmp);
            }
            return courses;
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

            course.teachers = GetTeachersInCourse(course.courseID);
            course.students = GetStudentsInCourse(course.courseID);
            course.projects = projectService.GetProjectsInCourse(course.courseID);
            //GetProjectsInCourse
            return course;
        }

        public CoursesViewModels GetStudentCourseByID(int? courseID)
        {
            CoursesViewModels course = (from item in db.Course
                                        where item.courseID == courseID
                                        select new CoursesViewModels
                                        {
                                            courseID = item.courseID,
                                            courseName = item.courseName
                                        }).SingleOrDefault();

            course.teachers = GetTeachersInCourse(course.courseID);
            course.students = GetStudentsInCourse(course.courseID);
            course.projects = projectService.GetStudentProjectsInCourse(course.courseID);
            //GetProjectsInCourse
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
            List<ApplicationUser> allUsers = um.Users.OrderBy(x => x.firstName).ToList();

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
                        tmp.fullName = u.firstName + " " + u.lastName;
                        tmp.username = u.UserName;
                        tmp.userID = u.Id;
                        tmp.email = u.Email;
                        users.Add(tmp);
                    }
                }
            }

            return users;

        }
        public List<UsersViewModels> GetStudentsInCourseExceptMe(int? courseID, string userId)
        {
            List<UsersViewModels> users = new List<UsersViewModels>();
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            List<ApplicationUser> allUsers = um.Users.OrderBy(x => x.firstName).ToList();

            foreach (ApplicationUser u in allUsers)
            {
                if(u.Id != userId)
                {
                    if (man.UserIsInRole(u.Id, "Student"))
                    {
                        StudentGroup studentExists = db.StudentGroup.SingleOrDefault(t => t.userID == u.Id && t.courseID == courseID);
                        if (studentExists != null)
                        {
                            UsersViewModels tmp = new UsersViewModels();
                            tmp.firstName = u.firstName;
                            tmp.lastName = u.lastName;
                            tmp.fullName = u.firstName + " " + u.lastName;
                            tmp.username = u.UserName;
                            tmp.userID = u.Id;
                            tmp.email = u.Email;
                            users.Add(tmp);
                        }
                    }
                }
            }

            return users;

        }

        public List<UsersViewModels> GetTeachersInCourse(int? courseID)
        {
            List<UsersViewModels> users = new List<UsersViewModels>();
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            List<ApplicationUser> allUsers = um.Users.OrderBy(x => x.firstName).ToList();
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
                        tmp.email = u.Email;
                        users.Add(tmp);
                    }
                }
            }

            return users;

        }


        public void DeleteCourse(int? courseID)
        {
            //Eyða öllum kennurum úr áfanganum
            List<TeacherGroup> allTeachers = (from item in db.TeacherGroup
                                              where item.courseID == courseID
                                        select item).ToList();

            foreach(TeacherGroup t in allTeachers){
                db.TeacherGroup.Remove(t);
            }
            //Eyða öllum nemendum úr áfanganum
            List<StudentGroup> allStudents = (from item in db.StudentGroup
                                              where item.courseID == courseID
                                              select item).ToList();

            foreach (StudentGroup s in allStudents)
            {
                db.StudentGroup.Remove(s);
            }
            db.SaveChanges();

            //Eyða áfanganum
            Courses course = db.Course.SingleOrDefault(t => t.courseID == courseID);
            db.Course.Remove(course);

            db.SaveChanges();
        }

        public List<CoursesViewModels> GetStudentCourses(string userID)
        {
            List<CoursesViewModels> allCourses = (from student in db.StudentGroup
                                                  join course in db.Course on student.courseID equals course.courseID
                                                  where student.userID == userID
                                                  select new CoursesViewModels
                                                  {
                                                      courseID = course.courseID,
                                                      courseName = course.courseName
                                                  }).ToList();
            return allCourses;
        }

        public List<CoursesViewModels> GetTeacherCourses(string userID)
        {
            List<CoursesViewModels> allCourses = (from teacher in db.TeacherGroup
                                                  join course in db.Course on teacher.courseID equals course.courseID
                                                  where teacher.userID == userID
                                                  select new CoursesViewModels
                                                  {
                                                      courseID = course.courseID,
                                                      courseName = course.courseName
                                                  }).ToList();
            return allCourses;
        }

    }

}