<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckOtherPriceDetail.aspx.cs" Inherits="WebApp.OutsourcingOrder.Statistics.CheckOtherPriceDetail" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="/easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="/easyui1.4/jquery.easyui.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="tr">
        >>费用信息
    </div>
    
    <asp:Repeater ID="gvList" runat="server">
        <HeaderTemplate>
            
            <table class="table1" style="width: 100%;">
                <tr class="tr_hui">
                    <td style="width: 40px;">
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
                        Format
                    </td>
                   
                    <td>
                        应付金额
                    </td>
                    <td>
                        应收金额
                    </td>
                    <td>
                        备注
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td style="width: 40px;">
                    <%#(AspNetPager1.CurrentPageIndex-1)*AspNetPager1.PageSize+ Container.ItemIndex + 1%>
                </td>
                <td>
                   其他费用
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
                        <%#Eval("order.Format")%>
                    </td>
               
                <td>
                    <%#Eval("order.PayOrderPrice")%>
                </td>
                 <td>
                    <%#Eval("order.ReceiveOrderPrice")%>
                </td>
               <td>
                     <%#Eval("order.Remark")%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (gvList.Items.Count == 0)
              {%>
            <tr class="tr_bai">
                <td colspan="11" style="text-align: center;">
                    --无数据--
                </td>
            </tr>
            <%} %>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <div style="text-align: center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    </form>
</body>
</html>
