<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShopDetail.aspx.cs" Inherits="WebApp.Subjects.InstallPrice.ShopDetail" %>

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
    <script type="text/javascript">
        $(function () {

            if ($("#hfProvinceAndCity").val().indexOf("$") == -1) {
               
                //$("#loadingImg").show();
                var provinceAndCity = "";
                try {
                    //if (typeof (window.parent.GetSelectProvinceAndCity) == 'function') {

                    //}
                    provinceAndCity = window.parent.GetSelectProvinceAndCity();
                }
                catch (e) {

                }

                if (provinceAndCity == "")
                    provinceAndCity = "$";

                $("#hfProvinceAndCity").val(provinceAndCity);
                $("#Button1").click();
            }
            
        })
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="hfProvinceAndCity" runat="server" Value=""/>
    <asp:Button ID="Button1" runat="server" Text="Button" onclick="Button1_Click" style=" display:none;"/>
    <%--<div class="tr">
        》安装费明细
    </div>--%>
    <div>
        <table class="table">
            <tr class="tr_hui">
                <td style="text-align: center;">
                    省份：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:CheckBoxList ID="cblProvince" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    </asp:CheckBoxList>
                </td>
                <td style="text-align: center;">
                    城市：
                </td>
                <td style="text-align: left; padding-left: 5px;" colspan="2">
                    <asp:CheckBoxList ID="cblCity" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    </asp:CheckBoxList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align: center; width: 80px;">
                    店铺数量：
                </td>
                <td style="text-align: left; width: 250px; padding-left: 5px;">
                    <asp:Label ID="labShopCount" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: center; width: 90px;">
                    合计金额：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labTotalPrice" runat="server" Text="0"></asp:Label>
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
        <asp:Repeater ID="gvPrice" runat="server" OnItemDataBound="gvPrice_ItemDataBound">
            <HeaderTemplate>
                <table class="table" style="width: 1800px; margin-right:10px;">
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
                    <td colspan="15" style="text-align: center;">
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
