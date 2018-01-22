<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckDetail.aspx.cs" Inherits="WebApp.Subjects.HandMadeOrder.CheckDetail" %>

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
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" />
        <p class="nav_table_p">
            查看项目明细
        </p>
    </div>
    <div class="tr">
        >>项目信息</div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
                项目编号
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                开始时间
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
                <asp:Label ID="labBeginDate" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
                结束时间
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labEndDate" runat="server" Text=""></asp:Label>
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
                联系电话
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labTel" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td class="style3">
                所属客户
            </td>
            <td  style="text-align: left; padding-left: 5px;" class="style3">
                <asp:Label ID="labCustomerName" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
                订单类型
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectType" runat="server" Text=""></asp:Label>
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
    <br />
    <div class="tr">
        >>订单信息
    </div>
    <div style="overflow: auto;">
    <asp:Repeater ID="orderList" runat="server">
        <HeaderTemplate>
            <table class="table1" style="width: 100%;">
                <tr class="tr_hui">
                    <td style="width: 30px;">
                        序号
                    </td>
                    <td>
                        店铺编号
                    </td>
                    <td>
                        店铺名称
                    </td>
                    <td>
                            物料支持级别
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
                        位置
                    </td>
                    <td>
                        位置描述
                    </td>
                    <td>
                        级别
                    </td>
                    <td>
                        性别
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
                            材质
                        </td>
                        
                    <td>
                        选图
                    </td>
                    <td>
                        备注
                    </td>
                     <td>
                        操作
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td style="width: 30px;">
                    <%#(AspNetPager1.CurrentPageIndex-1)*AspNetPager1.PageSize+ Container.ItemIndex + 1%>
                </td>
                <td>
                    <%#Eval("shop.ShopNo") %>
                </td>
                <td>
                    <%#Eval("shop.ShopName") %>
                </td>
                <td>
                    <%#Eval("order.MaterialSupport")%>
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
                    <%#Eval("order.Sheet") %>
                </td>
                <td>
                    <%#Eval("order.PositionDescription")%>
                </td>
                <td>
                    <%#Eval("order.LevelNum")%>
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
                         <%#Eval("order.GraphicMaterial")%>
                    </td>
                    
                <td>
                    <%#Eval("order.ChooseImg")%>
                </td>
                <td>
                    <%#Eval("order.Remark")%>
                </td>
                <td>
                         <%#Eval("order.Operation")%>
                    </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    </div>

    <div style="text-align: center; margin-top: 10px;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    <asp:Panel ID="Panel_ApproveInfo" runat="server" Visible="false">
        
        <div class="tr">
        >>审批记录
    </div>
        <div id="approveInfoDiv" runat="server">
        </div>
    </asp:Panel>
    <div style="text-align: center; margin-top:20px; margin-bottom:30px;">
       <input type="button" value="返 回" onclick="javascript:window.history.go(-1)" class="easyui-linkbutton" Style="width: 65px;
            height: 26px;"/>
            <asp:Button ID="btnEdit" runat="server" Text="继续提交" Visible="false" class="easyui-linkbutton" 
            style=" margin-left:20px;width: 65px; height: 26px;" onclick="btnEdit_Click"/>
             <asp:Button ID="btnDelete" runat="server" Text="删除项目" Visible="false" class="easyui-linkbutton" 
             style=" margin-left:20px;width: 65px; height: 26px; color:Red;" 
             onclick="btnDelete_Click" OnClientClick="return ConfirmDelete()"/>
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    function ConfirmDelete() {
        return confirm("确认删除吗？");
    }
</script>
