using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudyOnline.Models.AndroidBean
{
    public class User
    {
        public int Id { get; set; }
        public string Accid { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Sign { get; set; }
        public string Email { get; set; }
        public DateTime Birth { get; set; }
        public string Mobile { get; set; }
        public int Gender { get; set; }
        public string Ex { get; set; }
        public string Country { get; set; }
        public string Language { get; set; }
        public string Job { get; set; }
        public string About { get; set; }
        public string Voice { get; set; }
    }
}