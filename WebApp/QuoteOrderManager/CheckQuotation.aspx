<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckQuotation.aspx.cs"
    Inherits="WebApp.QuoteOrderManager.CheckQuotation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <style type="text/css">
        .numberColor
        {
            color: Blue;
            font-weight: bold;
        }
        .differenceColor1
        {
            color: green;
            font-weight: bold;
        }
        .differenceColor2
        {
            color: red;
            font-weight: bold;
        }
        #container
        {
           margin-bottom:40px;
        }
        #quoteFooter
        {
          position:fixed;
          width:100%;
          height:40px;
          bottom:0;	
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="container">
    <div>
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 120px;">
                    活动月份
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labGuidanceMonth" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    活动名称
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labGuidanceName" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    项目类型
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labSubjectCategory" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    项目名称
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labSubjectNames" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    VVIP店铺
                </td>
                <td style="width: 200px; text-align: left; padding-left: 5px;">
                    <asp:Label ID="labVVIPShop" runat="server" Text=""></asp:Label>
                </td>
                <td style="width: 120px;">
                    安装店铺
                </td>
                <td style="width: 200px; text-align: left; padding-left: 5px;">
                    <asp:Label ID="labInstallShop" runat="server" Text=""></asp:Label>
                </td>
                <td style="width: 120px;">
                    常规店铺
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labNormalShop" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px; height: 40px;">
                    <asp:Button ID="btnExport" runat="server" Text="导出报价单" class="layui-btn layui-btn-small"
                        OnClick="btnExport_Click" />
                </td>
            </tr>
        </table>
    </div>
    <div class="tr" style="margin-top: 20px;">
        》POP订单统计
    </div>
    <asp:Repeater ID="popList" runat="server" OnItemDataBound="popList_ItemDataBound">
        <HeaderTemplate>
            <table class="table">
                <%--<tr class="tr_hui">
                    <td>
                        画面位置
                    </td>
                    <td>
                        画面名称/描述
                    </td>
                    <td>
                        画面材质/安装物料
                    </td>
                    <td>
                        单位
                    </td>
                    <td>
                        画面尺寸/物料数量
                    </td>
                    <td>
                        单价
                    </td>
                    <td>
                        金额
                    </td>
                </tr>--%>
                <tr class="tr_hui">
                    <td rowspan="2">
                        画面位置
                    </td>
                    <td colspan="2" style="font-weight: bold;">
                        系统
                    </td>
                    <td colspan="6" style="font-weight: bold;">
                        实际报价
                    </td>
                </tr>
                <tr class="tr_hui">
                    <td style="width: 80px;">
                        尺寸/数量
                    </td>
                    <td>
                        金额
                    </td>
                    <td>
                        尺寸调整(%)
                    </td>
                    <td>
                        新增金额
                    </td>
                    <td>
                        尺寸/数量
                    </td>
                    <td>
                        总金额
                    </td>
                    <td>
                        与系统差额
                    </td>
                    <td style="color: Blue;">
                        其他报价项目(非订单)
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="tr_bai">
                <td runat="server" id="sheet" style="height: 30px;">
                    <span name="popSheetSpan" style="float: left; margin-left: 20px; cursor: pointer;
                        text-decoration: underline; color: Blue;">
                        <%#Eval("Sheet") %></span>
                </td>
                <td>
                    <%--画面尺寸/物料数量--%>
                    <%-- <asp:Label ID="labQuantity" runat="server" Text=""></asp:Label>--%>
                    <%#Eval("Amount")%>
                </td>
                <td>
                    <%--金额--%>
                    <%--<asp:Label ID="labSubPrice" runat="server" Text=""></asp:Label>--%>
                    <%#Eval("TotalPrice") != null?(Math.Round(decimal.Parse(Eval("TotalPrice").ToString()),2)):0%>
                </td>
                <td>
                    <asp:Label ID="labAddRate" runat="server" Text="0"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="labAddRatePrice" runat="server" Text="0"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="labImportArea" runat="server" Text="0"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="labImportPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td class="differenceTClass">
                    <asp:Label ID="labDifference" runat="server" Text="0"></asp:Label>
                </td>
                <td>
                    <table id="AddExtendPOPPriceTable" runat="server" style="width: 100%;">
                    </table>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (popList.Items.Count == 0)
              { %>
            <tr class="tr_bai">
                <td colspan="9" style="text-align: center;">
                    --无数据--
                </td>
            </tr>
            <%}
              else
              {
            %>
            <tr class="tr_bai" style="font-weight: bold;">
                <td style="text-align: right;">
                    合 计：
                </td>
                <td>
                    <asp:Label ID="labPOPTotalArea" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: center;">
                    <asp:Label ID="labPOPTotalPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td>
                </td>
                <td>
                    <asp:Label ID="labAddRateTotalPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="labPOPImportTotalArea" runat="server" Text="0"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="labPOPImportTotalPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td>
                </td>
                <td>
                    <asp:Label ID="labOtherQuoteTotalPrice" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
            <%} %>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <div class="tr" style="margin-top: 20px;">
        》T1-T3安装费统计
    </div>
    <table class="table">
        <tr class="tr_hui">
            <td rowspan="2" style="width: 180px;">
                费用名称
            </td>
            <td rowspan="2">
                类型
            </td>
            <td rowspan="2">
                单位
            </td>
            <td rowspan="2">
                费用说明
            </td>
            <td rowspan="2">
                单价
            </td>
            <td colspan="2" style="font-weight: bold;">
                系统
            </td>
            <td colspan="3" style="font-weight: bold;">
                实际报价
            </td>
        </tr>
        <tr class="tr_hui">
            <td>
                发生数量
            </td>
            <td>
                金额
            </td>
            <td>
                发生数量
            </td>
            <td>
                金额
            </td>
            <td>
                与系统差额
            </td>
        </tr>
        <tr class="tr_bai">
            <td rowspan="4">
                户外安装费
            </td>
            <td>
                一层门头以上位置 （距水平地面高度在6M以上）
            </td>
            <td>
                店
            </td>
            <td>
            </td>
            <td>
                5000
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel1Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel1" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel1QuoteCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel1Quote" runat="server" Text="0"></asp:Label>
            </td>
            <td class="differenceTClass">
                <asp:Label ID="labOOHLevel1QuoteDiff" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                一层门头以上位置 （距水平地面高度在6M以内）
            </td>
            <td>
                店
            </td>
            <td>
            </td>
            <td>
                2700
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel2Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel2" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel2QuoteCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel2Quote" runat="server" Text="0"></asp:Label>
            </td>
            <td class="differenceTClass">
                <asp:Label ID="labOOHLevel2QuoteDiff" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                一层门头以上位置 （距水平地面高度在3M以内）
            </td>
            <td>
                店
            </td>
            <td>
            </td>
            <td>
                1800
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel3Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel3" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel3QuoteCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel3Quote" runat="server" Text="0"></asp:Label>
            </td>
            <td class="differenceTClass">
                <asp:Label ID="labOOHLevel3QuoteDiff" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                一层门头位置
            </td>
            <td>
                店
            </td>
            <td>
            </td>
            <td>
                600
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel4Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel4" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel4QuoteCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel4Quote" runat="server" Text="0"></asp:Label>
            </td>
            <td class="differenceTClass">
                <asp:Label ID="labOOHLevel4QuoteDiff" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td rowspan="8">
                店铺安装费
            </td>
            <td>
                Core-VVIP店铺
            </td>
            <td>
                店
            </td>
            <td>
                橱窗部分
            </td>
            <td>
                1000
            </td>
            <td class="output">
                <asp:Label ID="labWindowLevel1Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labWindowLevel1" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labWindowLevel1QuoteCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labWindowLevel1Quote" runat="server" Text="0"></asp:Label>
            </td>
            <td class="differenceTClass">
                <asp:Label ID="labWindowLevel1QuoteDiff" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                Core-VVIP店铺
            </td>
            <td>
                店
            </td>
            <td>
                店内其它
            </td>
            <td>
                800
            </td>
            <td class="output">
                <asp:Label ID="labBasicLevel1Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labBasicLevel1" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labBasicLevel1QuoteCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labBasicLevel1Quote" runat="server" Text="0"></asp:Label>
            </td>
            <td class="differenceTClass">
                <asp:Label ID="labBasicLevel1QuoteDiff" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                Core-Premium店铺
            </td>
            <td>
                店
            </td>
            <td>
                橱窗部分
            </td>
            <td>
                500
            </td>
            <td class="output">
                <asp:Label ID="labWindowLevel2Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labWindowLevel2" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labWindowLevel2QuoteCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labWindowLevel2Quote" runat="server" Text="0"></asp:Label>
            </td>
            <td class="differenceTClass">
                <asp:Label ID="labWindowLevel2QuoteDiff" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                Core-Premium店铺
            </td>
            <td>
                店
            </td>
            <td>
                店内其它
            </td>
            <td>
                400
            </td>
            <td class="output">
                <asp:Label ID="labBasicLevel2Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labBasicLevel2" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labBasicLevel2QuoteCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labBasicLevel2Quote" runat="server" Text="0"></asp:Label>
            </td>
            <td class="differenceTClass">
                <asp:Label ID="labBasicLevel2QuoteDiff" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                Core-Basic店铺
            </td>
            <td>
                店
            </td>
            <td>
                橱窗部分
            </td>
            <td>
                200
            </td>
            <td class="output">
                <asp:Label ID="labWindowLevel3Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labWindowLevel3" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labWindowLevel3QuoteCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labWindowLevel3Quote" runat="server" Text="0"></asp:Label>
            </td>
            <td class="differenceTClass">
                <asp:Label ID="labWindowLevel3QuoteDiff" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                Core-Basic店铺
            </td>
            <td>
                店
            </td>
            <td>
                店内其它
            </td>
            <td>
                150
            </td>
            <td class="output">
                <asp:Label ID="labBasicLevel3Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labBasicLevel3" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labBasicLevel3QuoteCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labBasicLevel3Quote" runat="server" Text="0"></asp:Label>
            </td>
            <td class="differenceTClass">
                <asp:Label ID="labBasicLevel3QuoteDiff" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                其它类型店铺-带橱窗
            </td>
            <td>
                店
            </td>
            <td>
                橱窗+店内
            </td>
            <td>
                500
            </td>
            <td class="output">
                <asp:Label ID="labKidsWindowLevelCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labKidsWindowLevel" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labKidsWindowLevelQuoteCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labKidsWindowLevelQuote" runat="server" Text="0"></asp:Label>
            </td>
            <td class="differenceTClass">
                <asp:Label ID="labKidsWindowLevelQuoteDiff" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                其它类型店铺-不带橱窗
            </td>
            <td>
                店
            </td>
            <td>
                橱窗+店内
            </td>
            <td>
                150
            </td>
            <td class="output">
                <asp:Label ID="labKidsBasicLevelCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labKidsBasicLevel" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labKidsBasicLevelQuoteCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labKidsBasicLevelQuote" runat="server" Text="0"></asp:Label>
            </td>
            <td class="differenceTClass">
                <asp:Label ID="labKidsBasicLevelQuoteDiff" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                Generic POP 更换安装
            </td>
            <td>
            </td>
            <td>
                店
            </td>
            <td>
            </td>
            <td>
                150
            </td>
            <td class="output">
                <asp:Label ID="labGenericLevelCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labGenericLevel" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labGenericLevelQuoteCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labGenericLevelQuote" runat="server" Text="0"></asp:Label>
            </td>
            <td class="differenceTClass">
                <asp:Label ID="labGenericLevelQuoteDiff" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td colspan="6" style="text-align: right;">
                合计：
            </td>
            <td>
                <asp:Label ID="labInstallPriceTotal" runat="server" Text="0"></asp:Label>
            </td>
            <td>
            </td>
            <td>
                <asp:Label ID="labInstallPriceQuoteTotal" runat="server" Text="0"></asp:Label>
            </td>
            <td>
            </td>
        </tr>
    </table>
    <div class="tr" style="margin-top: 20px;">
        》其他级别安装费统计
    </div>
    <asp:Repeater ID="otherInstallPriceRepeater" runat="server" OnItemDataBound="otherInstallPriceRepeater_ItemDataBound">
        <HeaderTemplate>
            <table class="table" id="otherInstallPriceTable">
                <tr class="tr_hui" style="font-weight: bold;">
                    <td>
                        费用名称
                    </td>
                    <td>
                        单位
                    </td>
                    <td>
                        单价
                    </td>
                    <td>
                        发生数量
                    </td>
                    <td>
                        金额
                    </td>
                    <td>
                        合计
                    </td>
                    <td>
                        实际报价
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="tr_bai">
                <td id="priceType" runat="server">
                    <%#Eval("PriceType")%>
                </td>
                <td>
                    店
                </td>
                <td>
                    <%#Eval("Price")%>
                </td>
                <td>
                    <%#Eval("Count")%>
                </td>
                <td>
                    <asp:Label ID="labPrice" runat="server" Text=""></asp:Label>
                </td>
                <td id="subPrice" runat="server">
                    <asp:Label ID="labSubPrice" runat="server" Text=""></asp:Label>
                </td>
                <td id="operatePrice" runat="server" style="text-align: left; padding-left: 10px;">
                    <asp:Panel ID="PanelPOP" runat="server" Visible="false">
                        <span style="font-weight: bolder;">POP</span>：合计：
                        <asp:Label ID="labPOPTotal" runat="server" Text="0"></asp:Label>
                        <asp:Label ID="labPOPQuoteList" runat="server" Text=""></asp:Label>
                    </asp:Panel>
                    <asp:Panel ID="PanelOOH" runat="server">
                        <span style="font-weight: bolder;">高空安装费</span>： 5000：
                        <asp:Label ID="labOOHPriceCount1" runat="server" Text="0"></asp:Label>， &nbsp;&nbsp;2700：
                        <asp:Label ID="labOOHPriceCount2" runat="server" Text="0"></asp:Label>， &nbsp;&nbsp;1800：
                        <asp:Label ID="labOOHPriceCount3" runat="server" Text="0"></asp:Label>， &nbsp;&nbsp;600：
                        <asp:Label ID="labOOHPriceCount4" runat="server" Text="0"></asp:Label>
                        &nbsp;&nbsp; 合计：
                        <asp:Label ID="labOOHTotal" runat="server" Text="0"></asp:Label>
                    </asp:Panel>
                    <asp:Panel ID="PanelBasic" runat="server">
                        <span style="font-weight: bolder;">店内安装费</span>： 800：
                        <asp:Label ID="labBasicPriceCount1" runat="server" Text="0"></asp:Label>， &nbsp;&nbsp;400：
                        <asp:Label ID="labBasicPriceCount2" runat="server" Text="0"></asp:Label>， &nbsp;&nbsp;150：
                        <asp:Label ID="labBasicPriceCount3" runat="server" Text="0"></asp:Label>
                        &nbsp;&nbsp;合计：
                        <asp:Label ID="labBasicTotal" runat="server" Text="0"></asp:Label>
                    </asp:Panel>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (otherInstallPriceRepeater.Items.Count == 0)
              { %>
            <tr class="tr_bai">
                <td colspan="7" style="text-align: center;">
                    --无数据--
                </td>
            </tr>
            <%} %>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <div class="tr" style="margin-top: 20px;">
        》快递费/道具费统计—<span style="color: Blue;">系统数据</span>
    </div>
    <asp:Repeater ID="expressPriceListRepeater" runat="server" OnItemDataBound="expressPriceListRepeater_ItemDataBound">
        <HeaderTemplate>
            <table class="table">
                <tr class="tr_hui">
                    <td>
                        道具名称
                    </td>
                    <td>
                        类型
                    </td>
                    <td>
                        单价
                    </td>
                    <td>
                        数量
                    </td>
                    <td>
                        金额
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="tr_bai">
                <td runat="server" id="sheet">
                    <%--道具名称--%>
                    <%#Eval("PriceType")%>
                </td>
                <td>
                    <%--类型--%>
                    <%#Eval("OrderType")%>
                </td>
                <td>
                    <%--单价--%>
                    <%#Eval("Price")%>
                </td>
                <td>
                    <%--数量--%>
                    <%#Eval("Count")%>
                </td>
                <td>
                    <%--金额--%>
                    <asp:Label ID="labSubPrice" runat="server" Text=""></asp:Label>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (expressPriceListRepeater.Items.Count == 0)
              { %>
            <tr class="tr_bai">
                <td colspan="5" style="text-align: center;">
                    --无数据--
                </td>
            </tr>
            <%}
              else
              {
            %>
            <tr class="tr_bai">
                <td colspan="3" style="text-align: right;">
                    合计：
                </td>
                <td style="text-align: center;">
                    <asp:Label ID="labExpressPriceTotal" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
            <%} %>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <asp:Panel ID="Panel_QuoteExpress" runat="server" Visible="false">
        <div class="tr" style="margin-top: 20px;">
            》快递费/道具费统计—<span style="color: Blue;">实际报价</span>
        </div>
        <asp:Repeater ID="expressPriceListRepeater1" runat="server" OnItemDataBound="expressPriceListRepeater1_ItemDataBound">
            <HeaderTemplate>
                <table class="table">
                    <tr class="tr_hui">
                        <td>
                            道具名称
                        </td>
                        <td>
                            类型
                        </td>
                        <td>
                            单价
                        </td>
                        <td>
                            数量
                        </td>
                        <td>
                            金额
                        </td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="tr_bai">
                    <td runat="server" id="sheet">
                        <%--道具名称--%>
                        <%#Eval("PriceType")%>
                    </td>
                    <td>
                        <%#Eval("OrderType")%>
                    </td>
                    <td>
                        <%--单价--%>
                        <%#Eval("Price")%>
                    </td>
                    <td>
                        <%--数量--%>
                        <%#Eval("Count")%>
                    </td>
                    <td>
                        <%--金额--%>
                        <asp:Label ID="labSubPrice1" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <%if (expressPriceListRepeater1.Items.Count == 0)
                  { %>
                <tr class="tr_bai">
                    <td colspan="5" style="text-align: center;">
                        --无数据--
                    </td>
                </tr>
                <%}
                  else
                  {
                %>
                <tr class="tr_bai">
                    <td colspan="4" style="text-align: right;">
                        合计：
                    </td>
                    <td style="text-align: center;">
                        <asp:Label ID="labExpressPriceTotal1" runat="server" Text="0"></asp:Label>
                    </td>
                </tr>
                <%} %>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </asp:Panel>
    </div>
    <div id="quoteFooter">
        <table class="table" style="font-weight: bold;">
            <tr class="tr_hui">
                <td style="width: 120px; height:40px;">
                    系统合计金额：
                </td>
                <td style="text-align: left; padding-left: 5px; font-size: 18px; width: 250px;">
                    <asp:Label ID="labSystemTotalPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td style="width: 180px;">
                    实际报价合计金额：
                </td>
                <td style="text-align: left; padding-left: 5px; font-size: 18px;">
                    <asp:Label ID="labQuoteTotalPrice" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
<script src="js/checkQuotation.js" type="text/javascript"></script>
