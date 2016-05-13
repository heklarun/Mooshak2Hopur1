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


        //Creates a list of courses that is a CoursesViewModels list
        //creates a list result that selects everything from the Courses model
        //for each course in the result list it gets the values from the Course model and
        //and puts them into CoursesViewModels list which was created at the top of the function
        //then it returns the courses list
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

        //creates a variable called newCourse and adds the courseName sent into the function
        //adds the newCourse into the Course list that saves it to the database
        public void CreateNewCourse(CoursesViewModels courseToAdd)
        {

            var newCourse = new Courses();
            newCourse.courseName = courseToAdd.courseName;
            
                db.Course.Add(newCourse);
                db.SaveChanges();
           
        }

        // Creates a course that is an item in the Course model where the courseID that is sent into the
        //function and the courseID of the item
        //puts the courseID of the item into the courseID in CoursesViewModels and 
        //puts the courseName of the item into the courseName in CoursesViewModels
        //then it gets the teachers,students and projects in the course, by calling the functions
        //GetTeachersInCourse, GetStudentsInCourse and GetProjectsInCourse with the right
        //courseID, then returns the course
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

        // Creates a course that is an item in the Course model where the courseID that is sent into the
        //function and the courseID of the item
        //puts the courseID of the item into the courseID in CoursesViewModels and 
        //puts the courseName of the item into the courseName in CoursesViewModels
        //then it gets the teachers,students and projects in the course, by calling the functions
        //GetTeachersInCourse, GetStudentsInCourse and GetStudentProjectsInCourse with the right
        //courseID, then returns the course
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
            return course;
        }

        //creates courseToEdit and gets the correct course by getting the courseID
        //that is the same as the courseID of the course that is sent into the function
        //puts the courseName of the course sent into the function into the courseToEdit coursename
        //and saves it to the database
        public void EditCourse(CoursesViewModels course)
        {
            Courses courseToEdit = db.Course.Single(u => u.courseID == course.courseID);
            courseToEdit.courseName = course.courseName;
            db.SaveChanges();

        }

        //Creates a list called users that is a UsersViewModels list
        //gets a list of allUsers and orders them by first name
        //for each user in allUsers it checks if the user exists 
        //and checks it they are in role Student
        //and if it exists and they are in role Student we create a UserViewModels type called tmp
        //and take all the information from the user in allUsers and put it into tmp
        //then adds tmp into the users list and return it
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

        //does the same as the function GetStudentsInCourse but before we add the users to the users list 
        //we check if the id sent in is the same as a userid on the list allUsers
        //if it is the same, we dont add it to the users list
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

        //Creates a list called users that is a UsersViewModels list
        //gets a list of allUsers and orders them by first name
        //for each user in allUsers it checks if the user exists 
        //and checks it they are in role Teacher
        //and if it exists and they are in role Teacher we create a UserViewModels type called tmp
        //and take all the information from the user in allUsers and put it into tmp
        //then adds tmp into the users list and return it
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

        //gets a list of allTeachers in the course and deletes them
        //gets a list of allStudents in the course and deletes them saves it to the database
        //then it gets the right course and deletes it and saves it to the database
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
        
        //creates a list allCourses that selects everything from the StudentGroup model
        //for each course in the result list it gets the values from the StudentGroup model and
        //and puts them into CoursesViewModels list 
        //then it returns the allCourses list
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

        //creates a list allCourses that selects everything from the TeacherGroup model
        //for each course in the result list it gets the values from the TeacherGroup model and
        //and puts them into CoursesViewModels list 
        //then it returns the allCourses list
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