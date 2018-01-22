﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ADOriginalOrder.ascx.cs"
    Inherits="WebApp.Subjects.UC.ADOriginalOrder" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<link href="../../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
<link href="../../bootstrap-3.3.2-dist/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
<script src="../../easyui1.4/jquery.min.js" type="text/javascript"></script>
<script src="../../bootstrap-3.3.2-dist/js/bootstrap.min.js" type="text/javascript"></script>
<style type="text/css">
    .table1
    {
        border-collapse: collapse;
        text-align: center;
        font-size: 12px;
    }
    .table1 tr
    {
        height: 34px;
    }
    .table1 td
    {
        border: #dce0e9 solid 1px;
    }
    .center1
    {
        text-align: center;
    }
</style>
<script type="text/javascript">
    var h = '<%=isShow %>';
    var subjectType = '<%=subjectType %>';
    $(function () {
        var parent = window.parent;

        if (typeof (parent.ShowIframe) != 'undefined') {
            if (h == 0)
                parent.ShowIframe(0, subjectType);
            else
                parent.ShowIframe(1, subjectType);
        }
    })
</script>
<asp:ScriptManager ID="ScriptManager1" runat="server">
</asp:ScriptManager>
<asp:Panel ID="Panel1" runat="server">
    <div>
        <div class="tab" style="margin-top: 10px;">
            原始订单信息
            <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
        </div>
        <ul id="myTab" class="nav nav-tabs" style="background-color: #dce0e9;">
            <li class="active"><a href="#list" data-toggle="tab">List订单</a></li>
            <li><a href="#pop" data-toggle="tab">POP订单</a></li>
            <li><a href="#buchong" data-toggle="tab">补充订单</a></li>
            <li><a href="#merge" data-toggle="tab">合并订单</a></li>
            <li><a href="#material" data-toggle="tab">物料信息</a></li>
            <li><a href="#hc" data-toggle="tab">HC订单</a></li>
        </ul>
        <div id="myTabContent" class="tab-content" style="overflow: auto;">
            <div class="tab-pane fade in active" id="list" style="padding: 5px;">
                <table>
                    <tr>
                        <td style="width: 60px; color: Blue;">
                            搜索：
                        </td>
                        <td style="text-align: left;">
                            店铺编号：<asp:TextBox ID="txtShopNo1" runat="server"></asp:TextBox>
                            &nbsp;
                            <asp:Button ID="btnSreach1" runat="server" Text="搜 索" OnClick="btnSreach1_Click"
                                class="easyui-linkbutton ExportMerge" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                            &nbsp;
                            <asp:Button ID="btnExport1" runat="server" Text="导 出" OnClick="btnExport1_Click"
                                class="easyui-linkbutton ExportMerge" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                        </td>
                    </tr>
                </table>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:Repeater ID="gvList" runat="server" OnItemDataBound="gvList_ItemDataBound">
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
                                        <%#(AspNetPager1.CurrentPageIndex-1)*AspNetPager1.PageSize+ Container.ItemIndex + 1%>
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
                                        <asp:Label ID="labLevel" runat="server" Text=""></asp:Label>
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
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                        <div style="text-align: center;">
                            <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="10" CssClass="paginator"
                                CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                                CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
                            </webdiyer:AspNetPager>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="AspNetPager1" EventName="pagechanged" />
                        <asp:AsyncPostBackTrigger ControlID="btnSreach1" EventName="click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
            <div class="tab-pane fade" id="pop" style="padding: 5px;">
                <table>
                    <tr>
                        <td style="width: 60px; color: Blue;">
                            搜索：
                        </td>
                        <td style="text-align: left;">
                            店铺编号：<asp:TextBox ID="txtShopNo2" runat="server"></asp:TextBox>
                            &nbsp;
                            <asp:Button ID="btnSreach2" runat="server" Text="搜 索" OnClick="btnSreach2_Click"
                                class="easyui-linkbutton ExportMerge" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                            &nbsp;
                            <asp:Button ID="btnExport2" runat="server" Text="导 出" OnClick="btnExport2_Click"
                                class="easyui-linkbutton ExportMerge" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                        </td>
                    </tr>
                </table>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <asp:Repeater ID="gvPOP" runat="server">
                            <HeaderTemplate>
                                <table class="table1" style="width: 3000px;">
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
                                        <td>
                                            POP名称
                                        </td>
                                        <td>
                                            POP类型
                                        </td>
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
                                        <td>
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
                                        <td>
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
                                <tr class="tr_bai">
                                    <td style="width: 40px;">
                                        <%#(AspNetPager2.CurrentPageIndex-1)*AspNetPager2.PageSize+ Container.ItemIndex + 1%>
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
                                        <%#Eval("pop.GraphicNo")%>
                                    </td>
                                    <td>
                                        <%#Eval("pop.POPName")%>
                                    </td>
                                    <td>
                                        <%#Eval("pop.POPType")%>
                                    </td>
                                    <td>
                                        <%--<%#Eval("order.Gender")%>--%>
                                        <%#Eval("order.OrderGender") != null ? Eval("order.OrderGender") : Eval("order.Gender")%>
                                    </td>
                                    <td>
                                        <%#Eval("order.Quantity")%>
                                    </td>
                                    <td>
                                        <%#Eval("order.Sheet")%>
                                    </td>
                                    <td>
                                        <%#Eval("pop.PositionDescription")%>
                                    </td>
                                    <td>
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
                                    </td>
                                    <td>
                                        <%#Eval("pop.GraphicWidth")%>
                                    </td>
                                    <td>
                                        <%#Eval("pop.GraphicLength")%>
                                    </td>
                                    <td>
                                        <%#Eval("pop.Area")%>
                                    </td>
                                    <td>
                                        <%#Eval("pop.GraphicMaterial")%>
                                    </td>
                                    <td>
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
                                    </td>
                                    <td>
                                        <%#Eval("order.ChooseImg")%>
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
                        <div style="text-align: center;">
                            <webdiyer:AspNetPager ID="AspNetPager2" runat="server" PageSize="10" CssClass="paginator"
                                CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                                CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager2_PageChanged">
                            </webdiyer:AspNetPager>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="AspNetPager2" EventName="pagechanged" />
                        <asp:AsyncPostBackTrigger ControlID="btnSreach2" EventName="click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
            <div class="tab-pane fade" id="buchong" style="padding: 5px; overflow: auto;">
                <table>
                    <tr>
                        <td style="width: 60px; color: Blue;">
                            搜索：
                        </td>
                        <td style="text-align: left;">
                            店铺编号：<asp:TextBox ID="txtShopNo3" runat="server"></asp:TextBox>
                            &nbsp;
                            <asp:Button ID="btnSreach3" runat="server" Text="搜 索" OnClick="btnSreach3_Click"
                                class="easyui-linkbutton ExportMerge" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                            &nbsp;
                            <asp:Button ID="btnExport3" runat="server" Text="导 出" OnClick="btnExport3_Click"
                                class="easyui-linkbutton ExportMerge" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                        </td>
                    </tr>
                </table>
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <asp:Repeater ID="gvSupplement" runat="server">
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
                                        <%#(AspNetPager3.CurrentPageIndex-1)*AspNetPager3.PageSize+ Container.ItemIndex + 1%>
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
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                        <div style="text-align: center;">
                            <webdiyer:AspNetPager ID="AspNetPager3" runat="server" PageSize="10" CssClass="paginator"
                                CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                                CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager3_PageChanged">
                            </webdiyer:AspNetPager>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="AspNetPager3" EventName="pagechanged" />
                        <asp:AsyncPostBackTrigger ControlID="btnSreach3" EventName="click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
            <div class="tab-pane fade" id="merge" style="padding: 5px;">
                <table>
                    <tr>
                        <td style="width: 60px; color: Blue;">
                            搜索：
                        </td>
                        <td style="text-align: left;">
                            店铺编号：<asp:TextBox ID="txtShopNo4" runat="server"></asp:TextBox>
                            &nbsp;
                            <asp:Button ID="btnSreach4" runat="server" Text="搜 索" OnClick="btnSreach4_Click"
                                class="easyui-linkbutton" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                            &nbsp;
                            <asp:Button ID="Button1" runat="server" Text="导 出" OnClick="Button1_Click" class="easyui-linkbutton"
                                Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                        </td>
                    </tr>
                </table>
                <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                    <ContentTemplate>
                        <asp:Repeater ID="gvMerge" runat="server" OnItemDataBound="gvMerge_ItemDataBound">
                            <HeaderTemplate>
                                <table class="table1" style="width: 3000px;">
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
                                        <td>
                                            POP名称
                                        </td>
                                        <td>
                                            POP类型
                                        </td>
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
                                        <td>
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
                                        <td>
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
                                <tr class="tr_bai">
                                    <td style="width: 40px;">
                                        <%#(AspNetPager4.CurrentPageIndex-1)*AspNetPager4.PageSize+ Container.ItemIndex + 1%>
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
                                        <%#Eval("pop.GraphicNo")%>
                                    </td>
                                    <td>
                                        <%#Eval("pop.POPName")%>
                                    </td>
                                    <td>
                                        <%#Eval("pop.POPType")%>
                                    </td>
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
                                        <asp:Label ID="labLevel" runat="server" Text=""></asp:Label>
                                    </td>
                                    <td>
                                        <%#Eval("pop.PositionDescription")%>
                                    </td>
                                    <td>
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
                                    </td>
                                    <td>
                                        <%#Eval("pop.GraphicWidth")%>
                                    </td>
                                    <td>
                                        <%#Eval("pop.GraphicLength")%>
                                    </td>
                                    <td>
                                        <%#Eval("pop.Area")%>
                                    </td>
                                    <td>
                                        <%#Eval("pop.GraphicMaterial")%>
                                    </td>
                                    <td>
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
                                    </td>
                                    <td>
                                        <%#Eval("merge.ChooseImg")%>
                                    </td>
                                    <td>
                                        <%#Eval("merge.Remark")%>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                        <div style="text-align: center;">
                            <webdiyer:AspNetPager ID="AspNetPager4" runat="server" PageSize="10" CssClass="paginator"
                                CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                                CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager4_PageChanged">
                            </webdiyer:AspNetPager>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnSreach4" EventName="click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
            <div class="tab-pane fade" id="material" style="padding: 5px;">
                <table>
                    <tr>
                        <td style="width: 60px; color: Blue;">
                            搜索：
                        </td>
                        <td style="text-align: left;">
                            店铺编号：<asp:TextBox ID="txtShopNo5" runat="server"></asp:TextBox>
                            &nbsp;
                            <asp:Button ID="btnSreach5" runat="server" Text="搜 索" OnClick="btnSreach5_Click"
                                class="easyui-linkbutton" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                            &nbsp;
                            <asp:Button ID="Button3" runat="server" Text="导 出" OnClick="Button3_Click" class="easyui-linkbutton"
                                Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                        </td>
                    </tr>
                </table>
                <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                    <ContentTemplate>
                        <asp:Repeater ID="gvMaterial" runat="server">
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
                                        <%#(AspNetPager5.CurrentPageIndex-1)*AspNetPager5.PageSize+ Container.ItemIndex + 1%>
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
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                        <div style="text-align: center;">
                            <webdiyer:AspNetPager ID="AspNetPager5" runat="server" PageSize="10" CssClass="paginator"
                                CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                                CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager5_PageChanged">
                            </webdiyer:AspNetPager>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="AspNetPager5" EventName="pagechanged" />
                        <asp:AsyncPostBackTrigger ControlID="btnSreach5" EventName="click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
             <div class="tab-pane fade" id="hc" style="padding: 5px;">
                <table>
                    <tr>
                        <td style="width: 60px; color: Blue;">
                            搜索：
                        </td>
                        <td style="text-align: left;">
                            店铺编号：<asp:TextBox ID="txtShopNo7" runat="server"></asp:TextBox>
                            &nbsp;
                            <asp:Button ID="btnSreach7" runat="server" Text="搜 索" OnClick="btnSreach7_Click"
                                class="easyui-linkbutton" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                            &nbsp;
                            <%--<asp:Button ID="btnExportHC" runat="server" Text="导 出"  class="easyui-linkbutton"
                                Style="width: 65px; height: 26px; margin-bottom: 5px;" />--%>
                        </td>
                    </tr>
                </table>
                <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                    <ContentTemplate>
                        <asp:Repeater ID="gvHC" runat="server">
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
                                            物料支持
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
                                            级别
                                        </td>
                                        <td>
                                            位置描述
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
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td style="width: 40px;">
                                        <%#(AspNetPager7.CurrentPageIndex-1)*AspNetPager7.PageSize+ Container.ItemIndex + 1%>
                                    </td>
                                    <td>
                                        <%#Eval("shop.ShopNo")%>
                                    </td>
                                    <td>
                                        <%#Eval("shop.ShopName")%>
                                    </td>
                                    <td>
                                           <%#Eval("order.MaterialSupport")%>
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
                                        <%#Eval("order.LevelNum")%>
                                    </td>
                                    <td>
                                        <%#Eval("order.PositionDescription")%>
                                    </td>
                                    <td>
                                        <%#Eval("order.Quantity")%>
                                    </td>
                                    <td>
                                        <%#Eval("pop.GraphicLength")%>
                                    </td>
                                    <td>
                                        <%#Eval("pop.GraphicWidth")%>
                                    </td>
                                    <td>
                                        <%#Eval("pop.GraphicMaterial")%>
                                    </td>
                                     <td>
                                        <%#Eval("order.ChooseImg")%>
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
                        <div style="text-align: center;">
                            <webdiyer:AspNetPager ID="AspNetPager7" runat="server" PageSize="10" CssClass="paginator"
                                CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                                CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager7_PageChanged">
                            </webdiyer:AspNetPager>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="AspNetPager7" EventName="pagechanged" />
                        <asp:AsyncPostBackTrigger ControlID="btnSreach7" EventName="click" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
