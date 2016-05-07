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
            }
            if(user.isTeacher == true)
            {
                Teachers teacher = new Teachers();
                teacher.userID = userId;
            }
            if(user.isStudent == true)
            {
                Students student = new Students();
                student.userID = userId;
            }

            db.SaveChanges();

        }
    }
}