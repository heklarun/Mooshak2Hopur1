using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class Projects
    {
        public int projectID { get; set; }
        public string projectName { get; set; }
        Courses courseID { get; set; }
        //open?
        //close?

        public DateTime dueDate { get; set; }

        //public List<string> Admins { get; set; }
        //public List<string> Teachers { get; set; }
        //public List<string> Students { get; set; }
    }
}