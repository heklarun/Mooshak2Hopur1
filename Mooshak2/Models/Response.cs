﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mooshak2.Models
{
    public class Response
    {
        [Key]
        public int responseID { get; set; }
        public int projectID { get; set; }
    }
}