<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomerMaterialList1.aspx.cs"
    Inherits="WebApp.Materials.CustomerMaterialList1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="/easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="/easyui1.4/jquery.min.js"></script>
    <script src="/easyui1.4/jquery.easyui.min.js"></script>
    <script src="/easyui1.4/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var url = '<%=url %>';
        function FinishImport() {
            $("#hfIsFinishImport").val("1");
        }
    </script>
</head>
<body class="easyui-layout">
    <div data-options="region:'north',split:true," style="height: 44px; overflow: hidden;">
        <div class="nav_title">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
                class="nav_table_p">
                客户材质信息
            </p>
        </div>
    </div>
    <div data-options="region:'west',split:true,title:'客户信息'" style="width: 150px;">
        <table id="tbCustomer" style="width: 100%;">
        </table>
    </div>
    <div data-options="region:'center',title:'',height:'100%',iconCls:'icon-ok'" style="overflow: hidden;">
        <div class="easyui-layout" data-options="fit:true">
            <div data-options="region:'west',split:true,title:'价格类型'" style="width: 240px;">
                <table id="tbPriceType" style="width: 100%;">
                </table>
                <div id="toolbar1" style="height: 28px;">
                    <a id="btnRefreshType" style="float: left;" class="easyui-linkbutton" plain="true"
                        icon="icon-reload">刷新</a>
                    <div class='datagrid-btn-separator'>
                    </div>
                    <a id="btnAddType" style="float: left; display: none;" class="easyui-linkbutton"
                        plain="true" icon="icon-add">新增</a> <a id="btnEditType" style="float: left; display: none;"
                            class="easyui-linkbutton" plain="true" icon="icon-edit">编辑</a>
                         <a id="btnChangeTypeState" style="float: left; display:none;"
                            class="easyui-linkbutton" plain="true" icon="icon-reload">
                              <span id="spanChangeState">启用</span>
                            </a>  
                </div>
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
                        <a id="btnAdd" style="float: left; display: none;" class="easyui-linkbutton" plain="true"
                            icon="icon-add">新增</a> <a id="btnEdit" style="float: left; display: none;" class="easyui-linkbutton"
                                plain="true" icon="icon-edit">编辑</a> <a id="btnDelete" style="float: left; display: none;"
                                    class="easyui-linkbutton" plain="true" icon="icon-remove">删除</a>
                        <div id="separator1" style="display: none;" class='datagrid-btn-separator'>
                        </div>
                        <a id="btnImport" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-back">
                            导入</a> <a id="btnExport" style="float: left;" class="easyui-linkbutton" plain="true"
                                icon="icon-redo">导出</a>
                        <div class='datagrid-btn-separator'>
                        </div>
                        <input type="text" id="txtSearchMaterialName" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="editMaterialDiv" title="添加客户材质" style="display: none;">
        <table style="width: 500px; text-align: center; margin-top: 10px;">
            <tr class="tr_bai">
                <td style="width: 150px; height: 25px;">
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
                <td style=" height: 25px;">
                    基础材质：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <select id="selCategory">
                        <option value="0">类别</option>
                    </select>
                    <select id="selBasicMaterial">
                        <option value="0">材质名称</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
             <tr class="tr_bai">
                <td style=" height: 25px;">
                    单位：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selUnit">
                        <option value="0">--请选择--</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style=" height: 25px;">
                    应收单价：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtPrice" maxlength="20" style="width: 100px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 25px;">
                    应付单价（安装）：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtPayPriceInstall" maxlength="20" style="width: 100px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style=" height: 25px;">
                    应付单价（生产+安装）：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtPayPriceInstallAndProduct" maxlength="20" style="width: 100px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
             <tr class="tr_bai">
                <td style=" height: 25px;">
                    应付单价（发货）：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtPayPriceSend" maxlength="20" style="width: 100px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
        </table>
    </div>
    <div id="importMaterialDiv" title="导入客户材质信息" style="display: none;">
        <iframe src="" frameborder="0" scrolling="auto" name="iframe1" id="iframe1" height="300"
            width="100%"></iframe>
    </div>
    <div id="editPriceTypeDiv" title="添加材质价格类型" style="display: none;">
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 100px;">
                    类型名称：
                </td>
                <td style="text-align: left; padding-left: 5px; width: 200px;">
                   <input type="text" id="txtPriceItemName" maxlength="50"/>
                </td>
                <td style="width: 120px;">
                    开始时间：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   <input type="text" id="txtPriceItemBeginDate" onclick="WdatePicker()" class="Wdate" maxlength="20"/>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    使用其他报价
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <select id="selOtherCustomer">
                        <option value="0">--请选择客户--</option>
                    </select>
                    <select id="selOldPriceItem">
                        <option value="0">--请选择--</option>
                    </select>
                    
                </td>
            </tr>
        </table>
        <div id="itemContainer" style="height: 360px; overflow: auto;">
            <table class="table">
                <thead>
                    <tr class="tr_hui">
                        <td style="width: 40px;">
                           删除
                        </td>
                        <td style="width: 40px;">
                            序号
                        </td>
                        <td>
                            材质名称
                        </td>
                        <td style="width: 80px;">
                            单位
                        </td>
                        <td style="width: 120px;">
                            单价
                        </td>
                    </tr>
                </thead>
                <tbody id="tbMaterialDetail">
                </tbody>
            </table>
        </div>
    </div>
    <input type="hidden" id="hfIsFinishImport" value="0" />
</body>
</html>
<script src="../Scripts/common.js" type="text/javascript"></script>
<link href="../layer/skin/default/layer.css" rel="stylesheet" type="text/css" />
<script src="../layer/layer.js" type="text/javascript"></script>
<script src="js/CustomerMaterialList1.js" type="text/javascript"></script>
