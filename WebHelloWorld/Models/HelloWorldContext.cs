using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WebHelloWorld.Models
{
    public class HelloWorldContext : DbContext
    {
        public DbSet<Menu> Menu { get; set; }
    }
}