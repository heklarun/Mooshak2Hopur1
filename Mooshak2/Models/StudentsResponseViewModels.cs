using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mooshak2.Models
{
    public class StudentsResponseViewModels
    {
        public int studentsResponseID { get; set; }
        public string userID { get; set; }
        public int responseID { get; set; }
    }
}