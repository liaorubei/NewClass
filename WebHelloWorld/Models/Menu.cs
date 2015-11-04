using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHelloWorld.Models
{
    public class Menu
    {
        public Menu()
        {
            Children = new List<Menu>();
        }
        public int Id { get; set; }
        public String Name { get; set; }
        public Nullable<Int32> Parent_Id { get; set; }

        public Menu Parent { get; set; }
        public List<Menu> Children { get; set; }
    }
}