﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="POPBlackListManage.aspx.cs"
    Inherits="WebApp.Customer.POPBlackListManage" %>

<!DOCTYPE html>
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
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'north',split:true," style="height: 44px; overflow: hidden;">
        <div class="nav_title">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
                class="nav_table_p">
                店铺不包含POP信息管理
            </p>
        </div>
    </div>
    <div data-options="region:'center',title:'',height:'100%',iconCls:'icon-ok'" style="overflow: hidden;">
        <div id="machineFrameTitle" class="easyui-panel" title=">>店铺不包含POP信息" data-options="height:'100%',overflow:'auto'">
            <table id="tbList" style="width: 100%;">
            </table>
            <div id="toolbar" style="height: 25px; margin-top: 0px;">
                <a id="btnRefresh" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-reload">
                    刷新</a>
                <div class='datagrid-btn-separator'>
                </div>
                <a id="btnAdd" style="float: left; display: none;" class="easyui-linkbutton" plain="true"
                    icon="icon-add">添加</a> <a id="btnEdit" style="float: left; display: none;" class="easyui-linkbutton"
                        plain="true" icon="icon-edit">编辑</a> <a id="btnDelete" style="float: left; display: none;"
                            class="easyui-linkbutton" plain="true" icon="icon-remove">删除</a>
                <div id="separator1" style="display: none;" class='datagrid-btn-separator'>
                </div>
                <div id="tb" style="float: left; margin-top: 3px; margin-left: 8px;">
                    店铺编号：<input type="text" id="shopNoSearch" maxlength="10" />
                    &nbsp;&nbsp; 店铺名称：<input type="text" id="shopNameSearch" maxlength="30" />
                    &nbsp;&nbsp;
                    <input type="button" id="btnSearch" value="搜索" class="easyui-linkbutton" style="width: 65px;
                        height: 26px;" />
                </div>
            </div>
        </div>
    </div>
    <div id="editDiv" title="编辑" style="display: none;">
        <table style="width: 500px; text-align: center;">
            <tr class="tr_bai">
                <td style="width: 120px;">
                    店铺编号：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtShopNo" maxlength="20" style="width: 200px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 120px;">
                    POP位置：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="seleSheet">
                        <option value="">--请选择-</option>
                        <option value="鞋墙">鞋墙</option>
                        <option value="服装墙">服装墙</option>
                        <option value="陈列桌">陈列桌</option>
                        <option value="SMU">SMU</option>
                        <option value="中岛">中岛</option>
                        <option value="橱窗">橱窗</option>
                        <option value="OOH">OOH</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    POP编号：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtGraphicNo" maxlength="20" style="width: 200px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    (<span style='color: Blue;'>可以填写多个，用英文逗号(,)分隔</span>)
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
<script src="../Scripts/common.js" type="text/javascript"></script>
<script src="js/POPBlackListManage.js" type="text/javascript"></script>
