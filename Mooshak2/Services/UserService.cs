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

        //Nær í alla notendur
        public List<UsersViewModels> GetAllUsers()
        {
           // var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
           // return um.Users.ToList();

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
    


        //Býr til notanda
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

        //Breytir notanda
        public void EditUser(UsersViewModels user)
        {
            //ATH VIRKAR BARA ROLES EKKI AÐ BREYTA FIRST/LAST/USER NAME
            ApplicationUser appUser = man.GetUser(user.username);
            appUser.UserName = user.username;
            appUser.firstName = user.firstName;
            appUser.lastName = user.lastName;
            /*
            //var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            //var manager = new UserManager(store);

            var userContext = new ApplicationDbContext();
            var userStore = new UserStore<ApplicationUser>(userContext);

            var userManager = new UserManager<ApplicationUser>(userStore);
            userManager.Update(appUser);
            userContext.SaveChanges();*/

            //Uppfæra admin
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

        //Nær í alla kennara og athugar hvort þeir séu í ákveðnum áfanga
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