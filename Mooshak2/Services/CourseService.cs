﻿using Mooshak2.Models;
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

    }

}