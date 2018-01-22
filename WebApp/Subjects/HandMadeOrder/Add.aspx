<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="WebApp.Subjects.HandMadeOrder.Add" %>

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
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            <asp:Label ID="labTitel" runat="server" Text="新增补单项目"></asp:Label>
        </p>
    </div>
   <div class="tr">
        >>项目信息</div>
    <table class="table">
    <tr class="tr_bai">
            <td>
                按时间查询活动：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:TextBox ID="txtBegin" runat="server" onfocus="WdatePicker()"></asp:TextBox>
                —
                <asp:TextBox ID="txtEnd" runat="server" onfocus="WdatePicker()"></asp:TextBox>
                &nbsp;
                <asp:Button ID="btnSearch" runat="server" Text="查询"  class="easyui-linkbutton" 
            style="width: 65px; height:26px;" onclick="btnSearch_Click"/>
            </td>
            
        </tr>
    <tr class="tr_bai">
            <td>
                活动名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:DropDownList ID="ddlGuidance" runat="server" AutoPostBack="true"
                    onselectedindexchanged="ddlGuidance_SelectedIndexChanged">
                   <asp:ListItem Value="0">请选择</asp:ListItem>
                </asp:DropDownList>
               <span style="color:Red;">*</span>
            </td>
            
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <%--<asp:TextBox ID="txtSubjectName" runat="server" MaxLength="50" style="width:200px;"></asp:TextBox>
                <span style="color:Red;">*</span>
                <asp:Label ID="labMsg" runat="server" Text="" style="color:Red;"></asp:Label>--%>
                <asp:DropDownList ID="ddlSubject" runat="server">
                   <asp:ListItem Value="0">请选择</asp:ListItem>
                </asp:DropDownList>
                <span style="color:Red;">*</span>
            </td>
            
        </tr>
      
        <tr class="tr_bai">
            <td>
                备注
            </td>
            <td  style="text-align: left; padding-left: 5px;">
                <asp:TextBox ID="txtRemark" runat="server" style=" width:280px;"></asp:TextBox>
            </td>
        </tr>
    </table>
    <br />
    <div style="text-align: center; height: 35px;">
        <asp:Button ID="btnNext" runat="server" Text="下一步" 
            OnClientClick="return CheckVal()" class="easyui-linkbutton" 
            style="width: 65px; height:26px;" onclick="btnNext_Click"/>
            <img id="loadingImg" src="/image/WaitImg/loadingA.gif" style=" display:none;"/>
            &nbsp;&nbsp;&nbsp;
            <%--<input type="button" value="返 回" onclick="javascript:window.history.go(-1)" class="easyui-linkbutton" 
            style="width: 65px; height:26px;"/>--%>
           <asp:Button ID="btnGoBack" runat="server" Text="返 回" class="easyui-linkbutton" 
            style="width: 65px; height:26px;" onclick="btnGoBack_Click"/>
    </div>
    </form>
</body>
</html>

<script type="text/javascript">
    function CheckVal() {
        if ($("#ddlSubject").val() == "0") {
            alert("请选择项目名称");
            return false;
        }
        $("#loadingImg").show();
        return true;
    }
</script>