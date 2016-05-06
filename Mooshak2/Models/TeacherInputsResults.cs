using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class TeacherInputsResults
    {
        SubProjects h;
        public int userID { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string confirmPassword { get; set; }

    }
}