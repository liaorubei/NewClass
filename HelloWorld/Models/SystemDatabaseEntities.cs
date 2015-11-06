using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace StudyOnline.Models
{
    public class SystemDatabaseEntities : DbContext
    {
        public SystemDatabaseEntities() : base("DefaultConnectionss") { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }

    public class SystemEntities : DbContext
    {
        static SystemEntities()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<SystemEntities>());
        }

        
        public DbSet<Menu> Menu { get; set; }

    }


}