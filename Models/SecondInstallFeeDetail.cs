//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SecondInstallFeeDetail
    {
        public int Id { get; set; }
        public Nullable<int> SubjectId { get; set; }
        public Nullable<int> ShopId { get; set; }
        public string Sheet { get; set; }
        public Nullable<decimal> GraphicWidth { get; set; }
        public Nullable<decimal> GraphicLength { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string Remark { get; set; }
    }
}
