using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mooshak2.Models
{
    public class ProjectViewModels
    {
        [Key]
        public int projectID { get; set; }
        public string projectName { get; set; }
        public int courseID { get; set; }
        public SelectList selectedCourse { get; set; }
        public List<CoursesViewModels> courses { get; set; }
    }
}