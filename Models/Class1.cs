using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public partial class UserInfo
    {
        public string Roles { get; set; }
        public string Customers { get; set; }
        public string Activities { get; set; }
        public string Regions { get; set; }
    }

    public partial class Customer
    {
        public List<Region> Regions { get; set; }
    }

    public partial class Region
    {
        public string ProvinceIds { get; set; }
    }

    public partial class OrderPlan
    {
        public List<SplitOrderPlanDetail> SplitOrderPlanDetail { get; set; }
        public List<CheckOrderPlanDetail> CheckOrderPlanDetail { get; set; }
    }

    public partial class CustomerMaterial
    {
        public List<CustomerMaterialDetail> Materials { get; set; }
    }

    public partial class ShopMachineFrame
    {
        public string POSCode { get; set; }
    }

    public partial class POP
    {
        public string ShopNo { get; set; }
        public int PriceItemId { get; set; }
        public int? OutsourceType { get; set; }
        public int? CustomerId { get; set; }
        
        
    }

    public partial class ADOrderErrorCorrection
    {
        public string ShopNo { get; set; }
    }

    public partial class Subject
    {
        public string CustomerName { get; set; }
    }

    [Serializable]
    public partial class FinalOrderDetailTemp
    {
        //public string UnitName { get; set; }
        public int RealSubjectId { get; set; }
    }

    public partial class OrderPriceDetail
    {
        public string PriceType { get; set; }
        public string SubjectNo { get; set; }
        public string SubjectName { get; set; }
        public DateTime? AddDate { get; set; }
        public string ShopNo { get; set; }
        public string ShopName { get; set; }
        public string Region { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Sheet { get; set; }
        public string PositionDescription { get; set; }
        public string Gender { get; set; }
        public string GraphicMaterial { get; set; }
        public double GraphicWidth { get; set; }
        public double GraphicLength { get; set; }
        public double Area { get; set; }
        public double UnitPrice { get; set; }
        public double ReceiveUnitPrice { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public double ReceiveTotalPrice { get; set; }
        public string Remark { get; set; }
        public string GuidanceName { get; set; }
        public string OutsourceName { get; set; }
        public string Channel { get; set; }
        public string Format { get; set; }
        public string CustomServiceName { get; set; }
    }

    public partial class Shop {
        public decimal OOHInstallPrice { get; set; }
        public int? GuidanceId { get; set; }
        public string GuidanceName { get; set; }
        public int? SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string Sheet { get; set; }
        public decimal ReceiveInstallPrice { get; set; }
        public decimal ExpressPrice { get; set; }
        public decimal ReceiveExpressPrice { get; set; }
        //pop金额
        public decimal POPPrice { get; set; }
        //pop总面积
        public decimal POPArea { get; set; }
        //测量费
        public decimal MeasurePrice { get; set; }
        //物料/道具费用
        public decimal MaterialPrice { get; set; }
        //其他费
        public decimal OtherPrice { get; set; }
        public string CSUserName { get; set; }
    }

    public partial class NewMaterialSupport {
        public decimal BasicInstallPrice { get; set; }
        public decimal WindowInstallPrice { get; set; }
        public decimal OutsourceInstallPrice { get; set; }
    }

    public partial class Company
    {
        public string Regions { get; set; }
    }

    public partial class OrderMaterial
    {
        public string SubjectName { get; set; }
        public string ShopName { get; set; }
        public string ShopNo { get; set; }

    }

    public partial class MergeOriginalOrder
    {
        public string SubjectName { get; set; }
       
    }

    public partial class CustomerMaterialPriceItem
    {
        public List<CustomerMaterialInfo> Materials { get; set; }
    }

    public partial class OutsourceMaterialPriceItem
    {
        public List<OutsourceMaterialInfo> Materials { get; set; }
    }

    public partial class HCSmallGraphicSize
    {
        public string GraphicSizes { get; set; }
    }

    public partial class CustomerOrderType
    {
        public string OrderTypeIds { get; set; }
    }

    public partial class RegionOrderDetail
    {
        public string OrderTypeName { get; set; }
        public int? CustomerId { get; set; }
    }

    public partial class ExtraOrderDetail
    {
        public string OrderTypeName { get; set; }
        public int? CustomerId { get; set; }
    }

    public partial class CustomerMaterialInfo
    {
        /// <summary>
        /// 应付安装单价
        /// </summary>
        public decimal? PayPriceInstall { get; set; }
        /// <summary>
        /// 应付生产+安装单价
        /// </summary>
        public decimal? PayPriceInstallAndProduct { get; set; }
        /// <summary>
        /// 应付发货单价
        /// </summary>
        public decimal? PayPriceSend { get; set; }
        /// <summary>
        /// 对应外协材质表的ID
        /// </summary>
        public int? OutsourceMaterialId { get; set; }
    }

    public partial class InstallPriceShopInfo {
        public decimal? PayPrice { get; set; }
    }

    public partial class ExpressPriceDetail
    {
        public decimal? PayPrice { get; set; }
    }

    public partial class PropOrderDetail {
        public Nullable<int> PropOrderId { get; set; }
        public string PayPackaging { get; set; }
        public string OutsourceName { get; set; }
        public Nullable<int> PayQuantity { get; set; }
        public Nullable<decimal> PayUnitPrice { get; set; }
        public string PayUnitName { get; set; }
        public string PayRemark { get; set; }
        public string PayMaterialName { get; set; }
    }


    /// <summary>
    /// 与ERP对接的订单模型
    /// </summary>
    public partial class CalerOrderModel
    {
        public int Id { get; set; }
        public string OrderName { get; set; }
        public string OrderNo { get; set; }
        public string GuidanceName { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ShopName { get; set; }
        public string ShopNo { get; set; }
        public string AgentName { get; set; }
        public string AgentCode { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Sheet { get; set; }
        public string MaterialName { get; set; }
        public string Gender { get; set; }
        public int Quantity { get; set; }
        public int GraphicWidth { get; set; }
        public int GraphicLength { get; set; }
        public string PositionDescription { get; set; }
        public string Category { get; set; }
        public string ChooseImg { get; set; }
        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyNo { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        public string CustomerNo { get; set; }

    }

    public class OutsourceAssignOrderModel
    {
        public int GuidanceId { get; set; }
        public int GuidanceType { get; set; }
        /// <summary>
        /// 生产外协
        /// </summary>
        public int OutsourceId { get; set; }
        /// <summary>
        /// 安装外协
        /// </summary>
        public int InstallOutsourceId { get; set; }
        public int CustomerId { get; set; }
        public int AssignType { get; set; }
        public int SubjectId { get; set; }
        public string SubjectCornerType { get; set; }
        public string SubjectCategoryName { get; set; }
        public Shop Shop { get; set; }
        public List<FinalOrderDetailTemp> OrderList { get; set; }
        public List<OutsourceOrderDetail> AssignedOrderList { get; set; }
        public List<string> BCSCityTierList { get; set; }
        public bool HasExpressPrice { get; set; }
        public string MaterialAssign { get; set; }
        public string MaterialPlan { get; set; }
        public bool AddInstallPrice { get; set; }
        public bool AddExperssPrice { get; set; }
        public decimal OOHInstallPrice { get; set; }
        public SubjectGuidance Guidance { get; set; }
        public bool IsInstallShop { get; set; }
        //三叶草T1-T3安装费
        public decimal BCSBasicInstallPrice { get; set; }
        //是否三叶草项目（单店全部订单）
        public bool IsBCSSubject { get; set; }
        //是否是单独非常规项目（单店全部订单）
        public bool IsGenericSubject { get; set; }

    }

    public class MaterialClass
    {
        public string MaterialName { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal Area { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string UnitName { get; set; }
    }
    /// <summary>
    /// POP报价模板导出模型
    /// </summary>
    public class QuoteModel {
        public string Sheet { get; set; }
        public string PositionDescription { get; set; }
        public string QuoteGraphicMaterial { get; set; }
        public decimal Amount { get; set; }
        public decimal UnitPrice { get; set; }
        public string UnitName { get; set; }
        public decimal TotalPrice { get; set; }
    }
    /// <summary>
    /// 安装费报价模板导出模型
    /// </summary>
    public class InstallPriceQuoteModel
    {
        /// <summary>
        /// 费用名称
        /// </summary>
        public string ChargeItem { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string ChargeType { get; set; }
        public decimal Amount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class ExportPermissionContent {
        public int UserId { get; set; }
        public string Channel { get; set; }
        public string Format { get; set; }
    }

}
