using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class TeacherGroup
    {
        [Key]
        public int teacherGroupID { get; set; }
        public int teacherID { get; set; }
        public int courseID { get; set; }
    }
}