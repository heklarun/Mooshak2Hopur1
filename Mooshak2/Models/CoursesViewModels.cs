using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class CoursesViewModels
    {
            [Key]
            public int courseID { get; set; }
            public string courseName { get; set; }
            public List<UsersViewModels> students { get; set; }
            public List<UsersViewModels> teachers { get; set; }
        }
}