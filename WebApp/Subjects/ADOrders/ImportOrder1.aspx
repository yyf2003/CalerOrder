<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportOrder1.aspx.cs" Inherits="WebApp.Subjects.ADOrders.ImportOrder1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <script src="../../easyui1.4/jquery.min.js" type="text/javascript"></script>
    <link href="../../bootstrap-3.3.2-dist/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <script src="../../bootstrap-3.3.2-dist/js/bootstrap.min.js" type="text/javascript"></script>
    <script src="../../layui/layui.js" type="text/javascript"></script>
    <link href="../../layui/css/layui.css" rel="stylesheet" type="text/css" />
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
    </form>
</body>
</html>
