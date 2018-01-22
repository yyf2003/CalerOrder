<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckMaterialPriceOrder.aspx.cs" Inherits="WebApp.OutsourcingOrder.Statistics.CheckMaterialPriceOrder" %>

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
    <div>
        <table class="table">
            <tr class="tr_bai">
                <td style="text-align: center; width: 120px;">
                    店铺数量：
                </td>
                <td style="text-align: left; width: 150px; padding-left: 5px;">
                    <asp:Label ID="labShopCount" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: center; width: 90px;">
                    费用合计：
                </td>
                <td style="text-align: left; padding-left: 5px; ">
                    <asp:Label ID="labTotalPrice" runat="server" Text="0"></asp:Label>
                   
                </td>
                
            </tr>
        </table>
    </div>
    <div class="tr" style="height: 25px; margin-top: 8px;">
        》店铺信息
    </div>
     <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="table">
                    <tr class="tr_hui">
                        <td style="width: 40px;">
                            序号
                        </td>
                        <td>
                            外协名称
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
                            物料名称
                        </td>
                        <td>
                            数量
                        </td>
                        <td>
                            费用
                        </td>
                       <td>
                            添加时间
                        </td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="tr_bai">
                    <td style="width: 40px;">
                        <%# Container.ItemIndex + 1%>
                    </td>
                    <td>
                       <%#Eval("CompanyName")%>
                    </td>
                    <td>
                        <%#Eval("shop.ShopNo")%>
                    </td>
                    <td>
                        <%#Eval("shop.ShopName")%>
                    </td>
                    <td>
                        <%#Eval("shop.RegionName")%>
                    </td>
                    <td>
                        <%#Eval("shop.ProvinceName")%>
                    </td>
                    <td>
                        <%#Eval("shop.CityName")%>
                    </td>
                    <td>
                        <%#Eval("order.MaterialName")%>
                    </td>
                    <td>
                        <%#Eval("order.Count")%>
                    </td>
                    <td>
                        <%#Eval("order.TotalPrice")%>
                    </td>
                    <td>
                        <%#Eval("order.AddDate")%>
                    </td>
                    
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <%if (Repeater1.Items.Count == 0)
                  { %>
                <tr class="tr_bai">
                    <td colspan="11" style="text-align: center;">
                        --无数据--
                    </td>
                </tr>
                <%} %>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </form>
</body>
</html>
