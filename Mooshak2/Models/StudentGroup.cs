﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class StudentGroup
    {
        public int studentGroupID { get; set; }
        Students studentID { get; set; }
        Courses courseID { get; set; }
    }
}