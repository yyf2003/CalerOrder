<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="importPlace.aspx.cs" Inherits="WebApp.importPlace" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="Scripts/jquery-1.7.2.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:FileUpload ID="FileUpload1" runat="server" />
        <asp:Button ID="Button1" runat="server" Text="导入" onclick="Button1_Click" />
        <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
    </div>
    <div style=" margin-top:20px;">
        <asp:Button ID="Button2" runat="server" Text="更新快递费" onclick="Button2_Click" OnClientClick="return load(this)"/>
        <img id="loading" src="image/WaitImg/loadingA.gif" style=" display:none;"/>
        <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
    </div>
    <div style=" margin-top:20px;">
        <asp:Button ID="Button3" runat="server" Text="合并物料表" onclick="Button3_Click" OnClientClick="return load(this)"/>
        <img id="Img1" src="image/WaitImg/loadingA.gif" style=" display:none;"/>
        <asp:Label ID="Label3" runat="server" Text=""></asp:Label>
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    function load(obj) {
        //$("#loading").show();
        $(obj).next("img").show();
        return true;
    }
</script>

