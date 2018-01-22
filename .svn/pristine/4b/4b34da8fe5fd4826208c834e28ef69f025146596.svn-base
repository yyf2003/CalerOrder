<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MachineFrameSmallMaterial.aspx.cs" Inherits="WebApp.Materials.MachineFrameSmallMaterial" %>

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
<body class="easyui-layout">
    <div data-options="region:'north',split:true," style="height: 44px; overflow: hidden;">
        <div class="nav_title">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
                class="nav_table_p">
                 器架辅料信息
            </p>
        </div>
    </div>
    <div data-options="region:'west',split:true,title:'客户信息'" style="width: 200px;">
        <table id="tbCustomer" style="width: 100%;">
        </table>
    </div>
    <div data-options="region:'center',title:'',height:'100%',iconCls:'icon-ok'" style="overflow: hidden;">
        <div id="materialTitle" class="easyui-panel" title=">>客户名称：" data-options="height:'100%',overflow:'auto'">
            <table id="tbMaterial" style="width: 100%;">
            </table>
            <div id="toolbar" style="height: 28px">
                <a id="btnRefresh" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-reload">
                    刷新</a>
                <div class='datagrid-btn-separator'>
                </div>
                <a id="btnAdd" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-add">新增</a>
                <a id="btnEdit" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-edit">编辑</a>
                <a id="btnDelete" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-remove">删除</a>
                <a id="btnRecover" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-redo">恢复</a>
                <div id="separator1" style="display:none;" class='datagrid-btn-separator'>
                </div>
                <a id="btnImport" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-back">
                    导入</a>
                <div class='datagrid-btn-separator'>
                </div>
            </div>
        </div>
    </div>
    <div id="editMaterialDiv" title="添加客户材质" style="display: none;">
        <table style="width: 450px; text-align: center; margin-top: 10px;">
            <tr class="tr_bai">
                <td style="width: 120px; height: 25px;">
                    客户名称：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selCustomer">
                        <option value="0">--请选择--</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
          
            <tr class="tr_bai">
                <td style="width: 100px; height: 25px;">
                    器架：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <select id="selSheet">
                        <option value="0">位置</option>
                        <option value="鞋墙">鞋墙</option>
                        <option value="服装墙">服装墙</option>
                        <option value="中岛">中岛</option>
                        <option value="陈列桌">陈列桌</option>
                        
                    </select>
                    <select id="selMachineFrame">
                        <option value="0">器架名称</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            
            <tr class="tr_bai">
                <td style="width: 100px; height: 25px;">
                    辅料：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selSmallMaterial">
                        <option value="0">--请选择--</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
        </table>
    </div>
</body>
</html>
<script src="../Scripts/common.js" type="text/javascript"></script>
<script src="js/MachineFrameSmallMaterial.js" type="text/javascript"></script>
