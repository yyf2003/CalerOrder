<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.TableSize.List" %>

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
    <%--<form id="form1" runat="server">--%>
    <%--<div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" />
        <p class="nav_table_p">
            陈列桌包边尺寸
        </p>
    </div>--%>
    <div data-options="region:'north',split:true," style="height: 44px; overflow: hidden;">
        <div class="nav_title">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
                class="nav_table_p">
                陈列桌包边尺寸
            </p>
        </div>
    </div>
    <div data-options="region:'center',title:'',height:'100%',iconCls:'icon-ok'" style="overflow: hidden;">
        <div id="machineFrameTitle" class="easyui-panel" title=">>尺寸信息" data-options="height:'100%',overflow:'auto'">
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
            </div>
        </div>
    </div>
    <div id="editDiv" title="添加" style="display: none;">
        <table style="width: 500px; text-align: center;">
            <tr class="tr_bai">
                <td style="width: 150px;">
                    POP位置：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selSheet">
                        <option value="陈列桌">陈列桌</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    器架名称：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selFrameName">
                        <option value="0">--请选择--</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    位置说明：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <%--<asp:TextBox ID="txtRemark" runat="server" MaxLength="10"></asp:TextBox>--%>
                    <input type="text" id="txtRemark" maxlength="10" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    正常尺寸：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    宽：
                    <%--<asp:TextBox ID="txtNormalWidth" runat="server" MaxLength="10" style=" width:80px;"></asp:TextBox>--%>
                    <input type="text" id="txtNormalWidth" maxlength="10" style="width: 80px;" />
                    &nbsp;&nbsp; 高：
                    <%--<asp:TextBox ID="txtNormalLength" runat="server" MaxLength="10" style=" width:80px;"></asp:TextBox>--%>
                    <input type="text" id="txtNormalLength" maxlength="10" style="width: 80px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    反包尺寸：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    宽：
                    <%--<asp:TextBox ID="txtWithEdgeWidth" runat="server" MaxLength="10" style=" width:80px;"></asp:TextBox>--%>
                    <input type="text" id="txtWithEdgeWidth" maxlength="10" style="width: 80px;" />
                    &nbsp;&nbsp; 长：
                    <%--<asp:TextBox ID="txtWithEdgeLength" runat="server" MaxLength="10" style=" width:80px;"></asp:TextBox>--%>
                    <input type="text" id="txtWithEdgeLength" maxlength="10" style="width: 80px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    数量：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <%--<asp:TextBox ID="txtQuantity" runat="server" MaxLength="2" style=" width:50px;"></asp:TextBox>--%>
                    <input type="text" id="txtQuantity" maxlength="10" style="width: 80px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    材质：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selCategory">
                        <option value="0">类别</option>
                    </select>
                    <select id="selOrderMaterial">
                        <option value="0">材质名称</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
        </table>
    </div>
    <%--</form>--%>
</body>
</html>
<script src="../Scripts/common.js" type="text/javascript"></script>
<script src="js/list.js" type="text/javascript"></script>
