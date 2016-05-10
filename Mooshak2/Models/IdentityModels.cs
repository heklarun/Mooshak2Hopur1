using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Mooshak2.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string firstName { get; set; }
        public string lastName { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Projects> Project { get; set; }

        public DbSet<Courses> Course { get; set; }
        public DbSet<Users> User { get; set; }
        public DbSet<Teachers> Teacher { get; set; }

        public DbSet<Admins> Admin { get; set; }
        public DbSet<Students> Student { get; set; }
        public DbSet<TeacherGroup> TeacherGroup { get; set; }
        public DbSet<StudentGroup> StudentGroup { get; set; }
        public DbSet<SubProjects> SubProjects { get; set; }
        public DbSet<PartResponse> PartResponse { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}