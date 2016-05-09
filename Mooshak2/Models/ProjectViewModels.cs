using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class ProjectViewModels
    {
        [Key]
        public int projectID { get; set; }
        public string projectName { get; set; }
        Courses courseID { get; set; }
    }
}