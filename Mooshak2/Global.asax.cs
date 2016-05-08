using Mooshak2.Models;
using SecurityWebAppTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;


namespace Mooshak2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            IdentityManager manager = new IdentityManager();
            if (!manager.RoleExists("Admin"))
            {
                manager.CreateRole("Admin");
            }
            if (!manager.RoleExists("Student"))
            {
                manager.CreateRole("Student");
            }
            if (!manager.RoleExists("Teacher"))
            {
                manager.CreateRole("Teacher");
            }

            if (!manager.UserExists("admin"))
            {
                ApplicationUser newAdmin = new ApplicationUser();
                newAdmin.UserName = "admin";
                manager.CreateUser(newAdmin, "123456");
                manager.AddUserToRole(newAdmin.Id, "Admin");
            }

        }
    }
}
