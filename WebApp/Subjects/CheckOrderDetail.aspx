<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckOrderDetail.aspx.cs"
    Inherits="WebApp.Subjects.CheckOrderDetail" %>

<%@ Register Src="UC/ShowOrderDetail.ascx" TagName="ShowOrderDetail" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="../easyui1.4/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../easyui1.4/datagrid-detailview.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            查看项目明细
        </p>
    </div>
    <div class="tab">
    <uc1:ShowOrderDetail ID="ShowOrderDetail1" runat="server" />
    <br />
    <div style="text-align: center;">
        <input type="button" value="返 回" onclick="javascript:window.history.go(-1)" class="easyui-linkbutton"
            style="width: 65px; height: 26px; font-size: 13px;" />
    </div>
    <br />
    <br /></div>
    </form>
</body>
</html>
