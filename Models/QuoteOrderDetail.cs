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
    
    public partial class QuoteOrderDetail
    {
        public int Id { get; set; }
        public Nullable<int> GuidanceId { get; set; }
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
        public Nullable<decimal> AutoAddGraphicWidth { get; set; }
        public Nullable<decimal> AutoAddGraphicLength { get; set; }
        public Nullable<decimal> AutoAddArea { get; set; }
        public Nullable<decimal> TotalGraphicWidth { get; set; }
        public Nullable<decimal> TotalGraphicLength { get; set; }
        public Nullable<decimal> TotalArea { get; set; }
        public Nullable<int> CustomerMaterialId { get; set; }
        public string GraphicMaterial { get; set; }
        public string QuoteGraphicMaterial { get; set; }
        public string UnitName { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> DefaultTotalPrice { get; set; }
        public Nullable<decimal> AutoAddTotalPrice { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public string ChooseImg { get; set; }
        public string Remark { get; set; }
        public string IsElectricity { get; set; }
        public string LeftSideStick { get; set; }
        public string RightSideStick { get; set; }
        public string Floor { get; set; }
        public string WindowStick { get; set; }
        public string IsHang { get; set; }
        public string DoorPosition { get; set; }
        public string CornerType { get; set; }
        public string PositionDescription { get; set; }
        public Nullable<int> SmallMaterialId { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public string Category { get; set; }
        public string InstallPositionDescription { get; set; }
        public string InstallPricePOSScale { get; set; }
        public Nullable<int> RegionSupplementId { get; set; }
        public Nullable<System.DateTime> DeleteDate { get; set; }
        public Nullable<int> DeleteUserId { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
        public Nullable<int> AddUserId { get; set; }
        public string InstallPriceMaterialSupport { get; set; }
        public string PriceBlongRegion { get; set; }
        public Nullable<bool> IsValid { get; set; }
        public Nullable<bool> IsFromRegion { get; set; }
        public string ShopStatus { get; set; }
        public Nullable<decimal> OrderPrice { get; set; }
        public Nullable<decimal> PayOrderPrice { get; set; }
        public Nullable<int> CSUserId { get; set; }
        public Nullable<int> FinalOrderId { get; set; }
    }
}
