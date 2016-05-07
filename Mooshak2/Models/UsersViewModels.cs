using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class UsersViewModels
    {
        [Key]
        public int userID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string confirmPassword { get; set; }

        public Boolean isTeacher;
        public Boolean isStudent;
        public Boolean isAdmin;
    }
}