using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class StudentGroup
    {
        [Key]
        public int studentGroupID { get; set; }
        public int studentID { get; set; }
        public int courseID { get; set; }
    }
}