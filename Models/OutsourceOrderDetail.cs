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
    
    public partial class OutsourceOrderDetail
    {
        public int Id { get; set; }
        public Nullable<int> AssignType { get; set; }
        public Nullable<int> GuidanceId { get; set; }
        public Nullable<int> OutsourceId { get; set; }
        public Nullable<int> SubjectId { get; set; }
        public Nullable<int> ShopId { get; set; }
        public string ShopNo { get; set; }
        public string ShopName { get; set; }
        public Nullable<int> OrderType { get; set; }
        public string Region { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string CityTier { get; set; }
        public string IsInstall { get; set; }
        public string BCSIsInstall { get; set; }
        public string AgentCode { get; set; }
        public string AgentName { get; set; }
        public string POPAddress { get; set; }
        public string Contact { get; set; }
        public string Tel { get; set; }
        public string Channel { get; set; }
        public string Format { get; set; }
        public string LocationType { get; set; }
        public string BusinessModel { get; set; }
        public string MaterialSupport { get; set; }
        public string POSScale { get; set; }
        public string MachineFrame { get; set; }
        public string GraphicNo { get; set; }
        public string POPName { get; set; }
        public string POPType { get; set; }
        public string Sheet { get; set; }
        public string Gender { get; set; }
        public string OrderGender { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> WindowWide { get; set; }
        public Nullable<decimal> WindowHigh { get; set; }
        public Nullable<decimal> WindowDeep { get; set; }
        public string WindowSize { get; set; }
        public Nullable<decimal> GraphicWidth { get; set; }
        public Nullable<decimal> GraphicLength { get; set; }
        public Nullable<decimal> Area { get; set; }
        public Nullable<decimal> TotalArea { get; set; }
        public string OrderGraphicMaterial { get; set; }
        public string GraphicMaterial { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public Nullable<decimal> ReceiveUnitPrice { get; set; }
        public Nullable<decimal> ReceiveTotalPrice { get; set; }
        public string ChooseImg { get; set; }
        public string Remark { get; set; }
        public string CornerType { get; set; }
        public string PositionDescription { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
        public Nullable<int> AddUserId { get; set; }
        public string PriceBlongRegion { get; set; }
        public string CheckRemark { get; set; }
        public string ShopStatus { get; set; }
        public Nullable<decimal> PayOrderPrice { get; set; }
        public Nullable<decimal> ReceiveOrderPrice { get; set; }
        public string InstallPriceMaterialSupport { get; set; }
        public Nullable<int> RegionSupplementId { get; set; }
        public Nullable<decimal> PayBasicInstallPrice { get; set; }
        public Nullable<decimal> PayOOHInstallPrice { get; set; }
        public Nullable<decimal> PayExpressPrice { get; set; }
        public Nullable<decimal> ReceiveExpresslPrice { get; set; }
        public Nullable<int> CSUserId { get; set; }
        public string ModifyType { get; set; }
        public Nullable<System.DateTime> ModifyDate { get; set; }
        public Nullable<int> ModifyUserId { get; set; }
        public string ModifyRemark { get; set; }
        public Nullable<int> FinalOrderId { get; set; }
        public Nullable<int> InstallPriceAddType { get; set; }
        public Nullable<int> InstallPriceSubjectType { get; set; }
        public Nullable<int> BelongSubjectId { get; set; }
        public Nullable<bool> IsFromRegion { get; set; }
    }
}
