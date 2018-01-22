﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportOrder.aspx.cs" Inherits="WebApp.Subjects.ADOrders.ImportOrder" %>

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
    <script type="text/javascript">
        var subjectId = '<%=subjectId %>';
        var hasOrder = '<%=hasOrder %>';
        
    </script>
    <style type="text/css">
        .divi
        {
            float: left;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="hfHasErrorMaterialSupport" runat="server" />
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            添加订单
        </p>
    </div>
    <div class="tr">
        >>项目信息</div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                项目编号
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td class="style1">
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style1">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td class="style1">
                所属客户
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style1">
                <asp:Label ID="labCustomer" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                订单类型
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectType" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <div class="tr">
        >>导入订单</div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px; text-align: right;">
                导入原始订单：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div class="divi">
                    <asp:FileUpload ID="FileUpload1" runat="server" />
                </div>
                <div class="divi" style="padding-left: 20px;">
                    <asp:CheckBox ID="cbAdd" runat="server" />追加订单（保留原数据）
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px; text-align: right;">
                下载订单模板：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:LinkButton ID="lbDownLoad" runat="server" OnClick="lbDownLoad_Click">下载模板</asp:LinkButton>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px; text-align: right;">
            </td>
            <td style="text-align: left; padding-left: 5px; height: 50px;">
                <div id="showButton">
                    <asp:Button ID="btnImport" runat="server" Text="导 入" OnClientClick="return checkFile()"
                        class="easyui-linkbutton" Style="width: 65px; height: 26px;" OnClick="btnImport_Click" />
                </div>
                <div id="showWaiting" style="color: Red; display: none;">
                    <img src='../../Image/WaitImg/loadingA.gif' />正在导入，请稍等...
                </div>
            </td>
        </tr>
    </table>
    <asp:Panel ID="Panel1" runat="server" Visible="false">
        <table>
            <tr class="tr_bai">
                <td style="width: 120px; height: 50px;">
                </td>
                <td class="nav_table_tdleft" style="vertical-align: top;">
                    <asp:Label ID="labState" runat="server" Text="导入完成" Style="color: Red; font-weight: bold;
                        font-size: 16px;"></asp:Label>
                    <br />
                    <asp:Label ID="labTips" runat="server" Text="" Style="color: blue; font-size: 14px;"></asp:Label>
                    <div id="ExportFailMsg" runat="server" style="display: none;">
                        <span style="color: red; font-size: 14px;">导入失败信息：</span><asp:LinkButton ID="lbExportError"
                            runat="server" OnClick="lbExportError_Click" Style="text-decoration: underline;">导出</asp:LinkButton>
                    </div>
                    <div id="ExportEmptyFrame" runat="server" style="display: none;">
                        <span style="color: red; font-size: 14px;">存在空器架店铺：</span><asp:LinkButton ID="lbExportEmptyFrame"
                            runat="server" OnClick="lbExportEmptyFrame_Click" Style="text-decoration: underline;">导出</asp:LinkButton>
                    </div>
                    <div id="ExportPOPEmptyFrame" runat="server" style="display: none;">
                        <span style="color: red; font-size: 14px;">POP器架对应错误信息：</span><asp:LinkButton ID="lbExportPOPEmptyFrame"
                            runat="server" OnClick="lbExportPOPEmptyFrame_Click" Style="text-decoration: underline;">导出</asp:LinkButton>
                    </div>
                    <div id="ExportPOPPlaceWarning" runat="server" style="display: none;">
                        <span style="color: red; font-size: 14px;">POP位置占用警告：</span><asp:LinkButton ID="lbExportPOPPlaceWarning"
                            runat="server" OnClick="lbExportPOPPlaceWarning_Click" Style="text-decoration: underline;">导出</asp:LinkButton>
                    </div>
                    <div id="ErrorMaterialSupportWarning" runat="server" style="display: none;">
                        <span style="color: red; font-size: 14px;">物料支持级别不统一警告 ：</span><asp:LinkButton ID="lbExportErrorMaterialSupport"
                            runat="server" OnClick="lbExportErrorMaterialSupport_Click" Style="text-decoration: underline;">导出</asp:LinkButton>
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="loading" runat="server" style="text-align: center; display: none; padding-top: 50px;
                height: 200px;">
                <img src="../../image/WaitImg/loading1.gif" />
                <br />
                正在加载订单，请稍等...
            </div>
            <asp:Panel ID="Panel_POP" runat="server" Visible="false">
                <div class="tab" style="margin-top: 10px;">
                    订单信息
                    <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
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
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
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
                                                    <%#Eval("list.ShopNo")%>
                                                </td>
                                                <td>
                                                    <%#Eval("list.ShopName")%>
                                                </td>
                                                <td>
                                                    <%#Eval("list.RegionName")%>
                                                </td>
                                                <td>
                                                    <%#Eval("list.ProvinceName")%>
                                                </td>
                                                <td>
                                                    <%#Eval("list.CityName")%>
                                                </td>
                                                <td>
                                                    <%#Eval("list.CityTier")%>
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
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
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
                                            <table class="table1" style="width: 1500px;">
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
                                                    </td>--%>
                                                    <%-- <td>
                                                        角落类型
                                                    </td>
                                                    <td>
                                                        系列
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
                                                    <%#Eval("order.ShopNo")%>
                                                </td>
                                                <td>
                                                    <%#Eval("order.ShopName")%>
                                                </td>
                                                <td>
                                                    <%#Eval("order.RegionName")%>
                                                </td>
                                                <td>
                                                    <%#Eval("order.ProvinceName")%>
                                                </td>
                                                <td>
                                                    <%#Eval("order.CityName")%>
                                                </td>
                                                <td>
                                                    <%#Eval("order.CityTier")%>
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
                                                <%-- <td>
                                                    <%#Eval("pop.POPName")%>
                                                </td>
                                                <td>
                                                    <%#Eval("pop.POPType")%>
                                                </td>--%>
                                                <td>
                                                    <%--<%#Eval("order.OrderGender")%>--%>
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
                                                    <%#Eval("order.WindowWide")%>
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
                                                </td>--%>
                                                <%--<td>
                                                    <%#Eval("order.CornerType")%>
                                                </td>
                                                <td>
                                                    <%#Eval("order.Category")%>
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
                            <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
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
                            <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
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
                                            <table class="table1" style="width: 1500px;">
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
                                                    </td>--%>
                                                    <%--<td>
                                                        角落类型
                                                    </td>
                                                    <td>
                                                        系列
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
                                                    <%#Eval("merge.ShopNo")%>
                                                </td>
                                                <td>
                                                    <%#Eval("merge.ShopName")%>
                                                </td>
                                                <td>
                                                    <%#Eval("merge.RegionName")%>
                                                </td>
                                                <td>
                                                    <%#Eval("merge.ProvinceName")%>
                                                </td>
                                                <td>
                                                    <%#Eval("merge.CityName")%>
                                                </td>
                                                <td>
                                                    <%#Eval("merge.CityTier")%>
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
                                                    <%--<%#Eval("merge.OrderGender")%>--%>
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
                                                </td>--%>
                                                <%-- <td>
                                                    <%#Eval("pop.CornerType")%>
                                                </td>
                                                <td>
                                                    <%#Eval("pop.Category")%>
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
                            <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional">
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
                            <asp:UpdatePanel ID="UpdatePanel10" runat="server">
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
                    <br />
                    <div id="Div1" runat="server" style="text-align: center; margin-bottom: 30px;">
                        <asp:Button ID="btnSplit" runat="server" Text="拆分订单" OnClick="btnNext_Click" class="easyui-linkbutton"
                            Style="width: 65px; height: 26px;" />
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnGoNext" runat="server" Text="下一步" class="easyui-linkbutton" Style="width: 65px;
                            height: 26px;" OnClick="btnGoNext_Click" />
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnGoBack" runat="server" Text="返 回" class="easyui-linkbutton" Style="width: 65px;
                            height: 26px;" OnClick="btnGoBack_Click" />
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="lbGoSkip" runat="server" Text="无需拆单" class="easyui-linkbutton" Style="width: 65px;
                            height: 26px;" OnClick="lbGoSkip_Click1" />
                        <img id="imgLoading1" style="display: none;" src='/image/WaitImg/loadingA.gif' />
                    </div>
            </asp:Panel>
            <asp:Panel ID="Panel_Price" runat="server" Visible="false">
                <%--<asp:Panel ID="Panel_PriceList" runat="server" Visible="false">--%>
                <div class="tab" style="margin-top: 10px;">
                    订单信息
                    <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
                </div>
                <div>
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
                                        金额
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
                                    <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.ItemIndex + 1%>
                                </td>
                                <td>
                                    <%#Eval("order.ShopName") %>
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
                                    <%#Eval("order.Contents")%>
                                </td>
                                <td>
                                    <%#Eval("order.Remark") %>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div style="text-align: center; margin-top: 10px;">
                        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
                            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
                        </webdiyer:AspNetPager>
                    </div>
                </div>
                <%--</asp:Panel>--%>
                <div style="text-align: center; margin-top: 20px; margin-bottom: 30px;">
                    <asp:Button ID="btnSubmit" runat="server" Text="提 交" class="easyui-linkbutton" Style="width: 65px;
                        height: 26px;" OnClick="btnSubmit_Click" />
                    <img id="imgSubmitPrice" style="display: none;" src='/image/WaitImg/loadingA.gif' />
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnBack" runat="server" Text="返 回" class="easyui-linkbutton" Style="width: 65px;
                        height: 26px;" OnClick="btnGoBack_Click" />
                </div>
            </asp:Panel>
            <asp:Panel ID="PanelHandMake" runat="server" Visible="false">
                <div class="tab" style="margin-top: 10px;">
                    订单信息
                    <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
                </div>
                <ul id="myTab1" class="nav nav-tabs" style="background-color: #dce0e9;">
                    <li class="active"><a href="#pop1" data-toggle="tab">POP订单</a></li>
                    <%--<li><a href="#hc" data-toggle="tab">HC订单</a></li>--%>
                    <li><a href="#hcmaterial" data-toggle="tab">物料信息</a></li>
                    <li><a href="#hcpriceorder" data-toggle="tab">费用订单</a></li>
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
                                                    位置描述
                                                </td>
                                                <td>
                                                    备注
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
                        <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <%--<table>
                                        <tr>
                                            <td style="width: 60px; color: Blue;">
                                                搜索：
                                            </td>
                                            <td style="text-align: left;">
                                                店铺编号：<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                                                &nbsp;
                                                <asp:Button ID="Button2" runat="server" Text="搜 索" OnClick="btnSreachMaterial_Click"
                                                    class="easyui-linkbutton" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                                            </td>
                                        </tr>
                                    </table>--%>
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
                    <div class="tab-pane fade" id="hcpriceorder" style="padding: 5px;">
                            <asp:UpdatePanel ID="UpdatePanel11" runat="server">
                               <ContentTemplate>
                                   <table>
                                        <tr>
                                            <td style="width: 60px; color: Blue;">
                                                搜索：
                                            </td>
                                            <td style="text-align: left;">
                                                店铺编号：<asp:TextBox ID="txtHCPriceOrderShopNo" runat="server"></asp:TextBox>
                                                &nbsp;
                                                <asp:Button ID="btnSreachHCPriceOrder" runat="server" Text="搜 索" OnClick="btnSreachHCPriceOrder_Click"
                                                    class="easyui-linkbutton" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:Repeater ID="repeater_HCPriceOrderList" runat="server" OnItemDataBound="repeater_HCPriceOrderList_ItemDataBound">
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
                                                    <%#(AspNetPagerHCPriceOrder.CurrentPageIndex - 1) * AspNetPagerHCPriceOrder.PageSize + Container.ItemIndex + 1%>
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
                                            <%if (repeater_HCPriceOrderList.Items.Count == 0)
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
                                        <webdiyer:AspNetPager ID="AspNetPagerHCPriceOrder" runat="server" PageSize="10" CssClass="paginator"
                                            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" showinputbox="Never"
                                            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPagerHCPriceOrder_PageChanged">
                                        </webdiyer:AspNetPager>
                                    </div>
                               </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                </div>
                <div style="text-align: center; margin-top: 20px; margin-bottom: 30px;">
                    <asp:Button ID="btnSubmitHM" runat="server" Text="提 交" class="easyui-linkbutton"
                        Style="width: 65px; height: 26px;" OnClick="btnSubmitHM_Click" OnClientClick="return checkHM();"/>
                    <img id="imgSubmitSIS" style="display: none;" src='/image/WaitImg/loadingA.gif' />
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="Button3" runat="server" Text="返 回" class="easyui-linkbutton" Style="width: 65px;
                        height: 26px;" OnClick="btnGoBack_Click" />
                </div>
            </asp:Panel>
            <asp:HiddenField ID="hfSubmitState" runat="server" />
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="Button1" EventName="click" />
        </Triggers>
    </asp:UpdatePanel>
    <br />
    <br />
    <div style="text-align: center; margin-bottom: 20px;">
        <asp:Button ID="btnBack1" runat="server" Text="返 回" class="easyui-linkbutton" Style="width: 65px;
            height: 26px;" OnClick="btnGoBack_Click" />
    </div>
    <asp:HiddenField ID="hfSubjectType" runat="server" />
    <asp:HiddenField ID="hfGuidanceId" runat="server" />
    <asp:HiddenField ID="hfCustomerId" runat="server" />
    <asp:HiddenField ID="hfHasOrder" runat="server" />
    <asp:Button ID="Button1" runat="server" Text="Button" Style="display: none;" OnClick="Button1_Click" />
    </form>
</body>
</html>
<script src="../../easyui1.4/jquery.min.js"></script>
<script src="../../easyui1.4/jquery.easyui.min.js"></script>
<script src="../js/ImportOrder.js" type="text/javascript"></script>
<script type="text/javascript">


    function loading() {
        $("#imgLoading1").show();
        return true;
    }

    $(function () {
        if (hasOrder == 1) {
            $("#btnBack1").hide();
            $("#loading").show();
            $("#Button1").click();
        }

    })

    function checkHM() {
        $("#imgSubmitSIS").show();
    }

    var postBackId;
    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (sender, e) {
        $("#imgSubmitPrice").hide();
        $("#imgSubmitSIS").hide();
        if (postBackId.indexOf("btnSubmit") != -1 || postBackId.indexOf("btnSubmitSIS") != -1) {
            var state = $("#hfSubmitState").val();
            if (state == 1) {
                $("#imgSubmitSIS").hide();
                alert("提交成功！");
                window.location = "../SubjectList.aspx";
            }
            else {
                $("#imgSubmitSIS").hide();
                alert("提交失败！");
                $("#Button1").click();
            }
        }
    })

    Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(function (sender, e) {
        postBackId = e.get_postBackElement().id;
        if (postBackId.indexOf("btnSubmit") != -1) {
            $("#imgSubmitPrice").show();
        }
        if (postBackId.indexOf("btnSubmitSIS") != -1) {
            $("#imgSubmitSIS").show();
        }
        //        if (postBackId.indexOf("btnSplit") != -1 || postBackId.indexOf("btnGoNext") != -1 || postBackId.indexOf("lbGoSkip") != -1 || postBackId.indexOf("btnSubmitHM") != -1) {
        //            var val = $("#hfHasErrorMaterialSupport").val();
        //            if (val == 1) {
        //              
        //                e.set_cancel(true);
        //                alert("警告：存在物料支持级别不统一的店铺，请更新订单后再进行下一步")
        //            }
        //            else
        //                loading();
        //        }
    })
    

</script>
