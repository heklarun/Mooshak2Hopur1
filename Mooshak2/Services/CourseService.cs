using Mooshak2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Mooshak2.DAL
{
    public class CourseService
    {

        public List<Courses> GetAllCourses()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            List<Courses> result = (from item in db.Course
                                    select item).ToList();
            return result;
        }

       
    }

}