<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewShopInstallPriceDetail.aspx.cs" Inherits="WebApp.Statistics.NewShopInstallPriceDetail" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            <asp:Label ID="labTitle" runat="server" Text="运费/新开店安装费用明细"></asp:Label>
        </p>
    </div>
    <div style="margin: 0px;">
        <table class="table">
            
            
            <tr class="tr_hui">
                <td style="text-align: center; width: 90px;">
                    店铺数量：
                </td>
                <td style="text-align: left; padding-left: 5px; width: 100px;">
                    <asp:Label ID="labShopCount" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: center; width: 90px;">
                    金额合计：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labTotlePrice" runat="server" Text="0"></asp:Label>
                </td>
               </tr>
        </table>
    </div>
    <div class="tr">
        >>信息列表
    </div>
    <asp:Repeater ID="gvList" runat="server" onitemdatabound="gvList_ItemDataBound">
        <HeaderTemplate>
            <table class="table1" style="width: 100%;">
                <tr class="tr_hui">
                    <td style="width: 40px;">
                        序号
                    </td>
                    <td>
                        项目名称
                    </td>
                    <td>
                        订单类型
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
                        店铺地址
                    </td>
                    <td>
                        金额(元)
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
                    <%#Eval("subject.SubjectName")%>
                </td>
                 <td>
                     <asp:Label ID="labSubjectType" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    <%#Eval("order.ShopName")%>
                </td>
                <td>
                    <%#Eval("order.Region")%>
                </td>
                <td>
                    <%#Eval("order.Province")%>
                </td>
                <td>
                    <%#Eval("order.City")%>
                </td>
                <td>
                    <%#Eval("order.Address")%>
                </td>
                <td>
                    <%#Eval("order.Amount")%>
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
                <td colspan="10" style="text-align: center;">
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
