<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckQuoteInstallPrice.aspx.cs" Inherits="WebApp.QuoteOrderManager.CheckQuoteInstallPrice" %>
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
    <table class="table">
      <tr class="tr_bai">
         <td style=" width:150px;">基础安装费：</td>
         <td style=" text-align:left; padding-left:5px;">
             <asp:CheckBoxList ID="cblBasicInstall" runat="server" 
                 RepeatDirection="Horizontal" RepeatLayout="Flow"  AutoPostBack="true"
                 onselectedindexchanged="cblBasicInstall_SelectedIndexChanged">
             </asp:CheckBoxList>
         </td>
      </tr>
      <tr class="tr_bai">
         <td>橱窗安装费：</td>
         <td style=" text-align:left; padding-left:5px;">
             <asp:CheckBoxList ID="cblWindowInstall" runat="server" 
                 RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true"
                 onselectedindexchanged="cblWindowInstall_SelectedIndexChanged">
             </asp:CheckBoxList>
         </td>
      </tr>
      <tr class="tr_bai">
         <td>OOH安装费：</td>
         <td style=" text-align:left; padding-left:5px;">
             <asp:CheckBoxList ID="cblOOHInstall" runat="server" 
                 RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true"
                 onselectedindexchanged="cblOOHInstall_SelectedIndexChanged">
             </asp:CheckBoxList>
         </td>
      </tr>
      <tr class="tr_bai">
         <td>导出</td>
         <td style=" text-align:left; padding-left:5px; height:35px;">
             <asp:Button ID="btnExport" runat="server" Text="导出明细" 
                 class="layui-btn layui-btn-small" onclick="btnExport_Click"/>
             <img id="exportLoading" style=" display:none;" src="../image/WaitImg/loadingA.gif" />
         </td>
      </tr>
    </table>

    <table class="table" style=" margin-top:20px;">
      <tr class="tr_bai">
        <td style=" width:100px;">店铺总数量：</td>
        <td style=" width:80px; text-align:left; padding-left:5px;">
            <asp:Label ID="labTotalShopCount" runat="server" Text="0"></asp:Label>
        </td>
        <td style=" width:100px;">总金额：</td>
        <td style=" text-align:left; padding-left:5px;">
            <asp:Label ID="labTotalPeice" runat="server" Text="0"></asp:Label>
            &nbsp;&nbsp;
            <asp:Label ID="labTotalStr" runat="server" Text=""></asp:Label>
        </td>
      </tr>
    </table>
    <div class="tr">》安装费明细</div>
    <asp:Repeater ID="gvInstallPrice" runat="server" OnItemDataBound="gvInstallPrice_ItemDataBound">
            <HeaderTemplate>
                <table class="table">
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
                            城市级别
                        </td>
                        <td>
                            安装级别
                        </td>
                        <td>
                            物料支持级别
                        </td>
                        <td>
                            基础安装费
                        </td>
                        <td>
                            橱窗安装费
                        </td>
                        <td>
                            户外安装费
                        </td>
                        <td>
                            合计
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
                        <%#Eval("ShopNo")%>
                    </td>
                    <td>
                        <%#Eval("ShopName")%>
                    </td>
                    <td>
                        <%#Eval("RegionName")%>
                    </td>
                    <td>
                        <%#Eval("ProvinceName")%>
                    </td>
                    <td>
                        <%#Eval("CityName")%>
                    </td>
                    <td>
                        <%#Eval("CityTier")%>
                    </td>
                    <td>
                        <%#Eval("IsInstall")%>
                    </td>
                    <td>
                        <%#Eval("MaterialSupport")%>
                    </td>
                    <td>
                        <%#Eval("BasicInstallPrice")%>
                    </td>
                    <td>
                        <%#Eval("WindowInstallPrice")%>
                    </td>
                    <td>
                        <%#Eval("OOHInstallPrice")%>
                    </td>
                    <td>
                        <asp:Label ID="labTotal" runat="server" Text="0"></asp:Label>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <%if (gvInstallPrice.Items.Count == 0)
                  { %>
                <tr class="tr_bai">
                    <td colspan="13" style="text-align: center;">
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
<script type="text/javascript">
   
</script>
