﻿<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="ShowOrderDetail.aspx.cs"
    Inherits="WebApp.Subjects.ShowOrderDetail" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <script src="../../easyui1.4/jquery.min.js" type="text/javascript"></script>
    <link href="../../bootstrap-3.3.2-dist/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <script src="../../bootstrap-3.3.2-dist/js/bootstrap.min.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            查看项目明细
        </p>
    </div>
    <div class="tr">
        >>项目信息
    </div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                内部项目编号
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
            </td>
            <td style="text-align: left; padding-left: 5px;">
            </td>
        </tr>
        <tr class="tr_bai">
            <td class="style1">
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style2">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
            <td class="style1">
                外部项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style3">
                <asp:Label ID="labOutSubjectName" runat="server" Text=""></asp:Label>
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
                <asp:Label ID="labOrderType" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
                实施区域
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labPriceBlong" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目类型
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectType" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
                项目分类
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectCategory" runat="server" Text=""></asp:Label>
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
    <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="Panel1" runat="server">
                <div id="loading" runat="server" style="text-align: center; padding-top: 50px; height: 200px;">
                    <img src="../image/WaitImg/loading1.gif" />
                    <br />
                    正在加载订单，请稍等...
                </div>
            </asp:Panel>
            <asp:Panel ID="PanelPOPOrder" runat="server" Visible="false">
                <div class="tab" style="margin-top: 10px;">
                    原始订单信息
                    <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
                </div>
                <ul id="myTab" class="nav nav-tabs" style="background-color: #dce0e9;">
                    <li><a href="#list" data-toggle="tab">List订单</a></li>
                    <li><a href="#pop" data-toggle="tab">POP订单</a></li>
                    <li><a href="#buchong" data-toggle="tab">补充订单</a></li>
                    <li class="active"><a href="#merge" data-toggle="tab">合并订单</a></li>
                    <li><a href="#material" data-toggle="tab">物料信息</a></li>
                    <li><a href="#priceOrder" data-toggle="tab">费用订单</a></li>
                </ul>
                <div id="myTabContent" class="tab-content" style="overflow: auto;">
                    <div class="tab-pane fade" id="list" style="padding: 5px;">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td style="width: 60px; color: Blue;">
                                            搜索：
                                        </td>
                                        <td style="text-align: left;">
                                            店铺编号：<asp:TextBox ID="txtListShopNo" runat="server"></asp:TextBox>
                                            &nbsp;
                                            <asp:Button ID="btnSreachList" runat="server" Text="搜 索" OnClick="btnSreachList_Click"
                                                class="easyui-linkbutton" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:Repeater ID="repeater_List" runat="server">
                                    <HeaderTemplate>
                                        <table class="table1" style="width: 100%;">
                                            <tr class="tr_hui">
                                                <td style="width: 40px;">
                                                    序号
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
                                                    物料支持
                                                </td>
                                                <td>
                                                    店铺规模大小
                                                </td>
                                                <td>
                                                    角落类型
                                                </td>
                                                <td>
                                                    位置
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
                                                    选图
                                                </td>
                                                <td>
                                                    备注
                                                </td>
                                            </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td style="width: 40px;">
                                                <%#(AspNetPagerList.CurrentPageIndex - 1) * AspNetPagerList.PageSize + Container.ItemIndex + 1%>
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
                                                <%#Eval("shop.CityTier")%>
                                            </td>
                                            <td>
                                                <%#Eval("list.MaterialSupport")%>
                                            </td>
                                            <td>
                                                <%#Eval("list.POSScale")%>
                                            </td>
                                            <td>
                                                <%#Eval("list.CornerType")%>
                                            </td>
                                            <td>
                                                <%#Eval("list.Sheet")%>
                                            </td>
                                            <td>
                                                <%#Eval("list.LevelNum")%>
                                            </td>
                                            <td>
                                                <%#Eval("list.Gender")%>
                                            </td>
                                            <td>
                                                <%#Eval("list.Quantity")%>
                                            </td>
                                            <td>
                                                <%#Eval("list.ChooseImg")%>
                                            </td>
                                            <td>
                                                <%#Eval("list.Remark")%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <%if (repeater_List.Items.Count == 0)
                                          { %>
                                        <tr class="tr_bai">
                                            <td colspan="30" style="text-align: center;">
                                                --无数据信息--
                                            </td>
                                        </tr>
                                        <%} %>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                                <div style="text-align: center;">
                                    <webdiyer:AspNetPager ID="AspNetPagerList" runat="server" PageSize="10" CssClass="paginator"
                                        CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                        NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" showinputbox="Never"
                                        CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPagerList_PageChanged">
                                    </webdiyer:AspNetPager>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="tab-pane fade" id="pop" style="padding: 5px;">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td style="width: 60px; color: Blue;">
                                            搜索：
                                        </td>
                                        <td style="text-align: left;">
                                            店铺编号：<asp:TextBox ID="txtPOPShopNo" runat="server"></asp:TextBox>
                                            &nbsp;
                                            <asp:Button ID="btnSreachPOP" runat="server" Text="搜 索" OnClick="btnSreachPOP_Click"
                                                class="easyui-linkbutton" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:Repeater ID="repeater_POPList" runat="server">
                                    <HeaderTemplate>
                                        <table class="table1" style="width: 1800px;">
                                            <tr class="tr_hui">
                                                <td style="width: 40px;">
                                                    序号
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
                                                    物料支持
                                                </td>
                                                <td>
                                                    店铺规模大小
                                                </td>
                                                <td>
                                                    POP编号
                                                </td>
                                                <%--<td>
                                                    POP名称
                                                </td>
                                                <td>
                                                    POP类型
                                                </td>--%>
                                                <td>
                                                    性别
                                                </td>
                                                <td>
                                                    数量
                                                </td>
                                                <td>
                                                    位置
                                                </td>
                                                <td>
                                                    位置描述
                                                </td>
                                                <%--<td>
                                                    位置宽(mm)
                                                </td>
                                                <td>
                                                    位置高(mm)
                                                </td>
                                                <td>
                                                    位置深(mm)
                                                </td>
                                                <td>
                                                    位置规模
                                                </td>--%>
                                                <td>
                                                    POP宽(mm)
                                                </td>
                                                <td>
                                                    POP高(mm)
                                                </td>
                                                <%--<td>
                                                    面积(M2)
                                                </td>--%>
                                                <td>
                                                    POP材质
                                                </td>
                                                <%--<td>
                                                    样式
                                                </td>
                                                <td>
                                                    角落类型
                                                </td>
                                                <td>
                                                    系列
                                                </td>
                                                <td>
                                                    是否标准规格
                                                </td>
                                                <td>
                                                    是否格栅
                                                </td>
                                                <td>
                                                    是否框架
                                                </td>
                                                <td>
                                                    单/双面
                                                </td>
                                                <td>
                                                    是否有玻璃
                                                </td>
                                                <td>
                                                    背景
                                                </td>
                                                <td>
                                                    格栅横向数量
                                                </td>
                                                <td>
                                                    格栅纵向数量
                                                </td>
                                                <td>
                                                    平台长(mm)
                                                </td>
                                                <td>
                                                    平台宽(mm)
                                                </td>
                                                <td>
                                                    平台高(mm)
                                                </td>
                                                <td>
                                                    设备类别
                                                </td>--%>
                                                <td>
                                                    选图
                                                </td>
                                                <td>
                                                    备注
                                                </td>
                                            </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr class="tr_bai">
                                            <td style="width: 40px;">
                                                <%#(AspNetPagerPOP.CurrentPageIndex - 1) * AspNetPagerPOP.PageSize + Container.ItemIndex + 1%>
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
                                                <%#Eval("shop.CityTier")%>
                                            </td>
                                            <td>
                                                <%#Eval("order.MaterialSupport")%>
                                            </td>
                                            <td>
                                                <%#Eval("order.POSScale")%>
                                            </td>
                                            <td>
                                                <%#Eval("order.GraphicNo")%>
                                            </td>
                                            <%--<td>
                                                <%#Eval("pop.POPName")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.POPType")%>
                                            </td>--%>
                                            <td>
                                               <%-- <%#Eval("order.Gender")%>--%>
                                               <%#Eval("order.OrderGender") != null ? Eval("order.OrderGender") : Eval("order.Gender")%>
                                            </td>
                                            <td>
                                                <%#Eval("order.Quantity")%>
                                            </td>
                                            <td>
                                                <%#Eval("order.Sheet")%>
                                            </td>
                                            <td>
                                                <%#Eval("order.PositionDescription")%>
                                            </td>
                                            <%--<td>
                                                <%#Eval("pop.WindowWide")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.WindowHigh")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.WindowDeep")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.WindowSize")%>
                                            </td>--%>
                                            <td>
                                                <%#Eval("order.GraphicWidth")%>
                                            </td>
                                            <td>
                                                <%#Eval("order.GraphicLength")%>
                                            </td>
                                            <%--<td>
                                                <%#Eval("pop.Area")%>
                                            </td>--%>
                                            <td>
                                                <%#Eval("order.GraphicMaterial")%>
                                            </td>
                                            <%--<td>
                                                <%#Eval("pop.Style")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.CornerType")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.Category")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.StandardDimension")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.Modula")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.Frame")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.DoubleFace")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.Glass")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.Backdrop")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.ModulaQuantityWidth")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.ModulaQuantityHeight")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.PlatformLength")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.PlatformWidth")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.PlatformHeight")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.FixtureType")%>
                                            </td>--%>
                                            <td>
                                                <%#Eval("order.ChooseImg")%>
                                            </td>
                                            <td>
                                                <%#Eval("order.Remark")%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <%if (repeater_POPList.Items.Count == 0)
                                          { %>
                                        <tr class="tr_bai">
                                            <td colspan="19" style="text-align: center;">
                                                --无数据信息--
                                            </td>
                                        </tr>
                                        <%} %>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                                <div style="text-align: center;">
                                    <webdiyer:AspNetPager ID="AspNetPagerPOP" runat="server" PageSize="10" CssClass="paginator"
                                        CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                        NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" showinputbox="Never"
                                        CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPagerPOP_PageChanged">
                                    </webdiyer:AspNetPager>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="tab-pane fade" id="buchong" style="padding: 5px; overflow: auto;">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td style="width: 60px; color: Blue;">
                                            搜索：
                                        </td>
                                        <td style="text-align: left;">
                                            店铺编号：<asp:TextBox ID="txtBCShopNo" runat="server"></asp:TextBox>
                                            &nbsp;
                                            <asp:Button ID="btnSreachBC" runat="server" Text="搜 索" OnClick="btnSreachBC_Click"
                                                class="easyui-linkbutton" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:Repeater ID="repeater_BCList" runat="server">
                                    <HeaderTemplate>
                                        <table class="table1" style="width: 100%;">
                                            <tr class="tr_hui">
                                                <td style="width: 40px;">
                                                    序号
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
                                                    位置
                                                </td>
                                                <td>
                                                    性别
                                                </td>
                                                <td>
                                                    数量
                                                </td>
                                                <td>
                                                    选图
                                                </td>
                                                <td>
                                                    备注
                                                </td>
                                            </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td style="width: 40px;">
                                                <%#(AspNetPagerBC.CurrentPageIndex - 1) * AspNetPagerBC.PageSize + Container.ItemIndex + 1%>
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
                                                <%#Eval("shop.CityTier")%>
                                            </td>
                                            <td>
                                                <%#Eval("list.Sheet")%>
                                            </td>
                                            <td>
                                                <%#Eval("list.Gender")%>
                                            </td>
                                            <td>
                                                <%#Eval("list.Quantity")%>
                                            </td>
                                            <td>
                                                <%#Eval("list.ChooseImg")%>
                                            </td>
                                            <td>
                                                <%#Eval("list.Remark")%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <%if (repeater_BCList.Items.Count == 0)
                                          { %>
                                        <tr class="tr_bai">
                                            <td colspan="30" style="text-align: center;">
                                                --无数据信息--
                                            </td>
                                        </tr>
                                        <%} %>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                                <div style="text-align: center;">
                                    <webdiyer:AspNetPager ID="AspNetPagerBC" runat="server" PageSize="10" CssClass="paginator"
                                        CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                        NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" showinputbox="Never"
                                        CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPagerBC_PageChanged">
                                    </webdiyer:AspNetPager>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="tab-pane fade in active" id="merge" style="padding: 5px;">
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td style="width: 60px; color: Blue;">
                                            搜索：
                                        </td>
                                        <td style="text-align: left;">
                                            店铺编号：<asp:TextBox ID="txtMergeShopNo" runat="server"></asp:TextBox>
                                            &nbsp;
                                            <asp:Button ID="btnSreachMerge" runat="server" Text="搜 索" OnClick="btnSreachMerge_Click"
                                                class="easyui-linkbutton" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:Repeater ID="repeater_MergeList" runat="server">
                                    <HeaderTemplate>
                                        <table class="table1" style="width: 1800px;">
                                            <tr class="tr_hui">
                                                <td style="width: 40px;">
                                                    序号
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
                                                    物料支持
                                                </td>
                                                <td>
                                                    店铺规模大小
                                                </td>
                                                <td>
                                                    POP编号
                                                </td>
                                                <%-- <td>
                                                    POP名称
                                                </td>
                                                <td>
                                                    POP类型
                                                </td>--%>
                                                <td>
                                                    性别
                                                </td>
                                                <td>
                                                    数量
                                                </td>
                                                <td>
                                                    位置
                                                </td>
                                                <td>
                                                    级别
                                                </td>
                                                <td>
                                                    位置描述
                                                </td>
                                                <%--<td>
                                                    位置宽(mm)
                                                </td>
                                                <td>
                                                    位置高(mm)
                                                </td>
                                                <td>
                                                    位置深(mm)
                                                </td>
                                                <td>
                                                    位置规模
                                                </td>--%>
                                                <td>
                                                    POP宽(mm)
                                                </td>
                                                <td>
                                                    POP高(mm)
                                                </td>
                                                <%--<td>
                                                    面积(M2)
                                                </td>--%>
                                                <td>
                                                    POP材质
                                                </td>
                                                <%--<td>
                                                    样式
                                                </td>
                                                <td>
                                                    角落类型
                                                </td>
                                                <td>
                                                    系列
                                                </td>
                                                <td>
                                                    是否标准规格
                                                </td>
                                                <td>
                                                    是否格栅
                                                </td>
                                                <td>
                                                    是否框架
                                                </td>
                                                <td>
                                                    单/双面
                                                </td>
                                                <td>
                                                    是否有玻璃
                                                </td>
                                                <td>
                                                    背景
                                                </td>
                                                <td>
                                                    格栅横向数量
                                                </td>
                                                <td>
                                                    格栅纵向数量
                                                </td>
                                                <td>
                                                    平台长
                                                </td>
                                                <td>
                                                    平台宽
                                                </td>
                                                <td>
                                                    平台高
                                                </td>
                                                <td>
                                                    设备类别
                                                </td>--%>
                                                <td>
                                                    选图
                                                </td>
                                                <td>
                                                    备注
                                                </td>
                                            </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr class="tr_bai">
                                            <td style="width: 40px;">
                                                <%#(AspNetPagerMerge.CurrentPageIndex - 1) * AspNetPagerMerge.PageSize + Container.ItemIndex + 1%>
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
                                                <%#Eval("shop.CityTier")%>
                                            </td>
                                            <td>
                                                <%#Eval("merge.MaterialSupport")%>
                                            </td>
                                            <td>
                                                <%#Eval("merge.POSScale")%>
                                            </td>
                                            <td>
                                                <%#Eval("merge.GraphicNo")%>
                                            </td>
                                            <%--<td>
                                                <%#Eval("pop.POPName")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.POPType")%>
                                            </td>--%>
                                            <td>
                                                <%--<%#Eval("merge.Gender")%>--%>
                                                <%#Eval("merge.OrderGender") != null ? Eval("merge.OrderGender") : Eval("merge.Gender")%>
                                            </td>
                                            <td>
                                                <%#Eval("merge.Quantity")%>
                                            </td>
                                            <td>
                                                <%#Eval("merge.Sheet")%>
                                            </td>
                                            <td>
                                                <%#Eval("merge.LevelNum")%>
                                            </td>
                                            <td>
                                                <%#Eval("merge.PositionDescription")%>
                                            </td>
                                            <%--<td>
                                                <%#Eval("pop.WindowWide")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.WindowHigh")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.WindowDeep")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.WindowSize")%>
                                            </td>--%>
                                            <td>
                                                <%#Eval("merge.GraphicWidth")%>
                                            </td>
                                            <td>
                                                <%#Eval("merge.GraphicLength")%>
                                            </td>
                                            <%--<td>
                                                <%#Eval("pop.Area")%>
                                            </td>--%>
                                            <td>
                                                <%#Eval("merge.GraphicMaterial")%>
                                            </td>
                                            <%--<td>
                                                <%#Eval("pop.Style")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.CornerType")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.Category")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.StandardDimension")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.Modula")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.Frame")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.DoubleFace")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.Glass")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.Backdrop")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.ModulaQuantityWidth")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.ModulaQuantityHeight")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.PlatformLength")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.PlatformWidth")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.PlatformHeight")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.FixtureType")%>
                                            </td>--%>
                                            <td>
                                                <%#Eval("merge.ChooseImg")%>
                                            </td>
                                            <td>
                                                <%#Eval("merge.Remark")%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <%if (repeater_MergeList.Items.Count == 0)
                                          { %>
                                        <tr class="tr_bai">
                                            <td colspan="19" style="text-align: center;">
                                                --无数据信息--
                                            </td>
                                        </tr>
                                        <%} %>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                                <div style="text-align: center;">
                                    <webdiyer:AspNetPager ID="AspNetPagerMerge" runat="server" PageSize="10" CssClass="paginator"
                                        CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                        NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" showinputbox="Never"
                                        CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPagerMerge_PageChanged">
                                    </webdiyer:AspNetPager>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="tab-pane fade" id="material" style="padding: 5px;">
                        <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                
                                <table>
                                    <tr>
                                        <td style="width: 60px; color: Blue;">
                                            搜索：
                                        </td>
                                        <td style="text-align: left;">
                                            店铺编号：<asp:TextBox ID="txtMaterialShopNo" runat="server"></asp:TextBox>
                                            &nbsp;
                                            <asp:Button ID="btnSreachMaterial" runat="server" Text="搜 索" OnClick="btnSreachMaterial_Click"
                                                class="easyui-linkbutton" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:Repeater ID="repeater_Material" runat="server">
                                    <HeaderTemplate>
                                        <table class="table1" style="width: 100%;">
                                            <tr class="tr_hui">
                                                <td style="width: 40px;">
                                                    序号
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
                                                <%#(AspNetPagerMaterial.CurrentPageIndex - 1) * AspNetPagerMaterial.PageSize + Container.ItemIndex + 1%>
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
                                                <%#Eval("material.Sheet")%>
                                            </td>
                                            <td>
                                                <%#Eval("material.MaterialName")%>
                                            </td>
                                            <td>
                                                <%#Eval("material.MaterialCount")%>
                                            </td>
                                            <td>
                                                <%#Eval("material.MaterialLength")%>
                                            </td>
                                            <td>
                                                <%#Eval("material.MaterialWidth")%>
                                            </td>
                                            <td>
                                                <%#Eval("material.MaterialHigh")%>
                                            </td>
                                            <td>
                                                <%#Eval("material.Price")%>
                                            </td>
                                            <td>
                                                <%#Eval("material.Remark")%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <%if (repeater_Material.Items.Count == 0)
                                          { %>
                                        <tr class="tr_bai">
                                            <td colspan="20" style="text-align: center;">
                                                --无数据信息--
                                            </td>
                                        </tr>
                                        <%} %>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                                <div style="text-align: center;">
                                    <webdiyer:AspNetPager ID="AspNetPagerMaterial" runat="server" PageSize="10" CssClass="paginator"
                                        CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                        NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" showinputbox="Never"
                                        CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPagerMaterial_PageChanged">
                                    </webdiyer:AspNetPager>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="tab-pane fade" id="priceOrder" style="padding: 5px;">
                            <asp:UpdatePanel ID="UpdatePanel12" runat="server">
                               <ContentTemplate>
                                   <table>
                                        <tr>
                                            <td style="width: 60px; color: Blue;">
                                                搜索：
                                            </td>
                                            <td style="text-align: left;">
                                                店铺编号：<asp:TextBox ID="txtPriceOrderShopNo" runat="server"></asp:TextBox>
                                                &nbsp;
                                                <asp:Button ID="btnSreachPriceOrder" runat="server" Text="搜 索" OnClick="btnSreachPriceOrder_Click"
                                                    class="easyui-linkbutton" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:Repeater ID="repeater_OrderPrice" runat="server" OnItemDataBound="orderListRepeater_ItemDataBound">
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
                                                        应收金额
                                                    </td>
                                                    <td>
                                                        应付金额
                                                    </td>
                                                    <td>
                                                        备注
                                                    </td>
                                                </tr>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td style="width: 40px;">
                                                    <%#(AspNetPagerPriceOrder.CurrentPageIndex - 1) * AspNetPagerPriceOrder.PageSize + Container.ItemIndex + 1%>
                                                </td>
                                                <td>
                                                    <asp:Label ID="labPriceType" runat="server" Text=""></asp:Label>
                                                </td>
                                                <td>
                                                    <%#Eval("ShopNo")%>
                                                </td>
                                                <td>
                                                    <%#Eval("ShopName")%>
                                                </td>
                                                <td>
                                                    <%#Eval("Region")%>
                                                </td>
                                                <td>
                                                    <%#Eval("Province")%>
                                                </td>
                                                <td>
                                                    <%#Eval("City")%>
                                                </td>
                                                <td>
                                                    <%#Eval("Amount")%>
                                                </td>
                                                <td>
                                                    <%#Eval("PayAmount")%>
                                                </td>
                                                <td>
                                                    <%#Eval("Remark")%>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <%if (repeater_OrderPrice.Items.Count == 0)
                                              { %>
                                            <tr class="tr_bai">
                                                <td colspan="10" style="text-align: center;">
                                                    --无数据信息--
                                                </td>
                                            </tr>
                                            <%} %>
                                            </table>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                    <div style="text-align: center;">
                                        <webdiyer:AspNetPager ID="AspNetPagerPriceOrder" runat="server" PageSize="10" CssClass="paginator"
                                            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" showinputbox="Never"
                                            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPagerPriceOrder_PageChanged">
                                        </webdiyer:AspNetPager>
                                    </div>
                               </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                </div>
                <!--方案信息-->
                <div class="tab" style="margin-top: 10px;">
                    <span style="font-weight: bold;">拆单方案</span>
                    <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
                </div>
                <div id="showLoading" style="text-align: center;">
                    <img src="../../image/WaitImg/loading1.gif" />
                </div>
                <table id="tbPlanList1" style="width: 100%;">
                </table>
                <div class="tab" style="margin-top: 10px;">
                    <span style="font-weight: bold;">拆分后订单信息</span> &nbsp;&nbsp;
                    <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
                </div>
                <asp:Panel ID="PanelFinalOrder" runat="server" Visible="false">
                    <asp:UpdatePanel ID="UpdatePanel10" runat="server">
                        <ContentTemplate>
                            <table style="width: 98%; margin-left: 5px;">
                                <tr>
                                    <td style="width: 80px;">
                                        店铺编号：
                                    </td>
                                    <td style="width: 120px; text-align: left;">
                                        <asp:TextBox ID="txtShopNo" runat="server"></asp:TextBox>
                                    </td>
                                    <td style="width: 90px; text-align: center;">
                                        店铺名称：
                                    </td>
                                    <td style="width: 120px; text-align: left;">
                                        <asp:TextBox ID="txtShopName" runat="server" Style="width: 150px;"></asp:TextBox>
                                    </td>
                                    <td style="text-align: left;">
                                        <asp:Button ID="btnSearchFinal" runat="server" Text="查 询" class="easyui-linkbutton"
                                            Style="width: 65px; height: 26px; margin-left: 10px;" OnClick="btnSearchFinal_Click" />
                                        <img id="loadSearch" src="/image/WaitImg/loadingA.gif" alt="" style="display: none;" />
                                        <asp:Button ID="btnExportAll" runat="server" Text="导出全部" class="easyui-linkbutton"
                                            Style="width: 65px; height: 26px; margin-left: 10px;" OnClientClick="return check(this,0)" />
                                        <img id="loadExportAll" src="/image/WaitImg/loadingA.gif" alt="" style="display: none;" />
                                        <asp:Button ID="btnExportNotNull" runat="server" Text="导出非空(不含空尺寸)" class="easyui-linkbutton"
                                            Style="width: 150px; height: 26px; margin-left: 10px;" OnClientClick="return check(this,1)" />
                                        <img id="Img1" alt="" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                                    </td>
                                </tr>
                            </table>
                            <div class="tab" style="overflow: auto;">
                                <asp:Repeater ID="repeater_FinalOrder" runat="server" 
                                    onitemdatabound="repeater_FinalOrder_ItemDataBound">
                                    <HeaderTemplate>
                                        <%if (repeater_FinalOrder.Items.Count == 0)
                                          { %>
                                        <table class="table">
                                            <%}
                                          else
                                          { %>
                                            <table class="table1" style="width: 1800px;">
                                                <tr class="tr_hui">
                                                    <td style="width: 40px;">
                                                        序号
                                                    </td>
                                                    <td style="width: 50px;">
                                                        订单类型
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
                                                        店铺规模大小
                                                    </td>
                                                    <td>
                                                        物料支持
                                                    </td>
                                                    <td>
                                                        位置
                                                    </td>
                                                    <td>
                                                        级别
                                                    </td>
                                                    <td>
                                                        位置描述
                                                    </td>
                                                    <td>
                                                        器架类型
                                                    </td>
                                                    <td>
                                                        性别
                                                    </td>
                                                    <td>
                                                        数量
                                                    </td>
                                                    <td>
                                                        POP宽(mm)
                                                    </td>
                                                    <td>
                                                        POP高(mm)
                                                    </td>
                                                    <td>
                                                        面积(M2)
                                                    </td>
                                                    <td>
                                                        POP材质
                                                    </td>
                                                    <%--<td>
                                                        系列
                                                    </td>--%>
                                                    <td>
                                                        选图
                                                    </td>
                                                    <td>
                                                        备注
                                                    </td>
                                                </tr>
                                                <%} %>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr class="tr_bai">
                                            <td style="width: 40px;">
                                                <%#Container.ItemIndex + 1 + ((AspNetPagerFinal.CurrentPageIndex - 1) * AspNetPagerFinal.PageSize)%>
                                            </td>
                                            <td style="width: 50px;">
                                                <%--<%#Eval("order.OrderType") != null ? Eval("order.OrderType").ToString() == "1" ? "pop" : "道具" : "POP"%>--%>
                                                <asp:Label ID="labOrderType" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td>
                                                <%--店铺编号--%>
                                                <%#Eval("order.ShopNo")%>
                                            </td>
                                            <td>
                                                <%--店铺名称--%>
                                                <%#Eval("order.ShopName")%>
                                            </td>
                                            <td>
                                                <%--区域--%>
                                                <%#Eval("order.Region")%>
                                            </td>
                                            <td>
                                                <%--省份--%>
                                                <%#Eval("order.Province")%>
                                            </td>
                                            <td>
                                                <%--城市--%>
                                                <%#Eval("order.City")%>
                                            </td>
                                            <td>
                                                <%--城市级别--%>
                                                <%#Eval("order.CityTier")%>
                                            </td>
                                            <td>
                                                <%#Eval("order.POSScale")%>
                                            </td>
                                            <td>
                                                <%#Eval("order.MaterialSupport")%>
                                            </td>
                                            <td>
                                                <%--位置--%>
                                                <%#Eval("order.Sheet")%>
                                            </td>
                                            <td>
                                                <%#Eval("order.LevelNum")%>
                                            </td>
                                            <td>
                                                <%--位置描述--%>
                                                <%#Eval("order.PositionDescription")%>
                                            </td>
                                            <td>
                                                <%--器架类型--%>
                                                <%#Eval("order.MachineFrame")%>
                                            </td>
                                            <td>
                                                <%--性别--%>
                                               <%-- <%#Eval("order.Gender")%>--%>
                                               <%#Eval("order.OrderGender") != null ? Eval("order.OrderGender") : Eval("order.Gender")%>
                                            </td>
                                            <td>
                                                <%--数量--%>
                                                <%#Eval("order.Quantity")%>
                                            </td>
                                            <td>
                                                <%--POP宽(mm)--%>
                                                <%#Eval("order.GraphicWidth")%>
                                            </td>
                                            <td>
                                                <%-- POP高(mm)--%>
                                                <%#Eval("order.GraphicLength")%>
                                            </td>
                                            <td>
                                                <%--面积(M2)--%>
                                                <%#Eval("order.Area")%>
                                            </td>
                                            <td>
                                                <%--POP材质--%>
                                                <%#Eval("order.GraphicMaterial")%>
                                            </td>
                                            <%--<td>
                                               
                                                <%#Eval("pop") != null ? Eval("pop.Category") : ""%>
                                            </td>--%>
                                            <td>
                                                <%--选图--%>
                                                <%#Eval("order.ChooseImg")%>
                                            </td>
                                            <td>
                                                <%--备注--%>
                                                <%#Eval("order.Remark")%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <%if (repeater_FinalOrder.Items.Count == 0)
                                          {%>
                                        <tr class="tr_bai">
                                            <td colspan="21" style="text-align: center;">
                                                --无数据信息--
                                            </td>
                                        </tr>
                                        <%} %>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                            <div class="tab" style="text-align: center;">
                                <webdiyer:AspNetPager ID="AspNetPagerFinal" runat="server" PageSize="20" CssClass="paginator"
                                    CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                    NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                                    CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPagerFinal_PageChanged">
                                </webdiyer:AspNetPager>
                            </div>
                            <table class="table" style=" margin-top :20px; width:99%;">
                                <tr class="tr_hui">
                                    <td style="width: 120px;">
                                        店铺数量：
                                    </td>
                                    <td style="width: 120px; text-align: left; padding-left: 5px;">
                                        <asp:Label ID="labShopCount" runat="server" Text=""></asp:Label>
                                    </td>
                                    <td style="width: 120px;">
                                        总面积：
                                    </td>
                                    <td style="width: 150px; text-align: left; padding-left: 5px;">
                                        <asp:Label ID="labTotalArea" runat="server" Text=""></asp:Label>
                                    </td>
                                    <td style="width: 120px;">
                                        POP总金额：
                                    </td>
                                    <td style="text-align: left; padding-left: 5px;">
                                        <asp:Label ID="labTotalPrice" runat="server" Text=""></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:Panel>
            </asp:Panel>
            <asp:Panel ID="PanelPriceOrder" runat="server" Visible="false">
                <div class="tab" style="margin-top: 10px;">
                    订单信息
                    <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
                </div>
                <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Repeater ID="repeater_PriceList" runat="server">
                            <HeaderTemplate>
                                <table class="table">
                                    <tr class="tr_hui">
                                        <td>
                                            序号
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
                                            应收金额
                                        </td>
                                        <td>
                                            应付金额
                                        </td>
                                        <td>
                                            费用内容
                                        </td>
                                        <td>
                                            备注
                                        </td>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr class="tr_bai">
                                    <td>
                                        <%# (AspNetPagerPrice.CurrentPageIndex - 1) * AspNetPagerPrice.PageSize + Container.ItemIndex + 1%>
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
                                        <%#Eval("order.PayAmount")%>
                                    </td>
                                    <td>
                                        <%#Eval("order.Contents")%>
                                    </td>
                                    <td>
                                        <%#Eval("order.Remark") %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                <%if (repeater_PriceList.Items.Count == 0)
                                  { %>
                                <tr class="tr_bai">
                                    <td colspan="10" style="text-align: center;">
                                        --无数据信息--
                                    </td>
                                </tr>
                                <%} %>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                        <div style="text-align: center; margin-top: 10px;">
                            <webdiyer:AspNetPager ID="AspNetPagerPrice" runat="server" PageSize="10" CssClass="paginator"
                                CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                                CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPagerPrice_PageChanged">
                            </webdiyer:AspNetPager>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
            <asp:Panel ID="PanelSupplementOrder" runat="server" Visible="false">
                <div class="tab" style="margin-top: 10px;">
                    订单信息
                    <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
                </div>
                <ul id="myTab1" class="nav nav-tabs" style="background-color: #dce0e9;">
                    <li class="active"><a href="#pop1" data-toggle="tab">POP订单</a></li>
                    <li><a href="#hc" data-toggle="tab">HC订单</a></li>
                    <li><a href="#hcmaterial" data-toggle="tab">物料信息</a></li>
                </ul>
                <div id="myTabContent1" class="tab-content" style="overflow: auto;">
                    <div class="tab-pane fade in active" id="pop1" style="padding: 5px;">
                        <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td style="width: 60px; color: Blue;">
                                            搜索：
                                        </td>
                                        <td style="text-align: left;">
                                            店铺编号：<asp:TextBox ID="txtPOP1ShopNo" runat="server"></asp:TextBox>
                                            &nbsp;
                                            <asp:Button ID="btnSreachPOP1" runat="server" Text="搜 索" OnClick="btnSreachPOP1_Click"
                                                class="easyui-linkbutton" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:Repeater ID="repeater_POP1List" runat="server">
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
                                                    店铺规模大小
                                                </td>
                                                <td>
                                                    物料支持级别
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
                                                    位置
                                                </td>
                                                <td>
                                                    系列
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
                                                    位置描述
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
                                                <%#(AspNetPagerPOP1.CurrentPageIndex - 1) * AspNetPagerPOP1.PageSize + Container.ItemIndex + 1%>
                                            </td>
                                            <td>
                                                <%#Eval("shop.ShopNo") %>
                                            </td>
                                            <td>
                                                <%#Eval("order.POSScale")%>
                                            </td>
                                            <td>
                                                <%#Eval("order.MaterialSupport")%>
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
                                                <%#Eval("order.Sheet") %>
                                            </td>
                                            <td>
                                                <%#Eval("order.Category") %>
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
                                                <%#Eval("order.PositionDescription")%>
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
                                        <%if (repeater_POP1List.Items.Count == 0)
                                          { %>
                                        <tr class="tr_bai">
                                            <td colspan="20" style="text-align: center;">
                                                --无数据信息--
                                            </td>
                                        </tr>
                                        <%} %>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                                <div style="text-align: center; margin-top: 10px;">
                                    <webdiyer:AspNetPager ID="AspNetPagerPOP1" runat="server" PageSize="20" CssClass="paginator"
                                        CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                        NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                                        CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPagerPOP1_PageChanged">
                                    </webdiyer:AspNetPager>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="tab-pane fade" id="hc" style="padding: 5px;">
                        <asp:UpdatePanel ID="UpdatePanel9" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table>
                                    <tr>
                                        <td style="width: 60px; color: Blue;">
                                            搜索：
                                        </td>
                                        <td style="text-align: left;">
                                            店铺编号：<asp:TextBox ID="txtHCShopNo" runat="server"></asp:TextBox>
                                            &nbsp;
                                            <asp:Button ID="btnSreachHC" runat="server" Text="搜 索" OnClick="btnSreachHC_Click"
                                                class="easyui-linkbutton" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:Repeater ID="repeater_HCList" runat="server">
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
                                                    店铺规模大小
                                                </td>
                                                <td>
                                                    物料支持级别
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
                                                    位置
                                                </td>
                                                <td>
                                                    系列
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
                                                    材质
                                                </td>
                                                <td>
                                                    选图
                                                </td>
                                                <td>
                                                    安装位置描述
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
                                                <%#(AspNetPagerHC.CurrentPageIndex - 1) * AspNetPagerHC.PageSize + Container.ItemIndex + 1%>
                                            </td>
                                            <td>
                                                <%#Eval("shop.ShopNo") %>
                                            </td>
                                            <td>
                                                <%#Eval("order.POSScale")%>
                                            </td>
                                            <td>
                                                <%#Eval("order.MaterialSupport")%>
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
                                                <%#Eval("order.Sheet") %>
                                            </td>
                                            <td>
                                                <%#Eval("order.Category")%>
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
                                                <%#Eval("order.PositionDescription")%>
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
                                        <%if (repeater_HCList.Items.Count == 0)
                                          { %>
                                        <tr class="tr_bai">
                                            <td colspan="20" style="text-align: center;">
                                                --无数据信息--
                                            </td>
                                        </tr>
                                        <%} %>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                                <div style="text-align: center; margin-top: 10px;">
                                    <webdiyer:AspNetPager ID="AspNetPagerHC" runat="server" PageSize="20" CssClass="paginator"
                                        CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                        NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                                        CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPagerHC_PageChanged">
                                    </webdiyer:AspNetPager>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="tab-pane fade" id="hcmaterial" style="padding: 5px;">
                        <asp:UpdatePanel ID="UpdatePanel11" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <table>
                                        <tr>
                                            <td style="width: 60px; color: Blue;">
                                                搜索：
                                            </td>
                                            <td style="text-align: left;">
                                                店铺编号：<asp:TextBox ID="txthcmaterialShopNo" runat="server"></asp:TextBox>
                                                &nbsp;
                                                <asp:Button ID="btnSreachHCMaterial" runat="server" Text="搜 索" OnClick="btnSreachHCMaterial_Click"
                                                    class="easyui-linkbutton" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                                            </td>
                                        </tr>
                                    </table>
                                <asp:Repeater ID="repeater_HCmaterial" runat="server">
                                    <HeaderTemplate>
                                        <table class="table1" style="width: 100%;">
                                            <tr class="tr_hui">
                                                <td style="width: 40px;">
                                                    序号
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
                                                <%#(AspNetPagerHCMaterial.CurrentPageIndex - 1) * AspNetPagerHCMaterial.PageSize + Container.ItemIndex + 1%>
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
                                                <%#Eval("material.Sheet")%>
                                            </td>
                                            <td>
                                                <%#Eval("material.MaterialName")%>
                                            </td>
                                            <td>
                                                <%#Eval("material.MaterialCount")%>
                                            </td>
                                            <td>
                                                <%#Eval("material.MaterialLength")%>
                                            </td>
                                            <td>
                                                <%#Eval("material.MaterialWidth")%>
                                            </td>
                                            <td>
                                                <%#Eval("material.MaterialHigh")%>
                                            </td>
                                            <td>
                                                <%#Eval("material.Price")%>
                                            </td>
                                            <td>
                                                <%#Eval("material.Remark")%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <%if (repeater_HCmaterial.Items.Count == 0)
                                          { %>
                                        <tr class="tr_bai">
                                            <td colspan="20" style="text-align: center;">
                                                --无数据信息--
                                            </td>
                                        </tr>
                                        <%} %>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                                <div style="text-align: center;">
                                    <webdiyer:AspNetPager ID="AspNetPagerHCMaterial" runat="server" PageSize="10" CssClass="paginator"
                                        CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                        NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" showinputbox="Never"
                                        CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPagerHCMaterial_PageChanged">
                                    </webdiyer:AspNetPager>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="PanelSecondInstall" runat="server" Visible="false">
                <div class="tab" style="margin-top: 10px;">
                    二次安装费明细
                    <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
                </div>
                <div style="overflow: auto;">
                    <asp:Repeater ID="detailList" runat="server">
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
                                        长
                                    </td>
                                    <td>
                                        宽
                                    </td>
                                    <td>
                                        数量
                                    </td>
                                    <td>
                                        备注
                                    </td>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td style="width: 30px;">
                                    <%#(AspNetPagerInstallDetail.CurrentPageIndex - 1) * AspNetPagerInstallDetail.PageSize + Container.ItemIndex + 1%>
                                </td>
                                <td>
                                    <%#Eval("shop.ShopNo") %>
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
                                    <%#Eval("detail.Sheet")%>
                                </td>
                                <td>
                                    <%#Eval("detail.GraphicWidth")%>
                                </td>
                                <td>
                                    <%#Eval("detail.GraphicLength")%>
                                </td>
                                <td>
                                    <%#Eval("detail.Quantity")%>
                                </td>
                                <td>
                                    <%#Eval("detail.Remark")%>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            <%if (detailList.Items.Count == 0)
                              { %>
                            <tr class="tr_bai">
                                <td colspan="11" style="text-align: center;">
                                    --无信息--
                                </td>
                            </tr>
                            <%} %>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
                <div style="text-align: center; margin-top: 10px;">
                    <webdiyer:AspNetPager ID="AspNetPagerInstallDetail" runat="server" PageSize="20"
                        CssClass="paginator" CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页"
                        LastPageText="尾页" NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left"
                        ShowInputBox="Never" CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPagerInstallDetail_PageChanged">
                    </webdiyer:AspNetPager>
                </div>
            </asp:Panel>
            <asp:Panel ID="PanelSISOrder" runat="server" Visible="false">
                <div class="tab" style="margin-top: 10px;">
                    SIS订单明细
                    <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
                </div>
                <div>
                    <div style="width: 100%; overflow: auto;">
                        <asp:Repeater ID="repeaterSIS" runat="server">
                            <HeaderTemplate>
                                <table class="table" style="min-width: 1500px;">
                                    <tr class="tr_hui">
                                        <td style="width: 50px;">
                                            序号
                                        </td>
                                        <td>
                                            店铺编号
                                        </td>
                                        <td>
                                            店铺名称
                                        </td>
                                        <td>
                                            店铺规模大小
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
                                            宽
                                        </td>
                                        <td>
                                            高
                                        </td>
                                        <td>
                                            材质
                                        </td>
                                        <td>
                                            选图
                                        </td>
                                        <td>
                                            位置描述
                                        </td>
                                        <td>
                                            备注
                                        </td>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr class="tr_bai">
                                    <td>
                                        <%# (AspNetPagerSIS.CurrentPageIndex - 1) * AspNetPagerSIS.PageSize + Container.ItemIndex + 1%>
                                    </td>
                                    <td>
                                        <%#Eval("shop.ShopNo") %>
                                    </td>
                                    <td>
                                        <%#Eval("shop.ShopName") %>
                                    </td>
                                    <td>
                                        <%#Eval("order.POSScale")%>
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
                                        <%#Eval("order.PositionDescription")%>
                                    </td>
                                    <td>
                                        <%#Eval("order.Remark")%>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                    <div style="text-align: center; margin-top: 10px;">
                        <webdiyer:AspNetPager ID="AspNetPagerSIS" runat="server" PageSize="20" CssClass="paginator"
                            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPagerSIS_PageChanged">
                        </webdiyer:AspNetPager>
                    </div>
                </div>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="Button1" EventName="click" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:Panel ID="Panel_ApproveInfo" runat="server" Visible="false">
        <br />
        <div class="tr">
            >>审批记录</div>
        <div id="approveInfoDiv" runat="server">
        </div>
    </asp:Panel>
    <div style="text-align: center; margin-top: 20px; margin-bottom: 30px;">
        <input type="button" value="返 回" onclick="javascript:window.history.go(-1)" class="easyui-linkbutton"
            style="width: 65px; height: 26px;" />
        <asp:Button ID="btnEdit" runat="server" Text="继续提交" Visible="false" class="easyui-linkbutton" 
            style=" margin-left:20px;width: 65px; height: 26px;" onclick="btnEdit_Click"/>
            <asp:Button ID="btnDelete" runat="server" Text="删除项目" Visible="false" class="easyui-linkbutton" 
             style=" margin-left:20px;width: 65px; height: 26px; color:Red;" 
             onclick="btnDelete_Click" OnClientClick="return ConfirmDelete()"/>
    </div>
    <asp:Button ID="Button1" runat="server" Text="Button" Style="display: none;" OnClick="Button1_Click" />
    <asp:HiddenField ID="hfSubjectType" runat="server" />
    <asp:HiddenField ID="hfPlanIds" runat="server" />
    <div style="display: none;">
        <iframe name="exportFrame" id="exportFrame" src=""></iframe>
    </div>
    </form>
