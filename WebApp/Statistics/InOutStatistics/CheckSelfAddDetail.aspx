<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckSelfAddDetail.aspx.cs"
    Inherits="WebApp.Statistics.InOutStatistics.CheckSelfAddDetail" %>

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
        <asp:Repeater ID="Repeater1" runat="server" 
            onitemdatabound="Repeater1_ItemDataBound">
            <HeaderTemplate>
                <table class="table">
                    <tr class="tr_hui" style=" font-weight:bold;">
                        <td rowspan="2" style=" width:50px;">
                            序号
                        </td>
                        <td rowspan="2">
                           位置
                        </td>
                        <td colspan="3">
                            按百分百调整尺寸
                        </td>
                        <td rowspan="2">
                            其他项目折算
                        </td>
                    </tr>
                    <tr class="tr_hui">
                        <td>
                            百分百(%)
                        </td>
                        <td>
                            面积(㎡)
                        </td>
                        <td>
                            金额(元)
                        </td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="tr_bai">
                    <td>
                       <%#Container.ItemIndex+1 %>
                    </td>
                    <td>
                       <%#Container.DataItem %>
                    </td>
                    <td>
                        <asp:Label ID="labAddRate" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labAddRateArea" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labAddRatePrice" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                       <table id="AddExtendPOPPriceTable" runat="server" style="width: 100%;"></table>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <%if (Repeater1.Items.Count == 0)
                  { %>
                    <tr class="tr_bai">
                       <td colspan="6" style=" text-align:center;">--暂无数据--</td>
                    </tr>
                <%}
                  else
                  {%>
                     <tr class="tr_hui" style=" font-weight:bold; font-size:14px;">
                        <td colspan="6" style=" text-align:right; padding-right:50px;">
                        面积合计： <asp:Label ID="labTotalArea" runat="server" Text="0"></asp:Label>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        金额合计： <asp:Label ID="labTotalPrice" runat="server" Text="0"></asp:Label>
                        </td>
                    </tr>
                <%} %>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    </form>
</body>
</html>
