<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckDetail.aspx.cs" Inherits="WebApp.Subjects.SupplementByRegion.CheckDetail" %>

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
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            项目查看
        </p>
    </div>
    <div class="tr">
        >>项目信息
    </div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                项目编号
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目创建人
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
                <asp:Label ID="labAddUserName" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
                所属客户
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labCustomerName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                订单类型
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectType" runat="server" Text=""></asp:Label>
            </td>
            <td>
                区域
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labRegion" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                备注
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labRemark" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <div class="tab" style="margin-top: 15px;">
        补单明细
        <span style=" margin-left:20px;">
            <asp:LinkButton ID="lbExport" runat="server" onclick="lbExport_Click" OnClientClick="return CheckExport()">导出</asp:LinkButton>
            <img id="exportLoadingImg" src="../../image/loadingA.gif" style="display:none;"/>
        </span>
        <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
    </div>
    <div class="containerDiv1">
        <asp:Repeater ID="orderList" runat="server" 
            onitemdatabound="orderList_ItemDataBound">
            <HeaderTemplate>
                <table class="table1">
                    <tr class="tr_hui">
                        <td style="width: 30px;">
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
                            店铺大小
                        </td>
                        <td>
                            店铺级别
                        </td>
                        <td>
                            POP位置
                        </td>
                        <td>
                            器架名称
                        </td>
                        <td>
                            POP位置明细
                        </td>
                        <td>
                            性别
                        </td>
                        <td>
                            数量
                        </td>
                        <td>
                            POP宽
                        </td>
                        <td>
                            POP高
                        </td>
                        <td>
                            面积
                        </td>
                        <td>
                            材质
                        </td>
                        <td>
                            系列/选图
                        </td>
                        <td>
                            安装位置描述
                        </td>
                        <td>
                            其他备注
                        </td>
                    </tr>
                    <tbody id="tbody1">
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td style="width: 30px;">
                        <%#(AspNetPager1.CurrentPageIndex-1)*AspNetPager1.PageSize+ Container.ItemIndex + 1%>
                    </td>
                    <td>
                        <%#Eval("order.SubjectName")%>
                    </td>
                    <td>
                        <%#Eval("shop.ShopNo") %>
                    </td>
                    
                    <td>
                        <%#Eval("shop.ShopName") %>
                    </td>
                    <td>
                        <%#Eval("shop.RegionName") %>
                    </td>
                    <td>
                        <%#Eval("shop.ProvinceName") %>
                    </td>
                    <td>
                        <%#Eval("shop.CityName") %>
                    </td>
                    <td>
                        <%#Eval("order.POSScale")%>
                    </td>
                    <td>
                        <%#Eval("order.MaterialSupport")%>
                    </td>
                    <td>
                        <%#Eval("order.Sheet") %>
                    </td>
                    <td>
                        <%#Eval("order.MachineFrame")%>
                    </td>
                    <td>
                        <%#Eval("order.PositionDescription")%>
                    </td>
                    <td>
                        <%#Eval("order.Gender")%>
                    </td>
                    <td>
                        <%#Eval("order.Quantity")%>
                    </td>
                    <td>
                        <%#Eval("order.GraphicWidth")%>
                    </td>
                    <td>
                        <%#Eval("order.GraphicLength")%>
                    </td>
                    <td>
                        <%#Eval("order.Area")%>
                    </td>
                    <td>
                        <%#Eval("order.GraphicMaterial")%>
                    </td>
                    <td>
                        <%#Eval("order.ChooseImg")%>
                    </td>
                    
                    <td>
                        <%#Eval("order.InstallPositionDescription")%>
                    </td>
                    <td>
                        <%#Eval("order.Remark")%>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
                <%if (orderList.Items.Count == 0)
                  { %>
                <tr class="tr_bai">
                    <td colspan="19" style="text-align: center;">
                        --暂无数据--
                    </td>
                </tr>
                <%} %>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    <div style="text-align: center; margin-top: 10px;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" showinputbox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    <div style="text-align: center; margin-bottom: 30px; margin-top: 20px;">
        <input type="button" value="返 回" onclick="javascript:window.history.go(-1)" class="easyui-linkbutton"
            style="width: 65px; height: 26px; font-size: 13px;" />
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    function CheckExport() {
        if ($("#tbody1").html() == "") {
            alert("没有数据！");
            return false;
        }
        $("#exportLoadingImg").show();
        checkExport();
        return true;
    }

    var timer;
    function checkExport() {
        timer = setInterval(function () {
            $.ajax({
                type: "get",
                url: "CheckExport.ashx",
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        $("#exportLoadingImg").hide();
                        clearInterval(timer);
                    }

                }
            })

        }, 1000);
    }
</script>