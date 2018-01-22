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
    
    public partial class ADOrderErrorCorrection
    {
        public int Id { get; set; }
        public Nullable<int> OrderId { get; set; }
        public Nullable<int> GuidanceId { get; set; }
        public int SubjectId { get; set; }
        public int ShopId { get; set; }
        public Nullable<int> OrderType { get; set; }
        public string Sheet { get; set; }
        public string Gender { get; set; }
        public Nullable<int> LevelNum { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> GraphicWidth { get; set; }
        public Nullable<decimal> GraphicLength { get; set; }
        public Nullable<decimal> Area { get; set; }
        public Nullable<int> CustomerMaterialId { get; set; }
        public string GraphicMaterial { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public string ChooseImg { get; set; }
        public string Remark { get; set; }
        public Nullable<int> AddUserId { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<int> Approved { get; set; }
        public Nullable<int> ApproveUserId { get; set; }
        public Nullable<System.DateTime> ApproveDate { get; set; }
        public string EditRemark { get; set; }
    }
}
