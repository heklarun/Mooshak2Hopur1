using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class Users
    {
        [Key]
        public int userID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        //public string confirmPassword { get; set; }

        //public List<Courses> Course { get; set; }
        //public List<Projects> Project { get; set; }
        //public DateTime dueDate { get; set; }
        
        //public List<string> Admins { get; set; }
        //public List<string> Teachers { get; set; }
        //public List<string> Students { get; set; }
    }
}