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
    //Klasi sem tengist gagnagrunninum
    public class UserService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        IdentityManager man = new IdentityManager();

        //creates a ApplicationUser appUser and gets its values by calling the GetUser function
        // creates a new UsersViewModels called user
        //and puts all the values from appUser into user
        // and returns user
        public UsersViewModels GetUserById(string username)
        {
            ApplicationUser appUser = man.GetUser(username);
            UsersViewModels user = new UsersViewModels();
            user.firstName = appUser.firstName;
            user.lastName = appUser.lastName;
            user.username = appUser.UserName;
            user.email = appUser.Email;
            user.userID = appUser.Id;
            Boolean isAdmin = false;
            Boolean isTeacher = false;
            Boolean isStudent = false;
            IList<string> roles = man.GetUserRoles(appUser.Id);
            foreach(string role in roles){
                if (role.Equals("Admin"))
                {
                    isAdmin = true;
                }
                if (role.Equals("Teacher"))
                {
                    isTeacher = true;
                }
                if (role.Equals("Student"))
                {
                    isStudent = true;
                }
            }
            user.isAdmin = isAdmin;
            user.isTeacher = isTeacher;
            user.isStudent = isStudent;
            
            return user;
        }

        //Creates a list of allUsers that is a UsersViewModels list
        //creates a list users that selects everything from the Users model and orders them by first name
        //for each users in the users list it gets the values from the Users model and
        //and puts them into UsersViewModels list which was created at the top of the function
        //then it returns the allUsers list
        public List<UsersViewModels> GetAllUsers()
        {

            List<UsersViewModels> allUsers = new List<UsersViewModels>();
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            List<ApplicationUser> users = um.Users.OrderBy(x => x.firstName).ToList();
            foreach (ApplicationUser u in users)
            {
                    UsersViewModels tmp = new UsersViewModels();
                    tmp.firstName = u.firstName;
                    tmp.lastName = u.lastName;
                    tmp.username = u.UserName;
                    tmp.userID = u.Id;
                    tmp.email = u.Email;
                Boolean isTeacher = false;
                Boolean isStudent = false;
                Boolean isAdmin = false;
                if(man.UserIsInRole(tmp.userID, "Teacher"))
                {
                    isTeacher = true;
                }
                if(man.UserIsInRole(tmp.userID, "Student"))
                {
                    isStudent = true;
                }
                if(man.UserIsInRole(tmp.userID, "Admin"))
                {
                    isAdmin = true;
                }
                tmp.isAdmin = isAdmin;
                tmp.isTeacher = isTeacher;
                tmp.isStudent = isStudent;
                allUsers.Add(tmp);
            }
            
            return allUsers;
        }



        //creates a ApplicationUser newUser and adds all the informations of the user sent into the function
        //saves it to the database
        public void CreateNewUser(UsersViewModels user)
        {
            ApplicationUser newUser = new ApplicationUser
            {
                UserName = user.username,
                Email = user.email,
                lastName = user.lastName,
                firstName = user.firstName
            };
            bool result = man.CreateUser(newUser, user.password);
            if (result)
            {
                if (user.isAdmin == true)
                {
                    man.AddUserToRole(newUser.Id, "Admin");
                }
                if (user.isTeacher == true)
                {
                    man.AddUserToRole(newUser.Id, "Teacher");
                }
                if (user.isStudent == true)
                {
                    man.AddUserToRole(newUser.Id, "Student");
                }
            }
            else
            {

            }

            db.SaveChanges();

        }

        //creates appUser and adds all the informations of the user sent into the function
        //if the Admin checkbox is checked and the user was not in role Admin, he is put in role Admin
        //if the Admin checkbox is not Checked and the user was in role Admin, he is removed from role Admin
        //if the Teacher checkbox is checked and the user was not in role Teacher, he is put in role Teacher
        //if the Teacher checkbox is not Checked and the user was in role Teacher, he is removed from role Teacher
        //if the Student checkbox is checked and the user was not in role Student, he is put in role Student
        //if the Student checkbox is not Checked and the user was in role Student, he is removed from role Student
        //then it is saved to the database
        public void EditUser(UsersViewModels user)
        {
            ApplicationUser appUser = man.GetUser(user.username);
            appUser.UserName = user.username;
            appUser.firstName = user.firstName;
            appUser.lastName = user.lastName;
           
            if (user.isAdmin == true)
            {
                if (man.UserIsInRole(appUser.Id, "Admin") == false)
                {
                    man.AddUserToRole(appUser.Id, "Admin");
                }
            }
            else if (user.isAdmin == false)
            {
                if (man.UserIsInRole(appUser.Id, "Admin") == true)
                {
                    man.RemoveUserRole(appUser.Id, "Admin");
                }
            }
            //Uppfæra teacher
            if (user.isTeacher == true)
            {
                if (man.UserIsInRole(appUser.Id, "Teacher") == false)
                {
                    man.AddUserToRole(appUser.Id, "Teacher");
                }
            }
            else if (user.isTeacher == false)
            {
                if (man.UserIsInRole(appUser.Id, "Teacher") == true)
                {
                    man.RemoveUserRole(appUser.Id, "Teacher");
                }
            }

            //Uppfæra student
            if (user.isStudent == true)
            {
                if (man.UserIsInRole(appUser.Id, "Student") == false)
                {
                    man.AddUserToRole(appUser.Id, "Student");
                }
            }
            else if (user.isStudent == false)
            {
                if (man.UserIsInRole(appUser.Id, "Student") == true)
                {
                    man.RemoveUserRole(appUser.Id, "Student");
                }
            }
            db.SaveChanges();
        }

        //Creates a list of users that is a UsersViewModels list
        //creates a list allUsers that selects everything from the Users model and orders them by first name
        //for each user in the allUsers list it checks if user is in role Teacher
       // and gets the values from the Users model and
        //and puts them into UsersViewModels list which was created at the top of the function
        //then it returns the users list
        public List<UsersViewModels> GetAllTeachers(int? courseID)
        {
            List<UsersViewModels> users = new List<UsersViewModels>();
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            List<ApplicationUser> allUsers = um.Users.OrderBy(x => x.firstName).ToList();
            foreach (ApplicationUser u in allUsers)
            {
                if(man.UserIsInRole(u.Id, "Teacher"))
                {
                    UsersViewModels tmp = new UsersViewModels();
                    tmp.firstName = u.firstName;
                    tmp.lastName = u.lastName;
                    tmp.username = u.UserName;
                    tmp.userID = u.Id;
                    Boolean selected = false;
                    TeacherGroup teacherExists = db.TeacherGroup.SingleOrDefault(t => t.userID == u.Id && t.courseID == courseID);
                    if (teacherExists != null)
                    {
                        selected = true;
                    }
                    tmp.selected = selected;
                    users.Add(tmp);
                }
            }
            return users;
        }

        //for each user in users it checks if it was selected, and if it already is in group
        //if it is not already in group it is added to the group
        //if it is not selected but was in the group, it is removed from the group
        //then it is saved to the database
        public void AddTeachersToGroup(int? courseID, List<UsersViewModels> users)
        {
            foreach (UsersViewModels user in users)
            {

                if (user.selected == true)
                {
                    TeacherGroup teacherExists = db.TeacherGroup.SingleOrDefault(t => t.userID == user.userID && t.courseID == courseID);
                    if (teacherExists == null)
                    {
                        TeacherGroup teacher = new TeacherGroup();
                        teacher.userID = user.userID;
                        teacher.courseID = (int)courseID;
                        db.TeacherGroup.Add(teacher);
                    }
                }
                else if(user.selected == false)
                {
                    TeacherGroup teacherToDelete = db.TeacherGroup.SingleOrDefault(t => t.userID == user.userID && t.courseID == courseID);
                    if (teacherToDelete != null)
                    {
                        db.TeacherGroup.Remove(teacherToDelete);
                    }
                }
                db.SaveChanges();
            }

        }

        //Creates a list of users that is a UsersViewModels list
        //creates a list allUsers that selects everything from the Users model and orders them by first name
        //for each user in the allUsers list it checks if user is in role Student
        // and gets the values from the Users model and
        //and puts them into UsersViewModels list which was created at the top of the function
        //then it returns the users list
        public List<UsersViewModels> GetAllStudents(int? courseID)
        {
            List<UsersViewModels> users = new List<UsersViewModels>();
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            List<ApplicationUser> allUsers = um.Users.OrderBy(x => x.firstName).ToList();
            foreach (ApplicationUser u in allUsers)
            {
                if (man.UserIsInRole(u.Id, "Student"))
                {
                    UsersViewModels tmp = new UsersViewModels();
                    tmp.firstName = u.firstName;
                    tmp.lastName = u.lastName;
                    tmp.username = u.UserName;
                    tmp.userID = u.Id;
                    Boolean selected = false;
                    StudentGroup studentExists = db.StudentGroup.SingleOrDefault(t => t.userID == u.Id && t.courseID == courseID);
                    if (studentExists != null)
                    {
                        selected = true;
                    }
                    tmp.selected = selected;
                    users.Add(tmp);
                }
            }
            return users;
        }

        //for each user in users it checks if it was selected, and if it already is in group
        //if it is not already in group it is added to the group
        //if it is not selected but was in the group, it is removed from the group
        //then it is saved to the database
        public void AddStudentsToGroup(int? courseID, List<UsersViewModels> users)
        {
            foreach (UsersViewModels user in users)
            {

                if (user.selected == true)
                {
                    StudentGroup studentExists = db.StudentGroup.SingleOrDefault(t => t.userID == user.userID && t.courseID == courseID);
                    if (studentExists == null)
                    {
                        StudentGroup student = new StudentGroup();
                        student.userID = user.userID;
                        student.courseID = (int)courseID;
                        db.StudentGroup.Add(student);
                    }
                }
                else if (user.selected == false)
                {
                    StudentGroup studentToDelete = db.StudentGroup.SingleOrDefault(t => t.userID == user.userID && t.courseID == courseID);
                    if (studentToDelete != null)
                    {
                        db.StudentGroup.Remove(studentToDelete);
                    }
                }
                db.SaveChanges();
            }

        }
    }
}