using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class StudentsResponse
    {
        [Key]
        public int studentsResponseID { get; set; }
        public string userID { get; set; }
        public int responseID { get; set; }
    }
}