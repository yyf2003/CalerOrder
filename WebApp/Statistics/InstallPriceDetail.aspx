﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InstallPriceDetail.aspx.cs"
    Inherits="WebApp.Statistics.InstallPriceDetail" %>

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
    <div class="tr">
      》安装费明细
    </div>
    <div>
        <table class="table">
           
            <tr class="tr_hui">
                <td style="text-align: center; width: 90px;">
                    店铺编号：
                </td>
                <td style="text-align: left; width: 300px; padding-left: 5px;">
                    <asp:TextBox ID="txtShopNo" runat="server" MaxLength="20"></asp:TextBox>
                </td>
                <td style="text-align: right; width: 90px;">
                    安装费类型：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:CheckBoxList ID="cblFeeType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                      <asp:ListItem Value="window">橱窗&nbsp;</asp:ListItem>
                      <asp:ListItem Value="ooh">户外高空&nbsp;</asp:ListItem>
                    </asp:CheckBoxList>
                </td>
                <td style="width:180px;">
                   <asp:Button ID="btnSearch" runat="server" Text="查 询" class="easyui-linkbutton" OnClientClick="return check1()" style="width: 65px;
                        height: 26px; " onclick="btnSearch_Click"/>
                        <img id="searchWaiting" style="display: none;" src="../image/WaitImg/loadingA.gif" />
                </td>
            </tr>
           <tr class="tr_hui">
                <td>
                    店铺数量：
                </td>
                <td style="text-align: left;padding-left: 5px;">
                   <asp:Label ID="labShopCount" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: right;">
                    合计金额：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   <asp:Label ID="labTotalPrice" runat="server" Text="0"></asp:Label>
                   
                </td>
                <td>
                 <asp:Button ID="btnExport" runat="server" Text="导 出" class="easyui-linkbutton" OnClientClick="return check()" style="width: 65px;
                        height: 26px; " onclick="btnExport_Click"/>
                        <img id="exportWaiting" style="display: none;" src="../image/WaitImg/loadingA.gif" />
                </td>
            </tr>
        </table>
    </div>
    <div class="containerDiv">
        <asp:Repeater ID="gvPrice" runat="server" 
            onitemdatabound="gvPrice_ItemDataBound">
            <HeaderTemplate>
                <table class="table" style=" width:1500px;">
                    <tr class="tr_hui">
                        <td style="width: 40px;">
                            序号
                        </td>
                        <td>
                            活动名称
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
                            城市级别
                        </td>
                        <td>
                            安装级别
                        </td>
                        <td>
                            店铺规模大小
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
                       <%#Eval("SubjectName")%>
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
                        <%#Eval("CityTier")%>
                    </td>
                    <td>
                        <%#Eval("IsInstall")%>
                    </td>
                    <td>
                        <%#Eval("POSScale")%>
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
                <%if (gvPrice.Items.Count == 0)
                  { %>
                <tr class="tr_bai">
                    <td colspan="16" style="text-align: center;">
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
<script type="text/javascript">
    function check1() {
        $("#searchWaiting").show();
        
        return true;
    }

    function check() {
        $("#exportWaiting").show();
        checkExport();
        return true;
    }

    var timer;
    function checkExport() {
        timer = setInterval(function () {
            $.ajax({
                type: "get",
                url: "handler/CheckExportState.ashx?type=installPrice",
                cache: false,
                success: function (data) {

                    if (data == "ok") {
                        $("#exportWaiting").hide();
                        clearInterval(timer);
                    }

                }
            })

        }, 1000);
    }
</script>
