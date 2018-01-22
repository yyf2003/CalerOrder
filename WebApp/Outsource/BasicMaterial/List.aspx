<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Outsource.BasicMaterial.List" %>

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
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var url = '<%=url %>';
    </script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
     <div data-options="region:'north',split:true," style="height: 44px; overflow: hidden;">
        <div class="nav_title">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" />
             <p class="nav_table_p">
                基础物料信息
            </p>
        </div>
    </div>
    <div data-options="region:'center',title:'',height:'100%',iconCls:'icon-ok'" style="overflow: hidden;">
        <div id="materialTitle" class="easyui-panel" title=">>信息列表" data-options="height:'100%',overflow:'auto'">
            <table id="tbMaterial" style="width: 100%;">
            </table>
            <div id="toolbar" style="height: 28px">
                <a id="btnRefresh" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-reload">
                    刷新</a>
                <%--<a id="btnCheck" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-tip">
                    查看</a>--%>
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
                
            </div>
        </div>
    </div>

     <div id="editDiv" title="添加物料" style="display: none;">
        <table class="table" style="width: 500px; text-align: center; margin-top: 0px;">
            <tr class="tr_bai">
                <td style=" width: 100px;">
                    物料名称：
                </td>
                <td style="text-align: left;  padding-left: 5px;">
                    <asp:TextBox ID="txtMaterialName" runat="server" MaxLength="30" style=" width:200px;"></asp:TextBox>
                </td>
               
            </tr>
            <tr class="tr_bai">
                <td>
                    单位：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   <select id="seleUnit">
                     <option value="0">--请选择--</option>
                   </select>
                </td>
                
            </tr>
            <tr class="tr_bai">
                <td>
                    单价：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPrice" runat="server" MaxLength="10"></asp:TextBox>
                </td>
               
            </tr>
           
        </table>
        
    </div>
    
    </form>
</body>
</html>
<script src="/Scripts/common.js" type="text/javascript"></script>
<link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
<script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
<script src="list.js" type="text/javascript"></script>
