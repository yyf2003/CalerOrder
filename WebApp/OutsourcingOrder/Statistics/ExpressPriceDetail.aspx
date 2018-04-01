<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExpressPriceDetail.aspx.cs" Inherits="WebApp.OutsourcingOrder.Statistics.ExpressPriceDetail" %>
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
    <div>
        <table class="table">
             <tr class="tr_hui">
                <td style="text-align:right; width: 100px;">
                    店铺编号：
                </td>
                <td colspan="5" style="text-align: left;padding-left: 5px;">
                    <asp:TextBox ID="txtShopNo" runat="server" MaxLength="20"  style="width:250px;"></asp:TextBox>
                </td>
                
                <td>
                   <asp:Button ID="btnSearch" runat="server" Text="查 询" class="easyui-linkbutton" OnClientClick="return check1()" style="width: 65px;
                        height: 26px; " onclick="btnSearch_Click"/>
                        <img id="searchWaiting" style="display: none;" src="../../image/WaitImg/loadingA.gif" />
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align:right; width: 100px;">
                    店铺数量：
                </td>
                <td style="text-align: left; width: 150px; padding-left: 5px;">
                    <asp:Label ID="labShopCount" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: center; width: 90px;">
                    应付合计：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labTotalPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: center; width: 90px;">
                    应收合计：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labRTotalPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td style="width: 120px;">
                    <asp:Button ID="btnExport" runat="server" Text="导 出" class="easyui-linkbutton" OnClientClick="return check()"
                        Style="width: 65px; height: 26px;" OnClick="btnExport_Click" />
                    <img id="exportWaiting" style="display: none;" src="/image/WaitImg/loadingA.gif" />
                </td>
            </tr>
        </table>
        
    </div>
    <div class="tr" style=" height:25px;margin-top:8px;">
        》店铺信息
       </div>
    <div class="containerDiv">
        <div id="loadingImg" style="display:none;">
            <img src="../../image/WaitImg/loading1.gif" />
        </div>
        <asp:Repeater ID="gvPrice" runat="server">
            <HeaderTemplate>
                <table class="table" style="width: 1200px; margin-right:10px;">
                    <tr class="tr_hui">
                        <td style="width: 40px;">
                            序号
                        </td>
                        <td>
                            活动名称
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
                            应付费用
                        </td>
                        <td>
                            应收费用
                        </td>
                        <td>
                            备注
                        </td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="tr_bai">
                    <td style="width: 40px;">
                        <%#(AspNetPager1.CurrentPageIndex-1)*AspNetPager1.PageSize+ Container.ItemIndex + 1%>
                    </td>
                    <td>
                       <%#Eval("GuidanceName")%>
                    </td>
                    <td>
                        <%#Eval("ShopNo") %>
                    </td>
                    <td>
                        <%#Eval("ShopName") %>
                    </td>
                    <td>
                        <%#Eval("RegionName") %>
                    </td>
                    <td>
                        <%#Eval("ProvinceName") %>
                    </td>
                    <td>
                        <%#Eval("CityName") %>
                    </td>
                    <td>
                        <%#Eval("ExpressPrice")%>
                    </td>
                    <td>
                        <%#Eval("ReceiveExpressPrice")%>
                    </td>
                     <td>
                        <%#Eval("Remark")%>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <%if (gvPrice.Items.Count == 0)
                  { %>
                <tr class="tr_bai">
                    <td colspan="14" style="text-align: center;">
                        --无数据--
                    </td>
                </tr>
                <%} %>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
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
