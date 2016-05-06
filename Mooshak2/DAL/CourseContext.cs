using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Mooshak2.DAL
{
    public class CourseContext : DbContext //Db context er partur af entityinu
    {
        public CourseContext() : base("CourseContext")
        {

        }
        //public DbSet<Courses> Course { get; set; }
    }

}