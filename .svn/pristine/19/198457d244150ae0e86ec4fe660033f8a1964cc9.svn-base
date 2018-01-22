<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditSupplement.aspx.cs" Inherits="WebApp.Quotation.EditSupplement" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../easyui1.4/jquery.min.js"></script>
    <script src="../easyui1.4/jquery.easyui.min.js"></script>
    <script type="text/javascript">
        function Finish() {
            alert("提交成功！");
            window.parent.FinishImport();
        }
        function Fail(msg) {
            alert("提交失败：" + msg);

        }
    </script>
</head>
<body style=" height:350px;">
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            编辑对账补充信息
        </p>
    </div>
    <table class="table">
            <tr class="tr_bai">
                <td style="width: 120px;">
                    CR名称
                </td>
                <td style="width: 200px; text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtCRName" runat="server" MaxLength="50"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
                <td style="width: 120px;">
                    CR号
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtCRNumber" runat="server" MaxLength="50"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    PO号
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPONumber" runat="server" MaxLength="50"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
                <td>
                    KPI%
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtKPI" runat="server" MaxLength="10"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            
            <tr class="tr_bai">
                <td >
                    备注
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtRemark" runat="server" MaxLength="50" Style="width: 300px;"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <div style=" text-align:center;">
           <asp:Button ID="btnSubmit" runat="server" Text="提 交" OnClientClick="return CheckVal()"
            class="easyui-linkbutton" Style="width: 65px; height: 26px;" OnClick="btnSubmit_Click" />
        </div>
        <br />
        <br />
        <br />
    </form>
</body>
</html>
<script type="text/javascript">
    $(function () {

    })
    function CheckVal() {
        if ($.trim($("#txtCRName").val()) == "") {
            alert("请输入CR名称");
            return false;
        }
        if ($.trim($("#txtCRNumber").val()) == "") {
            alert("请输入CR号");
            return false;
        }
        if ($.trim($("#txtPONumber").val()) == "") {
            alert("请输入PO号");
            return false;
        }
        if ($.trim($("#txtKPI").val()) == "") {
            alert("请输入KPI");
            return false;
        }
        if (isNaN($("#txtKPI").val())) {
            alert("KPI必须是数字");
            return false;
        }
        return true;
    }
</script>