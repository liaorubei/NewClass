using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudyOnline.Models
{
    public class Menu
    {
        public Menu()
        {
            this.Children = new HashSet<Menu>();
        }

        public Int32 Id { get; set; }
        public String Name { get; set; }
        public Int32? ParentId { get; set; }
        public Int32? Order { get; set; }
        public String Controller { get; set; }
        public String Action { get; set; }
        public String Class { get; set; }
        public String Title { get; set; }
        public String Target { get; set; }
        public Int32 Width { get; set; }
        public Int32 Height { get; set; }




        public virtual Menu Parent { get; set; }
        public virtual ICollection<Menu> Children { get; set; }
    }
}