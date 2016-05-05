﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mooshak2.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult AdminIndex()
        {
            return View("AdminIndex");
        }

        public ActionResult CreateNewCourse()
        {
            return View("CreateNewCourse");
        }

        public ActionResult CreateNewUser()
        {
            return View("CreateNewUser");
        }
    }
}