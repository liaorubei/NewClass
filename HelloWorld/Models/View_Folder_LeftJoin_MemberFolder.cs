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
    
    public partial class View_Folder_LeftJoin_MemberFolder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public string NameSubCn { get; set; }
        public string NameSubEn { get; set; }
        public Nullable<int> Sort { get; set; }
        public Nullable<int> Show { get; set; }
        public string Cover { get; set; }
        public Nullable<int> LevelId { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string MemberId { get; set; }
        public Nullable<int> DocsCount { get; set; }
        public Nullable<int> KidsCount { get; set; }
        public Nullable<int> TargetId { get; set; }
    }
}