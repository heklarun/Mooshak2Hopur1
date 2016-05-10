using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class UsersViewModels
    {
        public string userID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string confirmPassword { get; set; }

        public Boolean isTeacher { get; set; }
        public int teacherID { get; set; }
        public Boolean isStudent { get; set; }
        public int studentID { get; set; }
        public Boolean isAdmin { get; set; }
        public int adminID { get; set; }

        public Boolean selected { get; set; }

    }
}