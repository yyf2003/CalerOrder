<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StatisticDetail.aspx.cs"
    Inherits="WebApp.Statistics.InOutStatistics.StatisticDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <script type="text/javascript">
        var quoteItemId = '<%=quoteItemId %>';
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <a href="/index.aspx">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /></a><p
                class="nav_table_p">
                项目收入/支出统计—统计明细
            </p>
    </div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                活动名：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labGuidanceName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目名称：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labQuoteSubjectName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目类型：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labQuoteSubjectCategoryName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
               
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <span id="checkQuote" style="color:Blue; text-decoration:underline;cursor:pointer;">查看报价统计单</span> 
            </td>
        </tr>
    </table>
    <div class="tr" style="margin-top: 20px;">
        》统计信息</div>
    <table class="table" id="table1">
        <tr class="tr_hui" style="font-weight: bold;">
            <td style="height: 35px; font-weight: bold; color: blue; width: 120px;">
                系统订单
            </td>
            <td>
                POP面积
            </td>
            <td>
                POP制作费
            </td>
            <td>
                安装费
            </td>
            <td>
                快递费
            </td>
            <td>
                物料费
            </td>
            <td>
                其他
            </td>
            <td>
                合计
            </td>
        </tr>
        <tr class="tr_bai" style="font-weight: bold; font-size: 14px;">
            <td>
                全部
            </td>
            <td>
                <asp:Label ID="labOrderArea" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labOrderPOPPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labOrderInstall" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labOrderExpress" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labOrderMaterial" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labOrderOther" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labOrderSub" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="font-weight: bold;">
                实际生产(外协)
            </td>
            <td>
                <asp:Label ID="labProduceArea" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labProducePOPPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labProduceInstall" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labProduceExpress" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labProduceMaterial" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labProduceOther" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labProduceSub" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="font-weight: bold;">
                闭店
            </td>
            <td>
                <asp:Label ID="labShutArea" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labShutPOPPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labShutInstall" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labShutExpress" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labShutMaterial" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labShutOther" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labShutSub" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="font-weight: bold;">
                非闭店不生产
            </td>
            <td>
                <asp:Label ID="labNonProduceArea" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labNonProducePOPPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labNonProduceInstall" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labNonProduceExpress" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labNonProduceMaterial" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labNonProduceOther" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labNonProduceSub" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        
        <tr>
            <td class="tr_hui" style="height: 35px; font-weight: bold; color: blue;">
                实际报价
            </td>
            <td colspan="7">
            </td>
        </tr>
       
        <tr class="tr_bai" style="font-weight: bold; font-size: 14px;">
            <td>
                总报价
            </td>
            <td>
                <asp:Label ID="labTotalArea" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labTotalPOPPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labTotalInstallPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labTotalExpressPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labTotalMaterialPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labTotalOtherPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labTotalSub" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="font-weight: bold;">
                阿迪增补
            </td>
            <td>
                <asp:Label ID="labSupplementArea" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labSupplementPOPPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labSupplementInstallPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labSupplementExpressPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labSupplementMaterialPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labSupplementOtherPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labSupplementSub" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="font-weight: bold;">
                卡乐添加(无订单)
            </td>
            <td>
                <asp:Label ID="labSelfAddArea" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labSelfAddPOPPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labSelfAddInstallPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labSelfAddExpressPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labSelfAddMaterialPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labSelfAddOtherPrice" runat="server" Text="0"></asp:Label>
            </td>
            <td>
                <asp:Label ID="labSelfAddSub" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
    </table>
    <div style=" text-align:center; margin-top:30px; margin-bottom:30px;">
       <input type="button" value="返 回" id="btnGoBack" class="layui-btn" onclick="javascript:window.history.go(-1);"/>
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    $(function () {
        $("#table1").delegate("span[name='checkSelfAddDetailSpan']", "click", function () {
            var url = "CheckSelfAddDetail.aspx?quoteItemId=" + quoteItemId;
            layer.open({
                type: 2,
                time: 0,
                title: '查看报价明细—卡乐添加',
                skin: 'layui-layer-rim', //加上边框
                area: ['95%', '90%'],
                content: url,
                id: 'layer1'

            });
        });

        $("#checkQuote").click(function () {
            var url = "/QuoteOrderManager/CheckQuotation.aspx?itemId=" + quoteItemId;
            layer.open({
                type: 2,
                time: 0,
                title: '查看报价单',
                skin: 'layui-layer-rim', //加上边框
                area: ['95%', '90%'],
                content: url,
                id: 'layer1'
            });
        })
    })
</script>
