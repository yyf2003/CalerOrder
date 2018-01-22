<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InstallPriceList.aspx.cs"
    Inherits="WebApp.Customer.InstallPriceList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="/easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="/easyui1.4/jquery.min.js"></script>
    <script src="/easyui1.4/jquery.easyui.min.js"></script>
    <script src="/easyui1.4/plugins/jquery.treegrid.js"></script>
    <script src="/easyui1.4/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script type="text/javascript">
        var url = '<%=url %>';
       
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            安装费设置
        </p>
    </div>
    <div class="tr">
        》安装费</div>
    <table id="tbList" style="width: 100%;">
    </table>
    <div id="toolbar" style="height: 25px; margin-top: 0px;">
        <a id="btnRefresh" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-reload">刷新</a>
        <div class='datagrid-btn-separator'>
        </div>
        <a id="btnEdit" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-edit">编辑</a>
        <a id="btnDelete" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-remove">删除</a>
        <div id="separator1" style="display:none;display:none;" class='datagrid-btn-separator'>
        </div>
    </div>
    <div id="editDiv" title="编辑" style="width: 500px; display: none;">
        <table id="editTb" class="table">
            <thead>
                <tr class="tr_hui">
                    <td style="width: 120px;">
                        级别
                    </td>
                    <td>
                        基础安装费
                    </td>
                    <td>
                        橱窗安装费
                    </td>
                    <td>
                        外协基础安装费
                    </td>
                    <td style="width: 60px;">
                        操作
                    </td>
                </tr>
            </thead>
            <tbody id="editBody">
            </tbody>
        </table>
        <div style="margin-top: 8px; text-align: right; padding-right: 8px; margin-bottom: 15px;">
            <input type="button" id="btnAddNewRow" value="新 增" class="easyui-linkbutton" style="width: 60px;
                height: 26px;" />
        </div>
    </div>
    </form>
</body>
</html>
<script src="../Scripts/common.js" type="text/javascript"></script>
<script src="js/InstallPriceList.js" type="text/javascript"></script>
