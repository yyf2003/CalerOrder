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
    
    public partial class SpecialPriceQuoteDetail
    {
        public int Id { get; set; }
        public Nullable<int> ItemId { get; set; }
        public Nullable<int> ChangeType { get; set; }
        public string Sheet { get; set; }
        public string PositionDescription { get; set; }
        public string GraphicMaterial { get; set; }
        public Nullable<decimal> GraphicMaterialUnitPrice { get; set; }
        public Nullable<decimal> AddArea { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> InstallPriceLevel { get; set; }
        public Nullable<int> AddUserId { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
    }
}
