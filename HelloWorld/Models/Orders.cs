//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace StudyOnline.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Orders
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public string Main { get; set; }
        public string Body { get; set; }
        public string TradeNo { get; set; }
        public string TradeStatus { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
    }
}
