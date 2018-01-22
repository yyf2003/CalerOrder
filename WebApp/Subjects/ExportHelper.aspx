<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExportHelper.aspx.cs" Inherits="WebApp.Subjects.ExportHelper" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            var city = window.parent.GetCityData();
            $("#hfCityData").val(city);
            $(".Button1").click();
        })
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:HiddenField ID="hfCityData" runat="server" />
    <div>
        <asp:Button ID="Button1" class="Button1" runat="server" Text="Button" onclick="Button1_Click" />
    </div>
    </form>
</body>
</html>
