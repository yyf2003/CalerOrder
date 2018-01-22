<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportOrderDetail.aspx.cs" Inherits="WebApp.Subjects.SecondInstallFee.ImportOrderDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <script type="text/javascript">
        function Change() {
            window.parent.MakeChange();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <br />
    <blockquote class="layui-elem-quote">
        <table style="width: 100%;">
            <tr>
                <td style="width: 90px;">
                    选择文件：
                </td>
                <td style="text-align: left; padding-left: 5px; width: 200px;">
                    <asp:FileUpload ID="FileUpload1" runat="server" />
                </td>
                <td style="text-align: left; padding-left: 5px; width: 300px;">
                    <div id="showBtn">
                    <span id="btnImport" class="layui-btn layui-btn-small layui-btn-normal">
                        <i class="layui-icon">&#xe62f;</i>导 入
                    </span>
                    <asp:CheckBox ID="cbDeleteOld" runat="server"  style=" margin-left:30px;"/>删除旧数据
                    </div>
                    <div id="showWaiting" style="color: Red; display: none;">
                        <img src='../../Image/WaitImg/loadingA.gif' />正在导入，请稍等...
                    </div>
                    
                </td>
                <td>
                    <asp:LinkButton ID="lbDownLoadTemplate" runat="server" OnClick="lbDownLoadTemplate_Click"
                        Style="color: Blue; text-decoration: underline;">下载模板</asp:LinkButton>
                </td>
            </tr>
        </table>
    </blockquote>
    <div>
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
    </div>
    <asp:Button ID="btnImport1" runat="server" Text="导入" OnClick="btnImport_Click" OnClientClick="return checkFile();" Style="display: none;" />
    <asp:HiddenField ID="hfCustomerId" runat="server" />
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    $(function () {
        $("#btnImport").click(function () {
            $("#btnImport1").click();
        })
    })

    function checkFile() {
        var val = $("#FileUpload1").val();
        if (val != "") {
            var extent = val.substring(val.lastIndexOf('.') + 1);
            if (extent != "xls" && extent != "xlsx") {

                layer.msg('只能上传Excel文件');
                return false;
            }
        }
        else {

            layer.msg('请选择文件');
            return false;
        }
        $("#showBtn").hide();
        $("#showWaiting").show();
        return true;
    }
</script>