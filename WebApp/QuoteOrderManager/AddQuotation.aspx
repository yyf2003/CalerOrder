<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddQuotation.aspx.cs" Inherits="WebApp.QuoteOrderManager.AddQuotation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
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
        </table>
    </div>
    <div class="tr" style="margin-top: 20px;">
        》POP订单统计
    </div>
    <asp:Repeater ID="popList" runat="server">
        <HeaderTemplate>
            <table class="table">
                <tr class="tr_hui">
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
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="tr_bai">
                <td runat="server" id="sheet">
                    <%--画面位置--%>
                    <%--<asp:Label ID="labSheet" runat="server" Text=""></asp:Label>--%>
                    <%#Eval("Sheet") %>
                </td>
                <td>
                    <%--画面名称/描述--%>
                    <%-- <asp:Label ID="labPositionDescription" runat="server" Text=""></asp:Label>--%>
                    <%#Eval("PositionDescription")%>
                </td>
                <td>
                    <%-- 画面材质/安装物料--%>
                    <%--<asp:Label ID="labGraphicMaterial" runat="server" Text=""></asp:Label>--%>
                    <%#Eval("QuoteGraphicMaterial")%>
                </td>
                <td>
                    <%--单位--%>
                    <%-- <asp:Label ID="labUnits" runat="server" Text=""></asp:Label>--%>
                    <%#Eval("UnitName")%>
                </td>
                <td>
                    <%--画面尺寸/物料数量--%>
                    <%-- <asp:Label ID="labQuantity" runat="server" Text=""></asp:Label>--%>
                    <%#Eval("Amount")%>
                </td>
                <td>
                    <%--单价--%>
                    <%--<asp:Label ID="labUnitPrice" runat="server" Text=""></asp:Label>--%>
                    <%#Eval("UnitPrice")%>
                </td>
                <td>
                    <%--金额--%>
                    <%--<asp:Label ID="labSubPrice" runat="server" Text=""></asp:Label>--%>
                    <%#Eval("TotalPrice")%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (popList.Items.Count == 0)
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
        》T1-T3安装费统计
    </div>
    <table class="table">
        <tr class="tr_hui">
            <td style=" width:180px;">
                费用名称
            </td>
            <td>
                类型
            </td>
            <td>
                单位
            </td>
            <td>
                费用说明
            </td>
            <td>
                单价
            </td>
            <td>
                发生数量
            </td>
            <td style=" min-width:60px;">
                金额
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
                须提供设备租赁证明及操作人员高空作业证
            </td>
            <td>
                5000
            </td>
            <td>
                <asp:Label ID="labOOHLevel1Count" runat="server" Text="0"></asp:Label>
            </td>
            <td>
               <asp:Label ID="labOOHLevel1" runat="server" Text="0"></asp:Label>
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
                须提供吊兰租赁证明及操作人员高空作业证
            </td>
            <td>
                2700
            </td>
            <td>
               <asp:Label ID="labOOHLevel2Count" runat="server" Text="0"></asp:Label>
            </td>
            <td>
               <asp:Label ID="labOOHLevel2" runat="server" Text="0"></asp:Label>
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
                须提供吊兰租赁证明及操作人员高空作业证
            </td>
            <td>
                1800
            </td>
            <td>
              <asp:Label ID="labOOHLevel3Count" runat="server" Text="0"></asp:Label>
            </td>
            <td>
              <asp:Label ID="labOOHLevel3" runat="server" Text="0"></asp:Label>
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
            <td>
              <asp:Label ID="labOOHLevel4Count" runat="server" Text="0"></asp:Label>
            </td>
            <td>
              <asp:Label ID="labOOHLevel4" runat="server" Text="0"></asp:Label>
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
            <td>
                <asp:Label ID="labBasicLevel1Count" runat="server" Text="0"></asp:Label>
            </td>
            <td>
               <asp:Label ID="labBasicLevel1" runat="server" Text="0"></asp:Label>
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
            <td>
                <asp:Label ID="labBasicLevel2Count" runat="server" Text="0"></asp:Label>
            </td>
            <td>
               <asp:Label ID="labBasicLevel2" runat="server" Text="0"></asp:Label>
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
            <td>
                <asp:Label ID="labBasicLevel3Count" runat="server" Text="0"></asp:Label>
            </td>
            <td>
               <asp:Label ID="labBasicLevel3" runat="server" Text="0"></asp:Label>
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
            <td>
                <asp:Label ID="labBasicLevel4Count" runat="server" Text="0"></asp:Label>
            </td>
            <td>
               <asp:Label ID="labBasicLevel4" runat="server" Text="0"></asp:Label>
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
            <td>
                <asp:Label ID="labBasicLevel5Count" runat="server" Text="0"></asp:Label>
            </td>
            <td>
               <asp:Label ID="labBasicLevel5" runat="server" Text="0"></asp:Label>
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
            <td>
                <asp:Label ID="labBasicLevel6Count" runat="server" Text="0"></asp:Label>
            </td>
            <td>
               <asp:Label ID="labBasicLevel6" runat="server" Text="0"></asp:Label>
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
            <td>
                <asp:Label ID="labBasicLevel7Count" runat="server" Text="0"></asp:Label>
            </td>
            <td>
               <asp:Label ID="labBasicLevel7" runat="server" Text="0"></asp:Label>
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
            <td>
                <asp:Label ID="labBasicLevel8Count" runat="server" Text="0"></asp:Label>
            </td>
            <td>
               <asp:Label ID="labBasicLevel8" runat="server" Text="0"></asp:Label>
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
            <td>
                <asp:Label ID="labBasicLevel9Count" runat="server" Text="0"></asp:Label>
            </td>
            <td>
               <asp:Label ID="labBasicLevel9" runat="server" Text="0"></asp:Label>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
