<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddOrder.aspx.cs" Inherits="WebApp.PropSubject.AddOrder" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="../easyui1.4/jquery.easyui.min.js"></script>
    <style type="text/css">
        .divi
        {
            float: left;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            添加道具订单
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
    </table>
    <div class="tr" style="margin-top: 30px;">
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
                    <img src='../image/WaitImg/loadingA.gif' />正在导入，请稍等...
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
                        <asp:LinkButton ID="lbExportError" runat="server" OnClick="lbExportError_Click" Style="text-decoration: underline;">导出失败信息</asp:LinkButton>
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="Panel2" runat="server">
        <div class="tr">
            》订单信息</div>
        <div style="width: 100%; overflow: auto;">
            <asp:Repeater ID="Repeater1" runat="server">
                <HeaderTemplate>
                    <table class="table" style="width: 2000px;">
                        <tr class="tr_hui" style="font-weight: bold;">
                            <td colspan="14">
                                应收
                            </td>
                            <td colspan="16" style="border-left-color: #000;">
                                应付
                            </td>
                        </tr>
                        <tr class="tr_hui">
                            <td>
                                序号
                            </td>
                            <td>
                                道具名称
                            </td>
                            <td>
                                应用位置
                            </td>
                            <td>
                                材料类型
                            </td>
                            <td>
                                尺寸规格
                            </td>
                            <td>
                                工艺/服务描述
                            </td>
                            <td>
                                包装方式
                            </td>
                            <td>
                                单位
                            </td>
                            <td>
                                数量
                            </td>
                            <td>
                                材料成本
                            </td>
                            <td>
                                加工成本
                            </td>
                            <td>
                                包装费
                            </td>
                            <td>
                                运输费
                            </td>
                            <td>
                                单价小计
                            </td>
                            <td>
                                外协名称
                            </td>
                            <td>
                                包装方式
                            </td>
                            <td>
                                单位
                            </td>
                            <td>
                                数量
                            </td>
                            <td>
                                单价小计
                            </td>
                            <td>
                                备注
                            </td>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="tr_bai">
                        <td>
                            <%--序号--%>
                            <%#Container.ItemIndex+1 %>
                        </td>
                        <td>
                            <%--道具名称--%>
                            <%#Eval("MaterialName")%>
                        </td>
                        <td>
                            <%--应用位置--%>
                            <%#Eval("Sheet")%>
                        </td>
                        <td>
                            <%--材料类型--%>
                            <%#Eval("MaterialType")%>
                        </td>
                        <td>
                            <%--尺寸规格--%>
                            <%#Eval("Dimension")%>
                        </td>
                        <td>
                            <%--工艺/服务描述--%>
                            <%#Eval("ServiceType")%>
                        </td>
                        <td>
                            <%--包装方式--%>
                            <%#Eval("Packaging")%>
                        </td>
                        <td>
                            <%--单位--%>
                            <%#Eval("UnitName")%>
                        </td>
                        <td>
                            <%--数量--%>
                            <%#Eval("Quantity")%>
                        </td>
                        <td>
                            <%--材料成本--%>
                            <%#Eval("BOMCost")%>
                        </td>
                        <td>
                            <%--加工成本--%>
                            <%#Eval("ProcessingCost")%>
                        </td>
                        <td>
                            <%--包装费--%>
                            <%#Eval("PackingCost")%>
                        </td>
                        <td>
                            <%--运输费--%>
                            <%#Eval("TransportationCost")%>
                        </td>
                        <td>
                            <%--单价小计--%>
                            <%#Eval("UnitPrice")%>
                        </td>
                        <td>
                            <%--外协名称--%>
                            <%#Eval("OutsourceName")%>
                        </td>
                        <td>
                            <%--包装方式--%>
                            <%#Eval("PayPackaging")%>
                        </td>
                        <td>
                            <%--单位--%>
                            <%#Eval("PayUnitName")%>
                        </td>
                        <td>
                            <%--数量--%>
                            <%#Eval("PayQuantity")%>
                        </td>
                        <td>
                            <%--单价小计--%>
                            <%#Eval("PayUnitPrice")%>
                        </td>
                        <td>
                            <%--备注--%>
                            <%#Eval("PayRemark")%>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%if (Repeater1.Items.Count == 0)
                      {%>
                    <tr class="tr_bai">
                        <td colspan="20">
                            --暂无数据--
                        </td>
                    </tr>
                    <%} %>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </asp:Panel>
    <div style="margin-top: 30px; margin-bottom: 20px; text-align: center;">
        <asp:Panel ID="PanelSubmit" runat="server">
            <asp:Button ID="btnSubmit" runat="server" Text="Button" OnClick="btnSubmit_Click"
                Style="display: none;" />
            <input type="button" id="btnSubmit1" value="提 交" class="easyui-linkbutton" style="width: 65px;
                height: 26px;" />
            <img id="imgSubmit" src='../image/WaitImg/loadingA.gif' style="display: none;" />
        </asp:Panel>
        <asp:Panel ID="PanelBack" runat="server">
            <asp:Button ID="btnBack" runat="server" Text="返 回" class="easyui-linkbutton" Style="width: 65px;
                height: 26px;" OnClick="btnBack_Click" />
        </asp:Panel>
    </div>
    <asp:HiddenField ID="hfGuidanceId" runat="server" />
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
        //return true;
    }


    $(function () {
        $("#btnSubmit1").click(function () {

            $(this).attr("disabled", true);
            $("#imgSubmit").show();
            $("#btnSubmit").click();
        })
    })
</script>
