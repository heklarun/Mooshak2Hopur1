using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class Users
    {
        public int userID { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string confirmPassword { get; set; }

        public List<Courses> Course { get; set; }
        public List<Projects> Project { get; set; }
        public DateTime dueDate { get; set; }
        
        public List<string> Admins { get; set; }
        public List<string> Teachers { get; set; }
        public List<string> Students { get; set; }
    }
}