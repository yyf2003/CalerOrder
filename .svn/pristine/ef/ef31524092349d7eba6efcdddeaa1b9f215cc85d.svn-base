<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Users.Edit" %>

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
    <script src="../easyui1.4/plugins/jquery.treegrid.js"></script>
</head>
<body>
    <form id="form1" runat="server">
     <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            用户信息—编辑
        </p>
    </div>
     <div class="tr">
        >>用户信息</div>
      <table class="table">
       <tr class="tr_bai">
          <td style=" width:120px;">用户姓名</td>
          <td style=" text-align:left; padding-left:5px;">
              <asp:TextBox ID="txtRealName" runat="server" MaxLength="20"></asp:TextBox>
              <span style=" color:Red;">*</span>
              <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="txtRealName" runat="server" ForeColor="Red" ErrorMessage="必填" ValidationGroup="1"></asp:RequiredFieldValidator>
          </td>
       </tr> 
       <tr class="tr_bai">
          <td>登陆账号</td>
          <td style=" text-align:left; padding-left:5px;">
          
              <asp:TextBox ID="txtUserName" runat="server" MaxLength="20"></asp:TextBox>
              <span style=" color:Red;">*</span>
              <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="txtUserName" runat="server" ForeColor="Red" ErrorMessage="必填" ValidationGroup="1"></asp:RequiredFieldValidator>
              </td>
       </tr> 
       
       <tr class="tr_bai">
          <td>所属公司</td>
          <td style=" text-align:left; padding-left:5px;">
              <asp:Label ID="labCompany" runat="server" Text=""></asp:Label>
          </td>
       </tr> 
       <tr class="tr_bai">
          <td>角色</td>
          <td style=" text-align:left; padding-left:5px;">
          <asp:Label ID="labRole" runat="server" Text=""></asp:Label>
          </td>
       </tr> 
       <tr class="tr_bai">
          <td>负责客户</td>
          <td style=" text-align:left; padding-left:5px;">
          <asp:Label ID="labCustomers" runat="server" Text=""></asp:Label>
          </td>
       </tr> 
       <tr class="tr_bai">
         <td></td>
         <td style=" text-align:left; padding-left:5px; height:30px;">  
           <asp:Button ID="btnSubmit1" ValidationGroup="1" runat="server" 
                 class="easyui-linkbutton" Text="提交修改" style="width: 65px; height: 26px;" 
                 onclick="btnSubmit1_Click"/>
         </td> 
       </tr>
      </table>
      <br />
      <div class="tr">
        >>修改密码</div>
      <table class="table">
       <tr class="tr_bai">
          <td style=" width:120px;">原密码</td>
          <td style=" text-align:left; padding-left:5px;">
              <asp:TextBox ID="txtOldPsw" runat="server" MaxLength="20" TextMode="Password"></asp:TextBox>
              <span style=" color:Red;">*</span>
              <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ControlToValidate="txtOldPsw" runat="server" ForeColor="Red" ErrorMessage="必填" ValidationGroup="2"></asp:RequiredFieldValidator>
          </td>
       </tr> 
       <tr class="tr_bai">
          <td>新密码</td>
          <td style=" text-align:left; padding-left:5px;">
          
              <asp:TextBox ID="txtNewPsw" runat="server" MaxLength="20" TextMode="Password"></asp:TextBox>
              <span style=" color:Red;">*</span>
              <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ControlToValidate="txtNewPsw" runat="server" ForeColor="Red" ErrorMessage="必填" ValidationGroup="2"></asp:RequiredFieldValidator>
              </td>
       </tr> 
       <tr class="tr_bai">
          <td>新密码确认</td>
          <td style=" text-align:left; padding-left:5px;">
          
              <asp:TextBox ID="txtPswConfirm" runat="server" MaxLength="20" TextMode="Password"></asp:TextBox>
              <span style=" color:Red;">*</span>
              <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ControlToValidate="txtPswConfirm" runat="server" ForeColor="Red" ErrorMessage="必填" Display="Dynamic" ValidationGroup="2"></asp:RequiredFieldValidator>
              <asp:CompareValidator ID="CompareValidator1" ControlToValidate="txtPswConfirm" ControlToCompare="txtNewPsw" runat="server" ForeColor="Red" ErrorMessage="两次密码输入不一致" Display="Dynamic" ValidationGroup="2"></asp:CompareValidator>
              </td>
       </tr> 
       <tr class="tr_bai">
         <td></td>
         <td style=" text-align:left; padding-left:5px; height:30px;">  
           <asp:Button ID="btnSubmit2" runat="server" ValidationGroup="2" 
                 class="easyui-linkbutton" Text="提交修改" style="width: 65px; height: 26px;" 
                 onclick="btnSubmit2_Click"/>
         </td> 
       </tr>
      </table>
    </form>
</body>
</html>
