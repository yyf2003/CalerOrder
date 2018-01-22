<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.BaseInfoManager.List" %>

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
            基础数据管理
        </p>
    </div>
    <table id="datagrid"></table>
    <div id="toolbar">
        <a id="btnRefresh" style="float:left;" class="easyui-linkbutton" plain="true" icon="icon-reload">刷新</a>
        <div class='datagrid-btn-separator'></div>
        <a id="btnAdd" style="float:left;" class="easyui-linkbutton" plain="true" icon="icon-add">新增</a>
        <a id="btnEdit" style="float:left;" class="easyui-linkbutton" plain="true" icon="icon-edit">编辑</a>
        <a id="btnDelete" style="float:left;" class="easyui-linkbutton" plain="true" icon="icon-remove">删除</a>
        <div class='datagrid-btn-separator'></div>
    </div>
    <div id="editDiv" title="添加数据" style="display:none;">
        <table style="width:350px;text-align:center;">
            <tr>
                <td style="width:120px;height:30px;">所属大类</td>
                <td style="text-align:left;padding-left:5px;">
                    <input id="bigCategoryInput"  style="width:160px;" />
                </td>
            </tr>
            <tr>
                <td>名称</td>
                <td style="text-align:left;padding-left:5px;">
                    <input type="text" id="txtName" maxlength="20"/>
                </td>
            </tr>
            <tr>
                <td>BaseCode</td>
                <td style="text-align:left;padding-left:5px;">
                    <input type="text" id="txtBaseCode" maxlength="20"/>
                </td>
            </tr>
            
        </table>
    </div>
    </form>
</body>
</html>

<script src="js/baseinfo.js" type="text/javascript"></script>
