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
    
    public partial class Teacherreginfo
    {
        public int ID { get; set; }
        public string Accid { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public System.DateTime CreateDate { get; set; }
        public decimal Category { get; set; }
        public decimal IsOnline { get; set; }
        public decimal IsActive { get; set; }
        public decimal IsEnable { get; set; }
        public decimal Refresh { get; set; }
        public decimal Enqueue { get; set; }
        public string Truename { get; set; }
        public string Phonenumber { get; set; }
        public string sex { get; set; }
        public string Cardnumber { get; set; }
        public string Education { get; set; }
        public string ForeignLanguages { get; set; }
        public string Note { get; set; }
        public Nullable<int> IsSync { get; set; }
    }
}
