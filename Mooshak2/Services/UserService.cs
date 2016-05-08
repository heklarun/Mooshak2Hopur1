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
        public ApplicationUser GetUserById(string username)
        {
            IdentityManager man = new IdentityManager();
            return man.GetUser(username);
        }

        //Nær í alla notendur
        public List<UsersViewModels> GetAllUsers()
        {
           // List<Users> users = (from item in db.User select item).ToList();
             List<UsersViewModels> users = (from item in db.User 
                               join teacher in db.Teacher on item.userID equals teacher.userID into t
                               join admin in db.Admin on item.userID equals admin.userID into a
                               join student in db.Student on item.userID equals student.userID into s
                               from teacher in t.DefaultIfEmpty()
                               from admin in a.DefaultIfEmpty()
                               from student in s.DefaultIfEmpty()
                               select new UsersViewModels
                               {
                                   userID = item.userID,
                                   firstName = item.firstName,
                                   lastName = item.lastName,
                                   username = item.username,
                                   //teacherID = teacher.teacherID != null ? teacher.teacherID : 0,
                                   isTeacher = teacher.teacherID != null, 
                                   //studentID = student.studentID != null ? student.studentID : 0,
                                   isStudent = student.studentID != null,
                                   //adminID = admin.adminID != null ? admin.adminID : 0,
                                   isAdmin = admin.adminID != null
                               }).ToList();
            
            return users;
        }

        //Nær í notanda eftir id
        public UsersViewModels GetUserByUserID(int? userId)
        {
            // List<Users> users = (from item in db.User select item).ToList();
            UsersViewModels users = (from item in db.User
                                           join teacher in db.Teacher on item.userID equals teacher.userID into t
                                           join admin in db.Admin on item.userID equals admin.userID into a
                                           join student in db.Student on item.userID equals student.userID into s
                                           from teacher in t.DefaultIfEmpty()
                                           from admin in a.DefaultIfEmpty()
                                           from student in s.DefaultIfEmpty()
                                           where item.userID == userId
                                          select new UsersViewModels
                                           {
                                               userID = item.userID,
                                               firstName = item.firstName,
                                               lastName = item.lastName,
                                               username = item.username,
                                               teacherID = teacher.teacherID != null ? teacher.teacherID : 0,
                                               isTeacher = teacher.teacherID != null,
                                               studentID = student.studentID != null ? student.studentID : 0,
                                               isStudent = student.studentID != null,
                                               adminID = admin.adminID != null ? admin.adminID : 0,
                                               isAdmin = admin.adminID != null
                                           }).SingleOrDefault();

            return users;
        }

        //Býr til notanda
        public void CreateNewUser(UsersViewModels user)
        {
            Users users = new Users();
            users.firstName = user.firstName;
            users.lastName = user.lastName;
            users.username = user.username;
            users.password = user.password;

            db.User.Add(users);

            db.SaveChanges();
            int userId = db.User.Max(item => item.userID);
            if (user.isAdmin == true)
            {
                Admins admin = new Admins();
                admin.userID = userId;
                db.Admin.Add(admin);
            }
            if(user.isTeacher == true)
            {
                Teachers teacher = new Teachers();
                teacher.userID = userId;
                db.Teacher.Add(teacher);
            }
            if(user.isStudent == true)
            {
                Students student = new Students();
                student.userID = userId;
                db.Student.Add(student);
            }

            db.SaveChanges();

        }

        //Breytir notanda
        public void EditUser(UsersViewModels user)
        {
            Users userToEdit = db.User.Single(u => u.userID == user.userID);
            userToEdit.username = user.username;
            userToEdit.firstName = user.firstName;
            userToEdit.lastName = user.lastName;

            //Uppfæra admin
            if(user.isAdmin == true)
            {
                Admins adminExists = db.Admin.SingleOrDefault(a => a.userID == user.userID);
                if (adminExists == null)
                {
                    Admins admin = new Admins();
                    admin.userID = user.userID;
                    db.Admin.Add(admin);
                }
            }
            else if(user.isAdmin == false)
            {
                Admins adminToDelete = db.Admin.SingleOrDefault(a => a.userID == user.userID);
                if(adminToDelete != null)
                {
                    db.Admin.Remove(adminToDelete);
                }
            }
            //Uppfæra teacher
            if (user.isTeacher == true && user.teacherID == 0)
            {
                Teachers teacherExists = db.Teacher.SingleOrDefault(t => t.userID == user.userID);
                if (teacherExists == null)
                {
                    Teachers teacher = new Teachers();
                    teacher.userID = user.userID;
                    db.Teacher.Add(teacher);
                }
            }
            else if (user.isTeacher == false)
            {
                Teachers teacherToDelete = db.Teacher.SingleOrDefault(t => t.userID == user.userID);
                if (teacherToDelete != null)
                {
                    db.Teacher.Remove(teacherToDelete);
                }
            }

            //Uppfæra student
            if (user.isStudent == true && user.studentID == 0)
            {
                Students studentExists = db.Student.SingleOrDefault(s => s.userID == user.userID);
                if (studentExists == null)
                {
                    Students student = new Students();
                    student.userID = user.userID;
                    db.Student.Add(student);
                }
            }
            else if (user.isStudent == false)
            {
                Students studentToDelete = db.Student.SingleOrDefault(s => s.userID == user.userID);
                if (studentToDelete != null)
                {
                    db.Student.Remove(studentToDelete);
                }
            }
            db.SaveChanges();
        }

        //Nær í alla kennara og athugar hvort þeir séu í ákveðnum áfanga
        public List<UsersViewModels> GetAllTeachers(int? courseID)
        {
            List<UsersViewModels> users = (from teacher in db.Teacher
                                           join item in db.User on teacher.userID equals item.userID
                                           join teacherGroup in db.TeacherGroup on teacher.teacherID equals teacherGroup.teacherID into t
                                           from teacherGroup in t.DefaultIfEmpty()
                                           where (teacherGroup.courseID == courseID || teacherGroup.courseID == null) //Ath að ef teacherGroup.courseID er annað courseId þa kemur ekki sá kennari
                                           select new UsersViewModels
                                           {
                                               userID = item.userID,
                                               firstName = item.firstName,
                                               lastName = item.lastName,
                                               username = item.username, 
                                               teacherID = teacher.teacherID,
                                               selected = teacherGroup.teacherID != null
                                           }).ToList();

            return users;
        }

        public void AddTeachersToGroup(int? courseID, List<UsersViewModels> users)
        {
            foreach (UsersViewModels user in users)
            {

                if (user.selected == true)
                {
                    TeacherGroup teacherExists = db.TeacherGroup.SingleOrDefault(t => t.teacherID == user.teacherID && t.courseID == courseID);
                    if (teacherExists == null)
                    {
                        TeacherGroup teacher = new TeacherGroup();
                        teacher.teacherID = user.teacherID;
                        teacher.courseID = (int)courseID;
                        db.TeacherGroup.Add(teacher);
                    }
                }
                else if(user.selected == false)
                {
                    TeacherGroup teacherToDelete = db.TeacherGroup.SingleOrDefault(t => t.teacherID == user.teacherID && t.courseID == courseID);
                    if (teacherToDelete != null)
                    {
                        db.TeacherGroup.Remove(teacherToDelete);
                    }
                }
                db.SaveChanges();
            }

        }
    }
}