</body>
</html>
<script src="../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
<script src="../easyui1.4/jquery.easyui.min.js" type="text/javascript"></script>
<script src="../easyui1.4/datagrid-detailview.js" type="text/javascript"></script>
<script type="text/javascript">
    var subjectId = '<%=subjectId %>';
    $(function () {
        $("#Button1").click();

    })
    function ConfirmDelete() {
        return confirm("确认删除吗？");
    }

    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (sender, e) {



        var planIds = $("#hfPlanIds").val() || "";
        var Plan = {
            getList: function () {

                $.ajax({
                    type: "get",
                    url: "/Subjects/Handler/SplitOrder1.ashx?type=getList&Ids=" + planIds + "&t=" + Math.random() * 1000,
                    success: function (data) {

                        $("#showLoading").hide();
                        $("#tbPlanList1").datagrid({
                            data: eval(data),
                            columns: [[
                            { field: 'Id', hidden: true },
                            { field: 'ShopNos1', title: '店铺编号',
                                formatter: function (value, rec) {
                                    var val = rec.ShopNos;
                                    val = val.length > 15 ? (val.substring(0, 15) + "...") : val;

                                    return val;
                                }

                            },
                            { field: 'PositionName', title: 'POP位置', sortable: true },
                            { field: 'RegionNames', title: '区域' },
                            {
                                field: 'ProvinceName1', title: '省份', formatter: function (value, row, index) {
                                    var provinceNames = row.ProvinceName;

                                    if (provinceNames.length > 20) {
                                        provinceNames = provinceNames.substring(0, 20) + "...<span style='color:blue;cursor:pointer;'>更多</span>";
                                    }
                                    return '<span>' + provinceNames + '</span>';
                                }
                            },
                            {
                                field: 'CityName1', title: '城市', formatter: function (value, row, index) {
                                    var cityNames = row.CityName;
                                    if (cityNames.length > 20) {
                                        cityNames = cityNames.substring(0, 20) + "...<span style='color:blue;cursor:pointer;'>更多</span>";
                                    }
                                    return '<span>' + cityNames + '</span>';
                                }
                            },
                            { field: 'Gender', title: '性别', sortable: true },
                            { field: 'CornerType', title: '角落类型', sortable: true },
                            { field: 'MachineFrameNames', title: '器架类型', sortable: true },
                            { field: 'CityTier', title: '城市级别', sortable: true },
                            { field: 'Format', title: '店铺类型', sortable: true },
                            { field: 'MaterialSupport', title: '物料支持类别', sortable: true },
                            { field: 'POSScale', title: '店铺规模', sortable: true },
                            { field: 'IsInstall', title: '是否安装', sortable: true },
                            { field: 'Quantity', title: '数量' },
                            { field: 'GraphicMaterial', title: 'POP材质' },
                            { field: 'POPSize', title: 'POP尺寸', sortable: true },
                            { field: 'WindowSize', title: 'Window尺寸' },
                            { field: 'IsElectricity', title: '通电否' },
                            { field: 'ChooseImg', title: '选图' },
                            { field: 'KeepPOPSize', title: '是否保留POP原尺寸' }
                ]],
                            singleSelect: true,
                            //toolbar: '#toolbar',
                            striped: true,
                            border: false,
                            pagination: false,
                            fitColumns: false,
                            height: 'auto',
                            fit: false,
                            iconCls: 'icon-save',
                            emptyMsg: '没有相关记录',
                            onLoadSuccess: function (data) {
                                //alert("loadOk");
                            },
                            onClickRow: function (rowIndex, rowData) {
                                $(this).datagrid("unselectRow", rowIndex);

                            },
                            view: detailview,
                            detailFormatter: function (index, row) {
                                return '<div style="padding:2px;"> <table id="ddv_' + index + '"></table></div>';
                            },
                            onExpandRow: function (index, row) {
                                commonExpandRow(index, row, this, 'ddv');
                            }

                        });
                    }
                })

            }
        }

        function commonExpandRow(index, row, target, nextName) {

            var curName = nextName + '_' + index;
            var id = row.Id;
            $('#' + curName).datagrid({
                method: 'get',
                url: '/Subjects/Handler/SplitOrder.ashx',
                cache: false,
                queryParams: { planId: id, type: "getDetail", t: Math.random() * 1000 },
                fitColumns: false,
                height: 'auto',
                pagination: false,
                //singleSelect: true,

                columns: [[
                    { field: 'Id', hidden: true },
                    { field: 'OrderType', title: '类型' },
                    { field: 'GraphicWidth', title: '宽' },
                    { field: 'GraphicLength', title: '高' },
                    { field: 'GraphicMaterial', title: '材质' },
                    { field: 'Supplier', title: '供货方' },
                    { field: 'RackSalePrice', title: '道具销售价' },
                    { field: 'Quantity', title: '数量' },
                    { field: 'NewChooseImg', title: '新选图' },
                    { field: 'Remark', title: '备注' },
                    { field: 'IsInSet', title: '男女共一套' }


            ]],
                onResize: function () {
                    $(target).datagrid('fixDetailRowHeight', index);
                },
                onLoadSuccess: function (data) {
                    setTimeout(function () {
                        $(target).datagrid('fixDetailRowHeight', index);
                    }, 0);
                },
                onClickRow: function (rowIndex, rowData) {
                    $(this).datagrid("unselectRow", rowIndex);

                }
            });
            $(target).datagrid('fixDetailRowHeight', index);

        }

        Plan.getList();


    })

    function check(obj, isFilter) {
        $("#btnExportAll,#btnExportNotNull").attr("disabled", true);

        $(obj).next("img").show();
        $("#exportFrame").attr("src", "ShowOrderDetailExport.aspx?subjectId=" + subjectId + "&isFilter=" + isFilter);
        checkExport(obj);
        return false;
    }

    var timer;
    function checkExport(obj) {
        timer = setInterval(function () {
            $.ajax({
                type: "get",
                url: "/Subjects/Handler/CheckExport.ashx",
                cache: false,
                success: function (data) {
                    if (data == "empty") {
                        $("#btnExportAll,#btnExportNotNull").attr("disabled", false);
                        $("#exportFrame").attr("src", "");
                        $(obj).next("img").hide();
                        clearInterval(timer);
                        alert("没有数据可以导出！");
                    }
                    else if (data == "ok") {
                        $("#btnExportAll,#btnExportNotNull").attr("disabled", false);
                        $("#exportFrame").attr("src", "");
                        $(obj).next("img").hide();
                        clearInterval(timer);
                    }

                }
            })

        }, 1000);
    }
</script>
