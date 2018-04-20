using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Common
{
    public class EnumClass
    {
    }
    public enum CategoryTypeEnum
    {
        /// <summary>
        /// 权限操作类型
        /// </summary>
        OperateType = 1,
        /// <summary>
        /// 店铺类型
        /// </summary>
        ShopCategory,
    }

    public enum FileTypeEnum
    {
        Image,
        Files,
        PPT,
        PDF,
        Word,
        Excel,
        ZIP,
        RAR

    }

    public enum ShopStatusEnum
    {
        正常 = 1,
        关闭,
        //装修,
    }

    public enum FileCodeEnum
    {
        /// <summary>
        /// 项目报价单
        /// </summary>
        SubjectQuotation = 1,
        /// <summary>
        /// 原始订单
        /// </summary>
        OriginalOrder = 2,
        /// <summary>
        /// 活动指引附件
        /// </summary>
        SubjectGuidanceAttach = 3,
        /// <summary>
        /// 发货单
        /// </summary>
        SendInvoice = 4,
        /// <summary>
        /// 签收单
        /// </summary>
        ReceiveInvoice = 5,
        /// <summary>
        /// 安装前照片
        /// </summary>
        BeforeInstallImg = 6,
        /// <summary>
        /// 安装后照片
        /// </summary>
        AfterInstallImg = 7,
    }

    public enum TableLevelEnum
    {
        第一展桌 = 1,
        第二展桌,
        第三展桌,
        第四展桌,
        第五展桌
    }

    public enum LevelNumEnum
    {
        首选 = 1,
        次选,
        三选
    }

    public enum LowerUpperEnum
    {
        ToLower = 1,
        ToUpper,
    }

    public enum SubjectTypeEnum
    {
        [Description("POP订单")]
        POP订单 = 1,
        [Description("新开店安装费")]
        新开店安装费,//2
        [Description("上海补单")]
        补单,//3
        [Description("二次安装")]
        二次安装,//4
        /// <summary>
        /// 手工单，不需要拆单
        /// </summary>
        [Description("手工订单")]
        手工订单,//5
        [Description("分区反导")]
        分区补单,//6
        [Description("HC订单")]
        HC订单,//7
        [Description("分区新开店订单")]
        新开店订单,//8
        [Description("运费")]
        运费,//9
        [Description("百丽订单")]
        正常单,//10
        [Description("分区增补")]
        分区增补,//11
        [Description("费用订单")]
        费用订单,//12
        [Description("散单")]
        散单,//13
        [Description("外协订单1")]
        外协订单,//

    }

    public enum GuidanceTypeEnum
    {
        [Description("安装")]
        Install = 1,
        [Description("发货")]
        Delivery,
        [Description("快递(促销)")]
        Promotion,
        [Description("分区活动")]
        Others
        //[Description("安装和发货")]
        //InstallAndDelivery

    }

    /// <summary>
    /// 外协分单配置选项
    /// </summary>
    public enum OutsourceOrderConfigType
    {
        /// <summary>
        /// 材质选项
        /// </summary>
        [Description("材质")]
        Material = 1,
        /// <summary>
        /// 不算安装费
        /// </summary>
        [Description("不算安装费")]
        NoInstallPrice,
        /// <summary>
        /// 不算快递费
        /// </summary>
        [Description("不算快递费")]
        NoExpressPrice,



    }

    public enum RegionOrderPriceTypeEnum
    {
        [Description("安装费")]
        InstallPrice = 1,
        [Description("发货费")]
        DeliveryPrice,
        [Description("测量费")]
        MeasurePrice,


    }

    /// <summary>
    /// 下单区域
    /// </summary>
    public enum OrderChannelEnum
    {

        上海订单 = 1,
        分区订单,
    }

    public enum UserLevelEnum
    {
        总部 = 1,
        分区,
        省级,
        市级,
        县级
    }

    public enum CompanyTypeEnum
    {
        [Description("总公司")]
        ParentCompany = 1,
        [Description("分公司")]
        BranchCompany = 2,
        [Description("外协")]
        Outsource = 3
    }

    public enum OrderTypeEnum
    {
        [Description("POP")]
        POP = 1,
        [Description("POP")]
        道具,
        [Description("POP")]
        物料,
        [Description("费用订单")]
        安装费,
        [Description("费用订单")]
        发货费,
        [Description("费用订单")]
        测量费,
        [Description("费用订单")]
        其他费用,
        [Description("费用订单")]
        运费,
    }

    public enum OrderChangeTypeEnum
    {
        /// <summary>
        /// 补单
        /// </summary>
        [Description("编辑")]
        Edit = 1,
        /// <summary>
        /// 修改
        /// </summary>
        [Description("删除")]
        Delete,

    }

    /// <summary>
    /// 基础数据库修改项目
    /// </summary>
    public enum BaseDataChangeItemEnum
    {
        /// <summary>
        /// 店铺信息
        /// </summary>
        [Description("店铺信息")]
        Shop = 1,
        /// <summary>
        /// 器架信息
        /// </summary>
        [Description("器架信息")]
        ShopMachineFrame,
        /// <summary>
        /// pop信息
        /// </summary>
        [Description("POP信息")]
        POP
    }

    /// <summary>
    ///数据变更类型
    /// </summary>
    public enum DataChangeTypeEnum
    {
        /// <summary>
        /// 新增
        /// </summary>
        [Description("新增")]
        Add = 1,
        /// <summary>
        /// 修改
        /// </summary>
        [Description("修改")]
        Edit,
        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Delete,
    }

    public enum CornerTypeEnum
    {
        三叶草 = 1
    }

    public enum OutsourceOrderTypeEnum
    {
        [Description("安装")]
        Install = 1,
        [Description("发货")]
        Send,
    }

    public enum QuoteOrderSettingTypeEnum
    {
        [Description("百分比")]
        Percent = 1,
        [Description("数值")]
        Amount,
    }

    /// <summary>
    /// 二次安装费类型
    /// </summary>
    public enum SecondInstallInstallTypeEnum
    {
        [Description("全部150")]
        basic = 1,
        [Description("T1-T3级别150，T4-T7按实际算")]
        secondLevel = 2,
        [Description("无安装费/或单独导入")]
        thirdLevel = 3

    }

    /// <summary>
    /// 报价订单导出模板
    /// </summary>
    public enum QuoteOrderTemplateEnum
    {

        DaHuo = 1,
        SanYeCao = 2,
        TongDian = 3,
        Terrex = 4
    }

    //安装费项目类型
    public enum InstallPriceSubjectTypeEnum
    {
        活动安装费 = 1,
        常规安装费=2,
        
    }
}

public class EnumEntity
{
    public int Value { get; set; }
    public string Name { get; set; }
    public string Desction { get; set; }
}