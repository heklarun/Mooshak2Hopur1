﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class Teachers
    {
        public int userID { get; set; }
        [Key]
        public int teacherID { get; set; }
    }
}