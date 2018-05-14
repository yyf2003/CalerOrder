<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Approve.aspx.cs" Async="true" Inherits="WebApp.Subjects.Approve" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<%@ Register src="UC/OrderDetailUC.ascx" tagname="OrderDetailUC" tagprefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
     <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <script type="text/javascript">
        function SubmitFail(msg) {
            alert("提交失败："+msg);
        }
        function ApproveStae(msg,url) {
            if (msg == "ok") {
                layer.msg("审批成功");
                window.location.href = url;
            }
            else {
                layer.confirm(msg, { title: "提交失败", btn: ['确定'] });
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
     <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            项目审批
        </p>
    </div>
     <uc1:OrderDetailUC ID="OrderDetailUC1" runat="server" />
    <br />
     <div class="tr">
        >>审批信息</div>
    <table class="table">
      <tr class="tr_bai">
        <td style="width:100px;">审批结果</td>
        <td style=" text-align:left; padding-left:5px;">
            <asp:RadioButtonList ID="rblApproveResult" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
              <asp:ListItem Value="1">通过 </asp:ListItem>
              <asp:ListItem Value="2">不通过 </asp:ListItem>
            </asp:RadioButtonList>
        </td>
      </tr>
      <tr class="tr_bai">
        <td>审批意见</td>
        <td style=" text-align:left; padding-left:5px; height:80px;">
            <asp:TextBox ID="txtRemark" runat="server" Columns="60" Rows="5" TextMode="MultiLine" MaxLength="100"></asp:TextBox>
            (100字以内)
        </td>
      </tr>
    </table>
    <br />
    <div id="btnDiv" style="text-align: center;">
        <asp:Button ID="btnSubmit" runat="server" Text="提 交" OnClientClick="return Check()" class="easyui-linkbutton" style="width: 65px;
            height: 26px; font-size:13px;" onclick="btnSubmit_Click"/>
            &nbsp;&nbsp;
            <input type="button" value="返 回" onclick="javascript:window.history.go(-1)" class="easyui-linkbutton" style="width: 65px;
            height: 26px;font-size:13px;"/>
    </div>
    <div id="loadingApprove" style="text-align: center; display:none;">
       
        <img src="../image/WaitImg/loading1.gif" />
    </div>
    <br />
    <br />
    <asp:HiddenField ID="hfCustomerId" runat="server" />
    <asp:HiddenField ID="hfSubjectId" runat="server" />
    <asp:HiddenField ID="hfPlanIds" runat="server" />
    <div id="approveLoading" style=" display:none;color:Red; font-size:18px; height:130px; width:420px; line-height:120px; text-align:center;">
       提示：正在审批，请稍等...
      
    </div>
    </form>
</body>
</html>
<script src="js/approve.js" type="text/javascript"></script>
