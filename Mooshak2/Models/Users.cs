using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public List<Courses> Course { get; set; }
        public List<Projects> Project { get; set; }
        public DateTime DueDate { get; set; }
        
        public List<string> Admins { get; set; }
        public List<string> Teachers { get; set; }
        public List<string> Students { get; set; }
    }
}