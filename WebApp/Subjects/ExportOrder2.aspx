<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExportOrder2.aspx.cs" Inherits="WebApp.Subjects.ExportOrder2" %>

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
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            订单导出
        </p>
    </div>
    <div>
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 120px;">
                    活动名称：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labGuidanceName" runat="server" Text=""></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    <div class="tr" style="margin-top: 20px;">
        >>项目信息
    </div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table class="table">
                <tr class="tr_bai">
                    <td style="width: 120px;">
                        项目类型：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:CheckBoxList ID="cblCategory" runat="server" RepeatDirection="Horizontal" 
                            RepeatLayout="Flow" AutoPostBack="true" onselectedindexchanged="cblCategory_SelectedIndexChanged">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        项目名称：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <div id="loadSubjectImg" style=" display:none;">
                           <img id="Img1"  src="../image/WaitImg/loadingA.gif" />
                        </div>
                        <asp:CheckBoxList ID="cblSubject" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div>
       <table class="table">
          <tr class="tr_bai">
                    <td style="width: 120px;">
                    </td>
                    <td style="text-align: left; padding-left: 5px; height:40px;">
                        <asp:Button ID="btnExport" runat="server" Text="导 出" class="layui-btn layui-btn-small" 
                            onclick="btnExport_Click" />
                            <img id="loadingImg" style=" display:none;" src="../image/WaitImg/loadingA.gif" />
                    </td>
                </tr>
       </table>
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function (sender, e) {
        var postBackId = e.get_postBackElement().id;
        if (postBackId.indexOf("cblCategory") != -1) {
            $("#loadSubjectImg").show();
        }
    })
</script>
