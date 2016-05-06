using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mooshak2.Models;
using System.ComponentModel.DataAnnotations;

namespace Mooshak2.Models
{
    public class Students
    {
        public int userID { get; set; }
        [Key]
        public int studentID { get; set; }
    }
}