using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudyOnline.Models
{
    public class Menu
    {
        public String Id { get; set; }
        public String ParentId { get; set; }
        public String Name { get; set; }
        public List<Menu> Mecuc { get; set; }
    }
}