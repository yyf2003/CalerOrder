<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuoteMaterialList.aspx.cs" Inherits="WebApp.Materials.QuoteMaterialList" %>

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
    <form id="form1" runat="server">
     <div data-options="region:'north',split:true," style="height: 44px; overflow: hidden;">
        <div class="nav_title">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
                class="nav_table_p">
                订单报价材质
            </p>
        </div>
    </div>
    <div data-options="region:'west',split:true,title:'客户信息'" style="width: 200px;">
        <table id="tbCustomer" style="width: 100%;">
        </table>
    </div>
    <div data-options="region:'center',title:'',height:'100%',iconCls:'icon-ok'" style="overflow: hidden;">
        <div id="materialTitle" class="easyui-panel" title=">>客户名称：" data-options="height:'100%',overflow:'auto'">
            <table id="tbQuoteMaterial" style="width: 100%;">
            </table>
            <div id="toolbar" style=" height:28px">
                <a id="btnRefresh" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-reload">
                    刷新</a>
                <div class='datagrid-btn-separator'>
                </div>
                <a id="btnAdd" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-add">新增</a>
                <a id="btnEdit" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-edit">编辑</a>
                <a id="btnDelete" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-remove">删除</a> 
                <div id="separator1" style="display:none;"  class='datagrid-btn-separator'>
                </div>
                 <a id="btnExport" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-undo">
                 导出</a>
                <input type="text" id="txtSearchQuoteMaterialName" />
                
            </div>
        </div>
    </div>

    <div id="editMaterialDiv" title="添加报价材质" style="display: none;">
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
                    基础材质：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <select id="selCategory">
                        <option value="0">类别</option>
                    </select>
                    <select id="selCustomerMaterial">
                        <option value="0">材质名称</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 100px; height: 25px;">
                    报价材质名称：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtQuoteMaterialName" maxlength="40" style="width: 200px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
           <%-- <tr class="tr_bai">
                <td style="width: 100px; height: 25px;">
                    单价：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labPrice" runat="server" Text=""></asp:Label>
                </td>
            </tr>--%>
            <tr class="tr_bai">
                <td style="width: 100px; height: 25px;">
                    单位：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labUnit" runat="server" Text=""></asp:Label>
                </td>
            </tr>
        </table>
    </div>

    <div id="importMaterialDiv" title="导入基础材质信息" style="display: none;">
        <iframe src="" frameborder="0" scrolling="auto" name="iframe1" id="iframe1" height="300"
            width="100%"></iframe>
    </div>

    </form>
</body>
</html>
<script src="../Scripts/common.js" type="text/javascript"></script>
<script src="js/QuoteMaterialList.js" type="text/javascript"></script>
