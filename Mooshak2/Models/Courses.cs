﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class Courses
    {
        [Key]
        public int courseID { get; set; }
        public string courseName { get; set; }
    }
}