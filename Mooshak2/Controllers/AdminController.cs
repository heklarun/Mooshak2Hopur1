using Mooshak2.DAL;
using Mooshak2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mooshak2.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin
        public ActionResult AdminIndex()
        {
            return View("AdminIndex");
        }

        public ActionResult CreateNewCourse()
        {
            return View("CreateNewCourse");
        }



        [HttpGet]
        public ActionResult CreateNewUser()
        {
            return View("CreateNewUser");
        }
        [HttpPost]
        public ActionResult CreateNewUser(Users user)
        {
            db.User.Add(user);
            db.SaveChanges();
            int userId = db.User.Max(item => item.userID);
            Teachers teacher = new Teachers();
            teacher.userID = userId;
            db.Teacher.Add(teacher);
            Admins admin = new Admins();
            admin.userID = userId;
            db.Admin.Add(admin);
            Students student = new Students();
            student.userID = userId;
            db.Student.Add(student);
            db.SaveChanges();
            return RedirectToAction("CreateNewUser");
        }
    }
}