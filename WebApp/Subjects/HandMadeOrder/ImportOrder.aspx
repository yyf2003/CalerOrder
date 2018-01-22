<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportOrder.aspx.cs" Inherits="WebApp.Subjects.HandMadeOrder.ImportOrder" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="../../easyui1.4/jquery.easyui.min.js"></script>
    <script src="../../easyui1.4/jquery.min.js" type="text/javascript"></script>
    <link href="../../bootstrap-3.3.2-dist/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <script src="../../bootstrap-3.3.2-dist/js/bootstrap.min.js" type="text/javascript"></script>
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
            补单导入
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
            <td>
                所属客户
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labCustomer" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                备注
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labRemark" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <div class="tr" style="margin-top: 20px;">
        >>导入订单</div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px; text-align: right;">
                导入订单：
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
                <td style="width: 120px; height: 100px;">
                </td>
                <td class="nav_table_tdleft" style="vertical-align: top;">
                    <asp:Label ID="labState" runat="server" Text="导入完成" Style="color: Red; font-weight: bold;
                        font-size: 16px;"></asp:Label>
                    <br />
                    <div id="ExportFailMsg" runat="server" style="display: none;">
                        <asp:LinkButton ID="lbExportError" runat="server" OnClick="lbExportError_Click">导出失败信息</asp:LinkButton>
                    </div>
                    <div id="ErrorMaterialSupportWarning" runat="server" style="display: none;">
                        <span style="color: red; font-size: 14px;">物料支持级别不统一警告
                        ：</span><asp:LinkButton ID="lbExportErrorMaterialSupport" runat="server" OnClick="lbExportErrorMaterialSupport_Click" Style="text-decoration: underline;">导出</asp:LinkButton>
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="Panel2" runat="server">
        <div class="tab" style="margin-top: 10px;">
            补单信息
            <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
        </div>
        <ul id="myTab" class="nav nav-tabs" style="background-color: #dce0e9;">
            <li class="active"><a href="#pop" data-toggle="tab">POP订单</a></li>
            <li><a href="#hc" data-toggle="tab">HC订单</a></li>
        </ul>
        <div id="myTabContent" class="tab-content" style="overflow: auto;">
            <div class="tab-pane fade in active" id="pop" style="padding: 5px;">
                <table>
                    <tr>
                        <td style="width: 60px; color: Blue;">
                            搜索：
                        </td>
                        <td style="text-align: left;">
                            店铺编号：<asp:TextBox ID="txtShopNo1" runat="server"></asp:TextBox>
                            &nbsp;
                            <asp:Button ID="btnSreach1" runat="server" Text="搜 索" OnClick="btnSreach1_Click"
                                class="easyui-linkbutton" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                        </td>
                    </tr>
                </table>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div class="containerDiv">
                        <asp:Repeater ID="popList" runat="server">
                            <HeaderTemplate>
                                <table class="table1" style="width: 1200px;">
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
                                 <%if (popList.Items.Count == 0)
                                  { %>
                                   <tr class="tr_bai">
                                     <td colspan="19" style=" text-align:center;">--暂无数据--</td>
                                   </tr>
                                <%} %>
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
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="tab-pane fade" id="hc" style="padding: 5px;">
                <table>
                    <tr>
                        <td style="width: 60px; color: Blue;">
                            搜索：
                        </td>
                        <td style="text-align: left;">
                            店铺编号：<asp:TextBox ID="txtShopNo2" runat="server"></asp:TextBox>
                            &nbsp;
                            <asp:Button ID="btnSreach2" runat="server" Text="搜 索" OnClick="btnSreach2_Click"
                                class="easyui-linkbutton" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                        </td>
                    </tr>
                </table>
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                       <div class="containerDiv">
                        <asp:Repeater ID="hcList" runat="server">
                            <HeaderTemplate>
                                <table class="table1" style="width: 1200px;">
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
                                            其他备注
                                        </td>
                                        <td>
                                            操作
                                        </td>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td style="width: 30px;">
                                        <%#(AspNetPager2.CurrentPageIndex-1)*AspNetPager2.PageSize+ Container.ItemIndex + 1%>
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
                                <%if (hcList.Items.Count == 0)
                                  { %>
                                   <tr class="tr_bai">
                                     <td colspan="19" style=" text-align:center;">--暂无数据--</td>
                                   </tr>
                                <%} %>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                        </div>
                        <div style="text-align: center; margin-top: 10px;">
                            <webdiyer:AspNetPager ID="AspNetPager2" runat="server" PageSize="20" CssClass="paginator"
                                CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                                CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager2_PageChanged">
                            </webdiyer:AspNetPager>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </asp:Panel>
    <div style="text-align: center; margin-top: 20px; margin-bottom: 30px;">
        <asp:Button ID="btnSubmit" runat="server" Text="确定提交" class="easyui-linkbutton" Style="width: 65px;
            height: 26px;" OnClick="btnSubmit_Click" />
        &nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnGoBack" runat="server" Text="上一步" class="easyui-linkbutton" Style="width: 65px;
            height: 26px;" OnClick="btnGoBack_Click" />
    </div>
    <asp:HiddenField ID="hfCustomerId" runat="server" />
    </form>
</body>
</html>
<script type="text/javascript">
    function checkFile() {
        var val = $("#FileUpload1").val();
        if (val != "") {
            var extent = val.substring(val.lastIndexOf('.') + 1);
            if (extent != "xls" && extent != "xlsx") {
                alert("只能上传Excel文件");
                return false;
            }
        }
        else {
            alert("请选择文件");
            return false;
        }
        $("#showButton").css({ display: "none" });
        $("#showWaiting").css({ display: "" });

    }

    $("#btnSubmit").click(function () {
//        var val = $("#hfHasErrorMaterialSupport").val();
//        if (val == 1) {
//            alert("警告：存在物料支持级别不统一的店铺，请更新订单后再进行下一步");
//            return false;
//        }
//        else
//            return true;
    })
</script>
