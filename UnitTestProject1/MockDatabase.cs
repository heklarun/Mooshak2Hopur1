using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mooshak2.Data;
using FakeDbSet;
using System.Data.Entity;
using Mooshak2.Service;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mooshak2.DAL;
using Mooshak2.test;

namespace Mooshak.test
{
    /// <summary>
    /// This is an example of how we'd create a fake database by implementing the 
    /// same interface that the BookeStoreEntities class implements.
    /// </summary>
    public class FakeDatabase
    {
        /// <summary>
        /// Sets up the fake database.
        /// </summary>
        /// 
        private readonly Models.IAppDataContext db;

        public FakeDatabase() 
        {
            // We're setting our DbSets to be InMemoryDbSets rather than using SQL Server.
            this.Project = new DbSet<Project>();
           // this.Books = new InMemoryDbSet<Book>();
        }

        //public IDbSet<Author> Authors { get; set; }
        //public IDbSet<Book> Books { get; set; }
        //public InMemoryDbSet<Project> Project { get; private set; }

        public int SaveChanges()
        {
            // Pretend that each entity gets a database id when we hit save.
            int changes = 0;
            //changes += DbSetHelper.IncrementPrimaryKey<Author>(x => x.AuthorId, this.Authors);
            //changes += DbSetHelper.IncrementPrimaryKey<Book>(x => x.BookId, this.Books);

            return changes;
        }

        public void Dispose()
        {
            // Do nothing!
        }
    }
}
