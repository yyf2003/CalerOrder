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
    
    public partial class InstallPriceTemp
    {
        public int Id { get; set; }
        public Nullable<int> GuidanceId { get; set; }
        public Nullable<int> ShopId { get; set; }
        public Nullable<decimal> BasicPrice { get; set; }
        public Nullable<decimal> WindowPrice { get; set; }
        public Nullable<decimal> OOHPrice { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
        public Nullable<int> AddType { get; set; }
    }
}
