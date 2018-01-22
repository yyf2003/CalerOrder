<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckFiles.aspx.cs" Inherits="WebApp.Quotation.CheckFiles" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../easyui1.4/jquery.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            查看项目报价单
        </p>
    </div>
    
    <div class="tr">>>文件信息</div>
    <table style=" width:100%;">
       <tr>
          <td style=" height:200px; vertical-align:top;">
              <%=fileStr%>
          </td>
          <td style=" height:100px;">
          </td>
       </tr>
    </table>
    </form>
</body>
</html>
