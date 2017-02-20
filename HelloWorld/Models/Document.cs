//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace StudyOnline.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Document
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Document()
        {
            this.Comment = new HashSet<Comment>();
            this.Playlist = new HashSet<Playlist>();
        }
    
        public int Id { get; set; }
        public int LevelId { get; set; }
        public string Title { get; set; }
        public string Lyrics { get; set; }
        public string Contents { get; set; }
        public string SoundPath { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
        public Nullable<double> Duration { get; set; }
        public Nullable<double> Length { get; set; }
        public string TitleTwo { get; set; }
        public string LengthString { get; set; }
        public Nullable<int> FolderId { get; set; }
        public Nullable<System.DateTime> AuditDate { get; set; }
        public Nullable<int> AuditCase { get; set; }
        public Nullable<double> Sort { get; set; }
        public string TitleSubCn { get; set; }
        public string TitleSubEn { get; set; }
        public string TitleSubPy { get; set; }
        public Nullable<int> Category { get; set; }
        public string TitlePy { get; set; }
        public string Cover { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual Folder Folder { get; set; }
        public virtual Level Level { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Playlist> Playlist { get; set; }
    }
}
