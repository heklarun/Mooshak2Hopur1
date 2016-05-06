using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class Admins
    {
        [Key]
        public int adminID { get; set; }
        public int userID { get; set; }
    }
}