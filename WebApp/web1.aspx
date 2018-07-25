<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="web1.aspx.cs" Inherits="WebApp.web1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="Button1" runat="server" Text="导出Terrex" onclick="Button1_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button2" runat="server" Text="导出大货" onclick="Button2_Click" />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button3" runat="server" Text="更新订单时间" onclick="Button3_Click" OnClientClick="return check()"/>
        <img id="imgLoad" style=" display:none;" src="image/WaitImg/loadingA.gif" />
    </div>
    </form>
</body>
</html>
<script src="Scripts/jquery-1.7.2.js" type="text/javascript"></script>
<script type="text/javascript">
    function check() {
        $("#imgLoad").show();
    }
</script>
