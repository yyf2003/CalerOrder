<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportOrder.aspx.cs" Inherits="WebApp.Subjects.SecondInstallFee.ImportOrder" %>

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
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            二次安装费明细导入
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
    </table>
    <div class="tr" style="margin-top: 20px;">
        >>导入明细</div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px; text-align: right;">
                选择文件：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:FileUpload ID="FileUpload1" runat="server" />
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px; text-align: right;">
                下载模板：
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
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="Panel2" runat="server">
        <div class="tab" style="margin-top: 10px;">
            安装明细信息
            <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
        </div>
        <div>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:Repeater ID="repeater_PriceList" runat="server">
                        <HeaderTemplate>
                            <table class="table">
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
                                        POP位置
                                    </td>
                                    <td>
                                        宽
                                    </td>
                                    <td>
                                        高
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
                            <tr class="tr_bai">
                                <td>
                                    <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.ItemIndex + 1%>
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
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>
    <div style=" text-align:center; margin-bottom:20px; margin-top:10px;">
        <asp:Button ID="btnBack" runat="server" Text="上一步" class="easyui-linkbutton" 
            style="width: 65px; height: 26px;" onclick="btnBack_Click"/>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnBack1" runat="server" Text="返回列表" class="easyui-linkbutton" 
            style="width: 65px; height: 26px;" onclick="btnGoBack_Click"/>
    </div>
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
</script>
