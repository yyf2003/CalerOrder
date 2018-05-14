<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckOrderDetail.aspx.cs"
    Inherits="WebApp.Subjects.PriceOrder.CheckOrderDetail" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="/easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="/easyui1.4/jquery.easyui.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            费用订单信息
        </p>
    </div>
    <div class="tr">
        >>项目信息</div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                项目编号
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td class="style1">
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style1">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td class="style1">
                所属客户
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style1">
                <asp:Label ID="labCustomer" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                订单类型
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectType" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <div class="tr" style="margin-top: 20px;">
        >>订单信息</div>
    <div class="containerDiv">
        <asp:Repeater ID="orderListRepeater" runat="server" OnItemDataBound="orderListRepeater_ItemDataBound">
            <HeaderTemplate>
                <table class="table">
                    <tr class="tr_hui">
                        <td style="width: 50px;">
                            序号
                        </td>
                        <td>
                            费用类型
                        </td>
                        <td>
                            店铺编号
                        </td>
                        <td>
                            店铺名称
                        </td>
                        <td>
                            区域
                        </td>
                        <td>
                            省份
                        </td>
                        <td>
                            城市
                        </td>
                        <td>
                            应收金额
                        </td>
                        <td>
                            应付金额
                        </td>
                        <td>
                            数量
                        </td>
                        <td>
                            费用内容
                        </td>
                        <td>
                            备注
                        </td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="tr_bai">
                    <td>
                        <%#(AspNetPager1.CurrentPageIndex-1)*AspNetPager1.PageSize+ Container.ItemIndex + 1%>
                    </td>
                    <td>
                        <asp:Label ID="labOrderType" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <%#Eval("ShopNo")%>
                    </td>
                    <td>
                        <%#Eval("ShopName")%>
                    </td>
                    <td>
                        <%#Eval("Region")%>
                    </td>
                    <td>
                        <%#Eval("Province")%>
                    </td>
                    <td>
                        <%#Eval("City")%>
                    </td>
                    <td>
                        <%#Eval("Amount")%>
                    </td>
                    <td>
                        <%#Eval("PayAmount")%>
                    </td>
                    <td>
                        <%#Eval("Quantity") == null ? 1 : Eval("Quantity")%>
                    </td>
                    <td>
                        <%#Eval("Contents")%>
                    </td>
                    <td>
                        <%#Eval("Remark")%>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <%if (orderListRepeater.Items.Count == 0)
                  { %>
                <tr class="tr_bai">
                    <td colspan="12" style="text-align: center;">
                        --暂无数据--
                    </td>
                </tr>
                <%} %>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    <div style="text-align: center; margin-top: 5px;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" showinputbox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    <asp:Panel ID="Panel_ApproveInfo" runat="server" Visible="false">
        <div class="tr" style="margin-top: 20px;">
            >>审批记录</div>
        <div id="approveInfoDiv" runat="server">
        </div>
    </asp:Panel>
    <div style="text-align: center; margin-bottom: 30px; margin-top: 20px;">
        <input type="button" value="返 回" onclick="javascript:window.history.go(-1)" class="easyui-linkbutton"
            style="width: 65px; height: 26px; font-size: 13px;" />
    </div>
    </form>
</body>
</html>
