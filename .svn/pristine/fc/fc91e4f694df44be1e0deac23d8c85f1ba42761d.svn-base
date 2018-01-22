<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Invoice.aspx.cs" Inherits="WebApp.Quotation.Invoice" %>

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
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var subjectId = '<%=subjectId %>';
        function Finish() {
            alert("提交成功！");
            window.parent.FinishImport();
        }
        function Fail(msg) {
            alert("提交失败：" + msg);

        }
    </script>
</head>
<body style=" height:400px;">
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            项目开票
        </p>
    </div>
    <div>
    <div class="tr">
            >>开票信息
        </div>
        <table class="table" id="editTb">
            <tr class="tr_bai">
                <td style="width: 120px;">
                    开票金额
                </td>
                <td style="width: 300px; text-align: left; padding-left: 5px;">
                   
                        <asp:TextBox ID="txtInvoiceMoney" runat="server" MaxLength="50"></asp:TextBox>
                        
                        <span style="color: Red;">*</span>
                   
                </td>
                <td style="width: 120px;">
                    发票号码
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   
                        <asp:TextBox ID="txtInvoiceNumber" runat="server" MaxLength="50"></asp:TextBox>
                        <span style="color: Red;">*</span>
                    
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    开票时间
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    
                        <asp:TextBox ID="txtInvoiceDate" onfocus="WdatePicker()"  runat="server" MaxLength="50"></asp:TextBox>
                        <span style="color: Red;">*</span>
                   
                </td>
                <td>
                    预计到款时间
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtExpectReceiveDate" runat="server" onfocus="WdatePicker()" MaxLength="50"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
           
            <tr class="tr_bai">
                <td>
                    备注</td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtRemark" runat="server" MaxLength="50" Style="width: 300px;"></asp:TextBox>
                   
                </td>
               
            </tr>
        </table>
    </div>
    <div style="text-align: center; margin-bottom:30px; margin-top:20px;">
        <asp:Button ID="btnSubmit" runat="server" Text="提 交" OnClientClick="return CheckVal()"
            class="easyui-linkbutton" Style="width: 65px; height: 26px;" OnClick="btnSubmit_Click" />
    </div>
    <asp:HiddenField ID="hfExpectReceiveDate" runat="server" />
    </form>
</body>
</html>
<script type="text/javascript">
    $(function () {
        $("#txtInvoiceDate").on("blur", function () {
            var invoiceDate = $.trim($("#txtInvoiceDate").val());
            if (invoiceDate != "") {
                invoiceDate = invoiceDate.replace('-', '/');
                var date = new Date(invoiceDate);
                date.setDate(date.getDate() + 60);
                $("#txtExpectReceiveDate").val(date.getFullYear() +"-"+ (date.getMonth()+1) +"-"+ date.getDate());
            }
        })
    })

    function CheckVal() {
        var money = $.trim($("#txtInvoiceMoney").val());
        if (money == "") {
            alert("请填写开票金额");
            return false;
        }
        else if (isNaN(money)) {
            alert("开票金额必须为数字");
            return false;
        }
        var number = $.trim($("#txtInvoiceNumber").val());
        if (number == "") {
            alert("请填写发票号码");
            return false;
        }
        var invoiceDate = $.trim($("#txtInvoiceDate").val());
        if (invoiceDate == "") {
            alert("请填写开票时间");
            return false;
        }
        var expectReceiveDate = $.trim($("#txtExpectReceiveDate").val());
        if (expectReceiveDate == "") {
            alert("请填写预计到款时间");
            return false;
        }
        //$("#hfExpectReceiveDate").val(expectReceiveDate);
        return true;
    }
</script>