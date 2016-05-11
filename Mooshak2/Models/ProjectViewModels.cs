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
        public string courseName { get; set; }
        public List<SubProjectsViewModels> subProjects { get; set; }
        public DateTime open { get; set; }
        public string openDate { get; set; }
        public DateTime close { get; set; }
        public string closeDate { get; set; }
        public Boolean canHandIn { get; set; }
    }
}