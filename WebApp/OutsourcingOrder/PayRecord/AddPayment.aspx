<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddPayment.aspx.cs" Inherits="WebApp.OutsourcingOrder.PayRecord.AddPayment" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="/layui230/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui230/layui.js" type="text/javascript"></script>
    <script type="text/javascript">
        function submitSuccess() {
            window.parent.submitSuccess();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 100px;">
                    外协名称：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labOutsourceName" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    活动名称：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labGuidanceName" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    项目类型：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:CheckBoxList ID="cblSubjectCategory" runat="server" RepeatDirection="Horizontal"
                        RepeatLayout="Flow">
                    </asp:CheckBoxList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    项目名称：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <div><span id="spanExpend" style=" color:Blue; text-decoration:underline; cursor:pointer;">展开</span></div>
                    <div id="subjectListDiv" style=" display:none;">
                      <asp:CheckBoxList ID="cblSubject" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                      </asp:CheckBoxList>
                    </div>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    应付金额：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labShouldPay" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    实付金额：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPay" runat="server" MaxLength="10" autocomplete="off"></asp:TextBox>
                    <span style=" color:Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    付款日期：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPayDate" runat="server" CssClass="Wdate" onclick="WdatePicker()"></asp:TextBox>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    备注：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtRemark" runat="server" autocomplete="off" Style="width: 350px;"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    <div style="text-align: center; margin-top: 20px;">
        <asp:Button ID="btnSubmit" runat="server" Text="提 交" class="layui-btn layui-btn-normal"
            OnClick="btnSubmit_Click" OnClientClick="return check();" />
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    function check() {
        var shouldPay = $("#labShouldPay").html() || 0;
        var pay = $.trim($("#txtPay").val());
        if (pay == "") {
            alert("请填写实付金额");
            return false;
        }
        else if (isNaN(pay)) {
            alert("实付金额填写不正确");
            return false;
        }
        else if (parseFloat(pay) <= 0) {
            alert("实付金额必须大于0");
            return false;
        }
        else if (parseFloat(pay) > parseFloat(shouldPay)) {
            alert("实付金额不能大于应付金额");
            return false;
        }
        var payDate = $.trim($("#txtPayDate").val());
        if (payDate == "") {
            alert("请填写付款日期");
            return false;
        }

        return true;
    }

    function checkVal() {
        //layer.msg("dddd");
        return false;
    }
</script>
