<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckMaterialOrderDetail.aspx.cs"
    Inherits="WebApp.Statistics.CheckMaterialOrderDetail" %>

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
        》物料(道具)费用明细
    </div>
    <table class="table">
        <tr class="tr_bai">
            <td style=" width:120px;">
              合计金额：
            </td>
            <td style=" width:200px; text-align:left; padding-left:5px;">
                <asp:Label ID="labTotalPrice" runat="server" Text=""></asp:Label>
            </td>
            <td style="text-align: right; padding-right: 25px; margin-top: 8px;">
                <asp:Button ID="btnExport" runat="server" Text="导 出" class="easyui-linkbutton" Style="width: 65px;
                    height: 26px;" OnClick="btnExport_Click" />
                <img id="exportWaiting" style="display: none;" src="/image/WaitImg/loadingA.gif" />
            </td>
        </tr>
    </table>
    <div class="tr">
        >>店铺信息
    </div>
    <asp:Repeater ID="gvList" runat="server">
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
                        POP位置
                    </td>
                    <td>
                        物料名称
                    </td>
                    <td>
                        数量
                    </td>
                    <td>
                        长
                    </td>
                    <td>
                        宽
                    </td>
                    <td>
                        高
                    </td>
                    <td>
                        价格
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
                    <%#Eval("shop.ShopNo") %>
                </td>
                <td>
                    <%#Eval("shop.ShopName") %>
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
                    <%#Eval("order.Sheet")%>
                </td>
                <td>
                    <%#Eval("order.MaterialName")%>
                </td>
                <td>
                    <%#Eval("order.MaterialCount")%>
                </td>
                <td>
                    <%#Eval("order.MaterialLength")%>
                </td>
                <td>
                   <%#Eval("order.MaterialWidth")%>
                </td>
                <td>
                   <%#Eval("order.MaterialHigh")%>
                </td>
                <td>
                   <%#Eval("order.Price")%>
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
                <td colspan="15" style="text-align: center;">
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
