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
    
    public partial class OrderPlan
    {
        public int Id { get; set; }
        public Nullable<int> PlanType { get; set; }
        public Nullable<int> CheckOrderId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> SubjectId { get; set; }
        public string ShopNos { get; set; }
        public string NotInvolveShopNos { get; set; }
        public Nullable<bool> IsExcept { get; set; }
        public string RegionId { get; set; }
        public string RegionNames { get; set; }
        public string ProvinceId { get; set; }
        public string CityId { get; set; }
        public string CityTier { get; set; }
        public string IsInstall { get; set; }
        public Nullable<int> PositionId { get; set; }
        public string PositionName { get; set; }
        public string MachineFrameIds { get; set; }
        public string MachineFrameNames { get; set; }
        public string Format { get; set; }
        public string ShopLevel { get; set; }
        public string MaterialSupport { get; set; }
        public string POSScale { get; set; }
        public string Gender { get; set; }
        public string Quantity { get; set; }
        public string GraphicNo { get; set; }
        public string GraphicMaterial { get; set; }
        public string GraphicWidth { get; set; }
        public string GraphicLength { get; set; }
        public string WindowWidth { get; set; }
        public string WindowHigh { get; set; }
        public string WindowDeep { get; set; }
        public string ChooseImg { get; set; }
        public Nullable<System.DateTime> BeginDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> AddUserId { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
        public string CornerType { get; set; }
        public Nullable<bool> KeepPOPSize { get; set; }
        public string POPSize { get; set; }
        public string WindowSize { get; set; }
        public string IsElectricity { get; set; }
    }
}
