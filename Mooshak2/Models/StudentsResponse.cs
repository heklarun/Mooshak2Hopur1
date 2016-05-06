using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class StudentsResponse
    {
        public int studentResponseID { get; set; }
        Students studentID { get; set; }
        Response responseID { get; set; }
    }
}