</asp:Panel>
<asp:Panel ID="Panel2" runat="server">
    <div class="tab" style="margin-top: 10px;">
            订单信息<!--新开店装修费订单-->
            <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
        </div>
    <div>
        <asp:UpdatePanel ID="UpdatePanel6" runat="server">
            <ContentTemplate>
                <asp:Repeater ID="repeater_List" runat="server">
                    <HeaderTemplate>
                        <table class="table">
                            <tr class="tr_hui">
                                <td>
                                    序号
                                </td>
                                <%--<td>
                                    店铺编号
                                </td>--%>
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
                                    备注
                                </td>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="tr_bai">
                            <td>
                                <%# (AspNetPager6.CurrentPageIndex - 1) * AspNetPager6.PageSize + Container.ItemIndex + 1%>
                            </td>
                            <%--<td>
                                <%#Eval("shop.ShopNo") %>
                            </td>--%>
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
                                <%#Eval("order.Remark") %>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <div style="text-align: center; margin-top: 10px;">
                    <webdiyer:AspNetPager ID="AspNetPager6" runat="server" PageSize="10" CssClass="paginator"
                        CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                        NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                        CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager6_PageChanged">
                    </webdiyer:AspNetPager>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="AspNetPager6" EventName="pagechanged" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
</asp:Panel>
