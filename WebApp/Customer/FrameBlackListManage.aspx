﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FrameBlackListManage.aspx.cs"
    Inherits="WebApp.Customer.FrameBlackListManage" %>

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
    <asp:HiddenField ID="hfIsFinishImport" runat="server" />
    <%--<div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" />
        <p class="nav_table_p">
            不含器架的店铺信息管理
        </p>
    </div>--%>
    <div data-options="region:'north',split:true," style="height: 44px; overflow: hidden;">
        <div class="nav_title">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
                class="nav_table_p">
                不含器架的店铺信息管理
            </p>
        </div>
    </div>
    <div data-options="region:'center',title:'',height:'100%',iconCls:'icon-ok'" style="overflow: hidden;">
        <div id="machineFrameTitle" class="easyui-panel" title=">>不含器架的店铺信息" data-options="height:'100%',overflow:'auto'">
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
                            <a id="btnImport" style="float: left; display: none;"
                            class="easyui-linkbutton" plain="true" icon="icon-redo">导入</a>
                <div id="separator1" style="display: none;" class='datagrid-btn-separator'>
                </div>
                <div id="tb" style="float: left; margin-top: 3px; margin-left: 8px;">
                    <input id="sheetSearch" style="width: 100px;" />
                    &nbsp;&nbsp; 店铺编号：<input type="text" id="shopNoSearch" maxlength="10" style=" width:120px;"/>
                    &nbsp;&nbsp; 店铺名称：<input type="text" id="shopNameSearch" maxlength="30" />
                    &nbsp;&nbsp;
                    <input type="button" id="btnSearch" value="搜索" class="easyui-linkbutton" style="width: 65px;
                        height: 26px;" />
                </div>
            </div>
        </div>
    </div>
    <div id="editDiv" title="添加" style="display: none;">
        <table style="width: 500px; text-align: center;">
            <tr class="tr_bai">
                <td style="width: 120px;">
                    店铺编号：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtShopNo" style="width: 200px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="vertical-align: top; padding-top: 5px;">
                    无器架POP位置：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px; vertical-align: top;
                    padding-top: 5px;">
                    <%--<input type="checkbox" id="cbAll" />全选
                    <hr style="width: 50px; margin-top: -6px;" />
                    <asp:CheckBoxList ID="cblSheet" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="鞋墙">鞋墙 </asp:ListItem>
                        <asp:ListItem Value="橱窗">橱窗 </asp:ListItem>
                        <asp:ListItem Value="中岛">中岛 </asp:ListItem>
                        <asp:ListItem Value="SMU">SMU </asp:ListItem>
                        <asp:ListItem Value="服装墙">服装墙 </asp:ListItem>
                        <asp:ListItem Value="陈列桌">陈列桌</asp:ListItem>
                    </asp:CheckBoxList>--%>
                    <asp:DropDownList ID="ddlSheet" runat="server">
                        <asp:ListItem Value="">--请选择--</asp:ListItem>
                        <asp:ListItem Value="鞋墙">鞋墙</asp:ListItem>
                        <asp:ListItem Value="凹槽">凹槽</asp:ListItem>
                        <asp:ListItem Value="中岛">中岛</asp:ListItem>
                        <asp:ListItem Value="服装墙">服装墙</asp:ListItem>
                        <asp:ListItem Value="陈列桌">陈列桌</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="vertical-align: top; padding-top: 5px;">
                    性别：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px; vertical-align: top;
                    padding-top: 5px;">
                   
                    <asp:DropDownList ID="ddlGender" runat="server">
                        <asp:ListItem Value="男女不限">男女不限</asp:ListItem>
                        <asp:ListItem Value="男">男</asp:ListItem>
                        <asp:ListItem Value="女">女</asp:ListItem>
                        
                        
                    </asp:DropDownList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="vertical-align: top; padding-top: 5px;">
                    角落类型：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px; vertical-align: top;
                    padding-top: 5px;">
                   
                    <asp:DropDownList ID="ddlCornerType" runat="server">
                        <asp:ListItem Value="">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
<link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
<script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
<script src="../Scripts/common.js" type="text/javascript"></script>
<script src="js/FrameBlackListManage.js" type="text/javascript"></script>
