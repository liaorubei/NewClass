//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace StudyOnline.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Document
    {
        public Document()
        {
            this.Comment = new HashSet<Comment>();
        }
    
        public int Id { get; set; }
        public int LevelId { get; set; }
        public string Title { get; set; }
        public string Lyrics { get; set; }
        public string Contents { get; set; }
        public string SoundPath { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
    
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual Level Level { get; set; }
    }
}
