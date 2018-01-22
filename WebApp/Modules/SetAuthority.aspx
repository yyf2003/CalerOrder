<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetAuthority.aspx.cs" Inherits="WebApp.Modules.SetAuthority" %>

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
    <link href="../zTree_v3/css/zTreeStyle/zTreeStyle.css" rel="stylesheet" />
    <script src="../zTree_v3/js/jquery.ztree.all-3.5.min.js"></script>
    <style type="text/css">
        .roleList
        {
            list-style-type: none;
        }
        
        .roleList li
        {
            margin-bottom: 5px;
        }
    </style>
</head>
<body class="easyui-layout">
    <div data-options="region:'north',split:true," style="height: 60px; overflow: hidden;">
        <div class="nav_title">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
                class="nav_table_p">
                权限设置
            </p>
        </div>
    </div>
    <div data-options="region:'west',split:true,title:'角色'" style="width: 200px;">
        <table id="tbRoles" style="width: 100%;">
        </table>
    </div>
    <div data-options="region:'center',title:'',iconCls:'icon-ok'" style="overflow: hidden;">
        <div id="moduleTitle" class="easyui-panel" title=">>角色：" data-options="height:'100%',overflow:'auto'">
            <div style="padding-left: 8px; height: 35px; background: #e5e2e2; line-height: 30px;">
                搜索模块：<input id="ModuleInput" style="width: 160px;" />&nbsp; <span id="showWait" style="display: none;">
                    <img src="../image/loadingA.gif" /></span> <a href="javascript:void(0)" id="btnSave"
                        class="easyui-linkbutton" data-options="iconCls:'icon-save'" style="width: 65px;">
                        提交</a>
            </div>
            <div>
                <ul id="tree1" class="ztree">
                </ul>
            </div>
        </div>
    </div>
</body>
</html>
<script src="js/SetAuthority.js" type="text/javascript"></script>
