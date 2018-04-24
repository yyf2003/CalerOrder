<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckPOPOrderDetail.aspx.cs"
    Inherits="WebApp.QuoteOrderManager.CheckPOPOrderDetail" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
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
                    位置区域
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labSheet" runat="server" Text=""></asp:Label>
                </td>
            </tr>
        </table>
        <div style=" width:100%; overflow:auto;">
        <asp:Repeater ID="CheckPOPOrderRepeater" runat="server">
            <HeaderTemplate>
                <table class="table" style=" width:1600px;">
                    <tr class="tr_hui">
                        <td>
                            序号
                        </td>
                        <td>
                            活动
                        </td>
                        <td>
                            项目
                        </td>
                        <td>
                            店铺名称
                        </td>
                        <td>
                            店铺编号
                        </td>
                        <td>
                            省份
                        </td>
                        <td>
                            城市
                        </td>
                        <td>
                            Sheet
                        </td>
                        <td>
                            器架名称
                        </td>
                        <td>
                            性别
                        </td>
                        <td>
                            POP宽
                        </td>
                        <td>
                            POP高
                        </td>
                        <td>
                            数量
                        </td>
                        <td>
                            材质
                        </td>
                        <td>
                            单价
                        </td>
                        <td>
                            金额
                        </td>
                        <td>
                            备注
                        </td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="tr_bai">
                    <td>
                        <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.ItemIndex + 1%>
                    </td>
                    <td>
                        <%#Eval("ItemName")%>
                    </td>
                    <td>
                        <%#Eval("SubjectName")%>
                    </td>
                    <td>
                        <%--店铺名称--%>
                        <%#Eval("order.ShopName")%>
                    </td>
                    <td>
                        <%--店铺编号--%>
                        <%#Eval("order.ShopNo")%>
                    </td>
                    <td>
                        <%--省份--%>
                        <%#Eval("order.Province")%>
                    </td>
                    <td>
                        <%--城市--%>
                        <%#Eval("order.City")%>
                    </td>
                    <td>
                        <%--Sheet--%>
                        <%#Eval("order.Sheet")%>
                    </td>
                    <td>
                        <%--器架名称--%>
                        <%#Eval("order.MachineFrame")%>
                    </td>
                    <td>
                        <%--性别--%>
                        <%#Eval("Gender")%>
                    </td>
                    <td>
                        <%--POP宽--%>
                        <%#Eval("order.GraphicWidth")%>
                    </td>
                    <td>
                        <%--POP高--%>
                        <%#Eval("order.GraphicLength")%>
                    </td>
                    <td>
                        <%--数量--%>
                        <%#Eval("order.Quantity")%>
                    </td>
                    <td>
                        <%--材质--%>
                        <%#Eval("order.QuoteGraphicMaterial")%>
                    </td>
                    <td>
                        <%--单价--%>
                        <%#Eval("order.UnitPrice")%>
                    </td>
                    <td>
                        <%--金额--%>
                        <%#Eval("order.TotalPrice")%>
                    </td>
                    <td>
                        <%--备注--%>
                        <%#Eval("order.Remark")%>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <%if (CheckPOPOrderRepeater.Items.Count == 0)
                  { %>
                <tr class="tr_bai">
                    <td colspan="17" style="text-align: center;">
                        --无数据--
                    </td>
                </tr>
                <%}%>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        </div>
        <div style="text-align:center;">
           <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
        </div>
    </div>
    </form>
</body>
</html>
