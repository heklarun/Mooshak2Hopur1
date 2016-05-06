using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Mooshak2.DAL
{
    //Klasi sem tengist gagnagrunninum
    public class UserContext : DbContext //Db context er partur af entityinu
    {
        public ProductContext() : base("UserContext")
        {

        }
        public DbSet<Users> User { get; set; }
    }
}