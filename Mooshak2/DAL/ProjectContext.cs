﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Mooshak2.DAL
{
    //Klasi sem tengist gagnagrunninum
    public class ProjectContext : DbContext //Db context er partur af entityinu
    {
        public ProjectContext() : base("ProjectContext")
        {

        }
        //public DbSet<Projects> Project { get; set; }

    }
}