using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class Courses
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }

        public List<Projects> Project { get; set; }

        public List<Teachers> Teachers { get; set; }
        public List<Students> Students { get; set; }
    }
}