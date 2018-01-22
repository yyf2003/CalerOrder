<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.MaterialSupportManager.List" %>

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
                AD订单物料支持类型管理
            </p>
        </div>
    </div>
    <div data-options="region:'west',split:true,title:'基础分类'" style="width: 200px;">
        <table id="tbBasicList" style="width: 100%;">
        </table>
    </div>
    <div data-options="region:'center',title:'',height:'100%',iconCls:'icon-ok'" style="overflow: hidden;">
        <div id="materialTitle" class="easyui-panel" title="基础分类名称：" data-options="height:'100%',overflow:'auto'">
            <table id="tbMaterialSupport" style="width: 100%;">
            </table>
            <div id="toolbar" style="height: 28px">
                <a id="btnRefresh" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-reload">刷新</a>
                <div class='datagrid-btn-separator'>
                </div>
                <a id="btnAdd" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-add">新增</a>
                <a id="btnEdit" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-edit">编辑</a>
                <a id="btnDelete" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-remove">删除</a>
                <div  id="separator1" style="display:none;" class='datagrid-btn-separator'>
                </div>
                
            </div>
        </div>
    </div>
    

     <div id="editDiv" title="添加" style="display: none;">
        <table style="width: 400px; text-align: center; margin-top: 10px;">
            <tr class="tr_bai">
                <td style="width: 120px; height: 25px;">
                    物料支持级别：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtName" maxlength="40" style="width: 150px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 100px; height: 25px;">
                    基础类型：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <select id="selBasic">
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
<script src="js/List.js" type="text/javascript"></script>
