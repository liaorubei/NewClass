//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace StudyOnline.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Level
    {
        public Level()
        {
            this.Document = new HashSet<Document>();
        }
    
        public int Id { get; set; }
        public string LevelName { get; set; }
    
        public virtual ICollection<Document> Document { get; set; }
    }
}
