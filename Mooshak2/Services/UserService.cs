using Mooshak2.Models;
using SecurityWebAppTest.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Mooshak2.DAL
{
    //Klasi sem tengist gagnagrunninum
    public class UserService
    {
        public ApplicationUser GetUserById(string username)
        {
            IdentityManager man = new IdentityManager();
            return man.GetUser(username);
        }
    }
}