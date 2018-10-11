<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BasicMaterialList.aspx.cs"
    Inherits="WebApp.Materials.BasicMaterialList" %>

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
        function FinishImport() {
            $("#hfIsFinishImport").val("1");
        }
    </script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'north',split:true," style="height: 44px; overflow: hidden;">
        <div class="nav_title">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
                class="nav_table_p">
                基础材质信息
            </p>
        </div>
    </div>
    <div data-options="region:'west',split:true,title:'分类'" style="width: 200px;">
        <table id="tbMaterialType" style="width: 100%;">
        </table>
    </div>
    <div data-options="region:'center',title:'',height:'100%',iconCls:'icon-ok'" style="overflow: hidden;">
        <div id="materialTitle" class="easyui-panel" title=">>类别名称：" data-options="height:'100%',overflow:'auto'">
            <table id="tbMaterial" style="width: 100%;">
            </table>
            <div id="toolbar" style="height: 60px">
                <div>
                    <table class="table" style="width: 100%;">
                        <tr class="tr_hui">
                            <td style="width: 70px; border: 0px; font-weight: bold;">
                                客户名称：
                            </td>
                            <td style="text-align: left; padding-left: 5px; border: 0px;">
                                <asp:DropDownList ID="ddlCustmoer" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </div>
                <div>
                    <a id="btnRefresh" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-reload">
                        刷新</a>
                    <div class='datagrid-btn-separator'>
                    </div>
                    <a id="btnAdd" style="float: left; display: none;" class="easyui-linkbutton" plain="true"
                        icon="icon-add">新增</a> <a id="btnEdit" style="float: left; display: none;" class="easyui-linkbutton"
                            plain="true" icon="icon-edit">编辑</a> <a id="btnDelete" style="float: left; display: none;"
                                class="easyui-linkbutton" plain="true" icon="icon-remove">删除</a> <a id="btnRecover"
                                    style="float: left; display: none;" class="easyui-linkbutton" plain="true" icon="icon-redo">
                                    恢复</a>
                    <div id="separator1" style="display: none;" class='datagrid-btn-separator'>
                    </div>
                    <a id="btnImport" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-back">
                        导入</a> <a id="btnExport" style="float: left;" class="easyui-linkbutton" plain="true"
                            icon="icon-redo">导出</a>
                    <div class='datagrid-btn-separator'>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="editMaterialDiv" title="添加基础材质" style="display: none;">
        <table style="width: 400px; text-align: center; margin-top: 10px;">
            <tr class="tr_bai">
                <td style="width: 120px; height: 25px;">
                    类别名称：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selCategory">
                        <option value="0">--请选择--</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 100px; height: 25px;">
                    材质名称：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtMaterialName" maxlength="40" style="width: 200px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 100px; height: 25px;">
                    单位：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selUnit">
                        <option value="0">--请选择--</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
        </table>
    </div>
    <div id="importMaterialDiv" title="导入基础材质信息" style="display: none;">
        <iframe src="" frameborder="0" scrolling="auto" name="iframe1" id="iframe1" height="300"
            width="100%"></iframe>
    </div>
    <input type="hidden" id="hfIsFinishImport" value="0" />
    </form>
</body>
</html>
<script src="../Scripts/common.js" type="text/javascript"></script>
<script src="js/BasicMaterialList.js" type="text/javascript"></script>
