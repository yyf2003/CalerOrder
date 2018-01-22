<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="loadUC.aspx.cs" Inherits="WebApp.Subjects.ADOrders.loadUC" %>

<%@ Register src="../UC/ADOriginalOrder.ascx" tagname="ADOriginalOrder" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../../easyui1.4/jquery.min.js"></script>
    <script src="../../easyui1.4/jquery.easyui.min.js"></script>
    <script src="../../easyui1.4/jquery.min.js"></script>
     
</head>
<body>
    <form id="form1" runat="server">
    <div>
        
        <uc1:ADOriginalOrder ID="ADOriginalOrder1" runat="server" />
        
    </div>
    </form>
</body>
</html>
