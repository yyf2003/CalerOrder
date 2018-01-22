<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="WebApp.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="renderer" content="webkit">
    <title>登陆</title>
    <link href="CSS/login.css" rel="stylesheet" type="text/css" />
   
  
    <script type="text/javascript">
        function LoginFail() {
            alert("登陆失败：账号或密码错误！");

        }

        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div class="main-login">
            <div class="login-logo">
                卡乐POP订单管理系统-ddddfffff
            </div>
            <div class="login-content">
                <div class="login-info">
                    <span class="user">&nbsp;</span>
                    <asp:TextBox ID="txtLoginName" runat="server" class="login-input"></asp:TextBox>
                </div>
                <div class="login-info">
                    <span class="pwd">&nbsp;</span>
                    <asp:TextBox ID="txtPassword" TextMode="Password" runat="server" class="login-input"></asp:TextBox>
                </div>
                <div class="login-oper">
                    <asp:Panel ID="Panel1" runat="server" Visible="false">
                        请选择角色：
                        <asp:DropDownList ID="ddlRoles" runat="server">
                            <asp:ListItem Value="0">--请选择角色--</asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button ID="btnLoginWithRole" runat="server" Text="登 录"  class="login-btn" 
                            style=" width:60px; margin-right:0px;" onclick="btnLoginWithRole_Click" OnClientClick="return CheckRole()"/>
                        <asp:Button ID="btnCancel" runat="server" Text="取 消" class="login-btn" 
                            style=" width:60px; margin-left:20px; margin-right:0px;" onclick="btnCancel_Click" />
                    </asp:Panel>
                </div>
                <div class="login-oper" style=" text-align:center;">
                    <asp:Panel ID="Panel2" runat="server">
                       <asp:Button ID="btnLogin" runat="server" Text="登 录" class="login-btn" OnClientClick="return CheckVal()" OnClick="btnLogin_Click" />
                      <input name="" type="reset" value="重 置" class="login-reset" />
                    </asp:Panel>
                    
                </div>
            </div>
            <div class="bottom">
            </div>
        </div>
    </div>
    </form>
</body>
</html>

<script type="text/javascript">
    function CheckVal() {
        var userName = document.getElementById("txtLoginName").value;
        if (userName == "") {
            alert("请输入用户名");
            return false;
        }
        var psw = document.getElementById("txtPassword").value;
        if (psw == "") {
            alert("请输入密码");
            return false;
        }
        return true;
    }

    function CheckRole() {
        var role = document.getElementById("ddlRoles").value;
        if (role == "0") {
            alert("请选择角色");
            return false;
        }
        return true;
    }
</script>