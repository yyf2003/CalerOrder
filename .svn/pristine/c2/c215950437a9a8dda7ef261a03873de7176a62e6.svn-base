<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ERPHostManage.aspx.cs" Inherits="WebApp.Customer.ERPHostManage" %>

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
            ERP客户端管理
        </p>
    </div>
    <div class="tr">
        》客户端信息</div>
        <table id="tbList" style="width: 100%;">
    </table>
    <div id="toolbar" style="height: 25px; margin-top: 0px;">
        <a id="btnRefresh" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-reload">刷新</a>
        <div class='datagrid-btn-separator'>
        </div>
        <a id="btnAdd" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-add">添加</a>
        <a id="btnEdit" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-edit">编辑</a>
        <a id="btnDelete" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-remove">删除</a>
        <div id="separator1" style="display:none;" class='datagrid-btn-separator'>
        </div>
    </div>

    <div id="editDiv" title="编辑" style="display:none;">
        <table style="width: 600px; text-align: center;">
            <tr class="tr_bai">
                <td style="width: 120px;">
                   客户公司名称：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtClientName" maxlength="40" style="width: 200px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 120px;">
                   客户公司编号：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtClientNo" maxlength="40" style="width: 200px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    服务器域名：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtClientHost" maxlength="40" style="width: 200px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="userRegionTr">
                <td style=" vertical-align:top; padding-top:5px;">
                    负责省份：
                </td>
                <td style=" padding:5px;">
                    <div id="provinceContainer" style="margin-left:0px; padding-left:5px; text-align:left; line-height:15px;">
                    </div>
                </td>
            </tr>
            <tr class="userRegionTr">
                <td style=" vertical-align:top; padding-top:5px;">
                    负责城市：
                </td>
                <td>
                    <div id="cityContainer" style="margin-left:0px; padding-left:5px; text-align:left;">
                    </div>
                </td>
            </tr>
        </table>
    </div>

    </form>
</body>
</html>
<script src="../Scripts/common.js" type="text/javascript"></script>
<script src="js/ERPHostManage.js" type="text/javascript"></script>
