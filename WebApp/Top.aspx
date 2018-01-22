<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Top.aspx.cs" Inherits="WebApp.Top" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="refresh" content="300" />
    <title></title>
    <link href="css/Systen_style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div id="header">
        
        <div class="logo">
           
           <%--卡乐POP订单管理系统--%>
        </div>
       <div class="User">
       <ul class="User_ul">
            <li><img src="image/home.png" alt="" /></li>
            <li><a href="index.aspx" target="ifrContext" title="返回首页">首页</a></li>
            <li><a href="Users/Edit1.aspx" target="ifrContext" title="修改个人信息"><img src="image/User.png" alt="" /><asp:Label ID="labUserName" runat="server" Text=""></asp:Label></a></li>
           
           
            <li><img src="Image/Quit.png" alt="" /></li>
            <li><a href="logOut.aspx" target="_parent" title="退出">退出</a></li>       
            </ul>
        </div>
    </div>
    </form>
</body>
</html>