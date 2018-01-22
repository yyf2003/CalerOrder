<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Import.aspx.cs" Inherits="WebApp.Subjects.Material.Import" %>

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
    <script type="text/javascript">
        function Finish() {
            window.parent.FinishImport();
            
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <%--<div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            导入物料信息
        </p>
    </div>--%>
    <div class="tr">
        >>选择文件
    </div>
    <table class="table">
        
        <tr class="tr_bai">
            <td style="width: 120px;">
                模板下载
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:LinkButton ID="lbDownLoad" runat="server" onclick="lbDownLoad_Click" 
                    >下载模板</asp:LinkButton>
                
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                选择文件
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:FileUpload ID="FileUpload1" runat="server" />
                &nbsp;
                <asp:CheckBox ID="cbAdd" runat="server" />追加（保留原数据）
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
            </td>
            <td style="text-align: left; padding-left: 5px; height: 35px;">
                <div id="showButton">
                    <asp:Button ID="btnImport" runat="server" Text="导 入" class="easyui-linkbutton" OnClientClick="return checkFile()"
                        Style="width: 65px; height: 26px;" OnClick="btnImport_Click" />
                    
                   
                </div>
                <div id="showWaiting" style="color: Red; display: none;">
                    <img src='/Image/WaitImg/loadingA.gif' />正在导入，请稍等...
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
        return true;
    }
</script>