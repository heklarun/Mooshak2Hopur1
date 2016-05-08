using Mooshak2.Models;
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

        public CoursesViewModels GetCourseByID(int? courseID)
        {
            CoursesViewModels course = (from item in db.Course
                                     where item.courseID == courseID
                                     select new CoursesViewModels
                                     {
                                         courseID = item.courseID,
                                         courseName = item.courseName
                                     }).SingleOrDefault();
            return course;
        }

        public void EditCourse(CoursesViewModels course)
        {
            Courses courseToEdit = db.Course.Single(u => u.courseID == course.courseID);
            courseToEdit.courseName = course.courseName;
            db.SaveChanges();

        }

    }

}