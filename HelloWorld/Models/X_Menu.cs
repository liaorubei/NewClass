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
    
    public partial class X_Menu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public X_Menu()
        {
            this.X_Menu1 = new HashSet<X_Menu>();
            this.X_Role = new HashSet<X_Role>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> Order { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string Area { get; set; }
        public string Ctrl { get; set; }
        public string Action { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<X_Menu> X_Menu1 { get; set; }
        public virtual X_Menu X_Menu2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<X_Role> X_Role { get; set; }
    }
}
