<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExportOrder.aspx.cs" Inherits="WebApp.Subjects.ExportOrder" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="../easyui1.4/jquery.easyui.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        var subjectId = '<%=subjectId %>';
        function NoInfo() {
            alert("没有数据可以导出");

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            订单导出
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
            <td style="width: 120px;">
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
                外部项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
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
                项目负责人
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
                <asp:Label ID="labContact" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
                联系电话
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labTel" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                所属客户
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labCustomerName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                新增方式
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labAddType" runat="server" Text=""></asp:Label>
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
        >>搜索</div>
    <div class="tab">
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 120px;">
                    区域
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <div id="RegionDiv">
                    </div>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 120px;">
                    省份
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <div id="ProvinceDiv" style="width: 90%;">
                    </div>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 120px;">
                    城市
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <div id="CityDiv" style="width: 90%;">
                    </div>
                </td>
            </tr>
            <tr class="tr_bai">
                <td colspan="4" style="padding-right: 20px; text-align: right; height: 30px;">
                    <asp:Button ID="btnSearch" runat="server" Text="查 询" class="easyui-linkbutton" Style="width: 65px;
                        height: 26px;margin-right:10px;" OnClick="btnSearch_Click" />
                    
                    <%--<asp:Button ID="btnExport" runat="server" Text="导出订单" class="easyui-linkbutton" Style="width: 65px;
                        height: 26px;" OnClick="btnExport_Click" />
                        &nbsp;&nbsp;
                    <asp:Button ID="btnExport350" runat="server" Text="导出350总表" 
                        class="easyui-linkbutton" Style="width: 85px;
                        height: 26px;" onclick="btnExport350_Click" />--%>
                    <input type="button" value="导出订单" id="btnExport" class="easyui-linkbutton" style="width: 65px;
                        height: 26px; margin-left: 10px;" />
                    <img id="downloading1" src="../image/WaitImg/loadingA.gif" style="display: none;" />
                    <input type="button" value="导出350总表" id="btnExport350" class="easyui-linkbutton"
                        style="width: 80px; height: 26px; margin-left: 10px;" />
                    <img id="downloading2" src="../image/WaitImg/loadingA.gif" style="display: none;" />
                </td>
            </tr>
        </table>
    </div>
    <br />
    <div class="tab" style="margin-top: 10px;">
        订单信息
        <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
    </div>
    <div class="tab" style="overflow: auto;">
        <asp:Repeater ID="gvPOP" runat="server" OnItemDataBound="gvPOP_ItemDataBound">
            <HeaderTemplate>
                <%if (gvPOP.Items.Count == 0)
                  { %>
                <table class="table">
                    <%}
                  else
                  {%>
                    <table class="table1" style="width: 3000px;">
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
                                店铺类型
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
                                位置描述
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
                        <%} %>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="tr_bai">
                    <td style="width: 40px;">
                        <%#(AspNetPager1.CurrentPageIndex-1)*AspNetPager1.PageSize + Container.ItemIndex + 1%>
                    </td>
                    <td style="width: 50px;">
                        <%#Eval("order.OrderType") != null ? Eval("order.OrderType").ToString() == "1" ? "pop" : "道具" : "POP"%>
                    </td>
                    <td>
                        <%--店铺编号--%>
                        <%#Eval("shop.ShopNo")%>
                    </td>
                    <td>
                        <%--店铺名称--%>
                        <%#Eval("shop.ShopName")%>
                    </td>
                    <td>
                        <%--区域--%>
                        <%#Eval("shop.RegionName")%>
                    </td>
                    <td>
                        <%--省份--%>
                        <%#Eval("shop.ProvinceName")%>
                    </td>
                    <td>
                        <%--城市--%>
                        <%#Eval("shop.CityName")%>
                    </td>
                    <td>
                        <%--城市级别--%>
                        <%#Eval("shop.CityTier")%>
                    </td>
                    <td>
                        <%--店铺类型--%>
                        <%#Eval("order.Format")%>
                    </td>
                    <td>
                        <%--物料支持--%>
                        <%#Eval("order.MaterialSupport")%>
                    </td>
                    <td>
                        <%--店铺规模大小--%>
                        <%#Eval("order.POSScale")%>
                    </td>
                    <td>
                        <%--POP编号--%>
                        <%#Eval("order.GraphicNo")%>
                    </td>
                    
                    <td>
                        <%--位置--%>
                        <%#Eval("order.Sheet")%>
                    </td>
                    <td>
                        <%--级别--%>
                        <asp:Label ID="labLevel" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <%--性别--%>
                        <%#Eval("order.Gender")%>
                    </td>
                    <td>
                        <%--数量--%>
                        <%#Eval("order.Quantity")%>
                    </td>
                    <td>
                        <%--位置描述--%>
                        <%#Eval("pop") != null ? Eval("pop.PositionDescription") : ""%>
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
                    <td>
                        <%--位置宽(mm)--%>
                        <%#Eval("pop") != null ? Eval("pop.WindowWide") : ""%>
                    </td>
                    <td>
                        <%--位置高(mm)--%>
                        <%#Eval("pop") != null ? Eval("pop.WindowHigh") : ""%>
                    </td>
                    <td>
                        <%--位置深(mm)--%>
                        <%#Eval("pop") != null ? Eval("pop.WindowDeep") : ""%>
                    </td>
                    <td>
                        <%--位置规模--%>
                        <%#Eval("pop") != null ? Eval("pop.WindowSize") : ""%>
                    </td>
                    <td>
                        <%--样式--%>
                        <%#Eval("pop") != null ? Eval("pop.Style") : ""%>
                    </td>
                    <td>
                        <%--角落类型--%>
                        <%#Eval("pop") != null ? Eval("pop.CornerType") : ""%>
                    </td>
                    <td>
                        <%--系列--%>
                        <%#Eval("pop") != null ? Eval("pop.Category") : ""%>
                    </td>
                    <td>
                        <%--是否标准规格--%>
                        <%#Eval("pop") != null ? Eval("pop.StandardDimension") : ""%>
                    </td>
                    <td>
                        <%--是否格栅--%>
                        <%#Eval("pop") != null ? Eval("pop.Modula") : ""%>
                    </td>
                    <td>
                        <%--是否框架--%>
                        <%#Eval("pop") != null ? Eval("pop.Frame") : ""%>
                    </td>
                    <td>
                        <%--单/双面--%>
                        <%#Eval("pop") != null ? Eval("pop.DoubleFace") : ""%>
                    </td>
                    <td>
                        <%--是否有玻璃--%>
                        <%#Eval("pop") != null ? Eval("pop.Glass") : ""%>
                    </td>
                    <td>
                        <%--背景--%>
                        <%#Eval("pop") != null ? Eval("pop.Backdrop") : ""%>
                    </td>
                    <td>
                        <%--格栅横向数量--%>
                        <%#Eval("pop") != null ? Eval("pop.ModulaQuantityWidth") : ""%>
                    </td>
                    <td>
                        <%--格栅纵向数量--%>
                        <%#Eval("pop") != null ? Eval("pop.ModulaQuantityHeight") : ""%>
                    </td>
                    <td>
                        <%--平台长(mm)--%>
                        <%#Eval("pop") != null ? Eval("pop.PlatformLength") : ""%>
                    </td>
                    <td>
                        <%--平台宽(mm)--%>
                        <%#Eval("pop") != null ? Eval("pop.PlatformWidth") : ""%>
                    </td>
                    <td>
                        <%--平台高(mm)--%>
                        <%#Eval("pop") != null ? Eval("pop.PlatformHeight") : ""%>
                    </td>
                    <td>
                        <%--设备类别--%>
                        <%#Eval("pop") != null ? Eval("pop.FixtureType") : ""%>
                    </td>
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
                <%if (gvPOP.Items.Count == 0)
                  { %>
                <tr>
                    <td>
                        --无数据--
                    </td>
                </tr>
                <%} %>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    <div class="tab" style="text-align: center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    <div style="text-align: center; margin-top: 20px; margin-bottom: 20px;">
        <input type="button" value="返 回" onclick="javascript:window.history.go(-1);" class="easyui-linkbutton"
            style="width: 65px; height: 26px;" />
    </div>
    <asp:HiddenField ID="hfRegion" runat="server" />
    <asp:HiddenField ID="hfProvince" runat="server" />
    <asp:HiddenField ID="hfCity" runat="server" />
    <div style="display: none;">
        <iframe id="exportFrame" name="exportFrame" src=""></iframe>
    </div>
    </form>
</body>
</html>
<script src="js/exportOrder.js" type="text/javascript"></script>
