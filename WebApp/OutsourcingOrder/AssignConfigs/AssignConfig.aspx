<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssignConfig.aspx.cs" Inherits="WebApp.OutsourcingOrder.AssignConfigs.AssignConfig" %>

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
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            外协订单设置
        </p>
    </div>
    <div class="tr">
        》设置信息</div>
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
        <div id="separator1" style="display: none; display: none;" class='datagrid-btn-separator'>
        </div>
    </div>
    <div id="editDiv" title="新增设置" style="display: none;">
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 120px; height: 30px;">
                    客户
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <%-- <select id="seleCustomer" style=" height:23px;">
                    </select>--%>
                    <asp:DropDownList ID="seleCustomer" runat="server" Style="height: 23px;">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 30px;">
                    类型
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <%--<select id="seleConfigType" style=" height:23px;">
                        <option value="0">--请选择--</option>
                    </select>--%>
                    <asp:DropDownList ID="seleConfigType" runat="server" Style="height: 23px;">
                        <asp:ListItem Value="0">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 30px;">
                    材质名称
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <%--<asp:TextBox ID="txtMaterialName" runat="server" MaxLength="30"></asp:TextBox>
                    &nbsp;--%>
                    <div id="materialDiv" style="display: none;">
                        <div style="color: Blue; margin-left: 0px; padding-left: 5px; text-align: left; border-bottom: 1px solid Blue;">
                            <input type="checkbox" value="1" id="cbIsFullMatch" />是否完全匹配
                        </div>
                        <img id="loadMaterial" style="display: none;" src="../../image/WaitImg/loadingA.gif" />
                        <div id="materialContainer" style="margin-left: 0px; padding-left: 5px; text-align: left;">
                        </div>
                    </div>
                    <%--<asp:CheckBox ID="cbIsFullMatch" runat="server" />--%>
                    <%--（提示：选择完全匹配：网格布≠网格布双喷绘，不选择完全匹配：网格布=网格布双喷绘）--%>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 30px;">
                    生产外协
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="seleOutsource" runat="server" Style="height: 23px;">
                        <asp:ListItem Value="-1">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 30px;">
                    店铺Channel
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <%-- <asp:TextBox ID="txtChannel" runat="server" MaxLength="30" style="width:300px;"></asp:TextBox>--%>
                    <input type="checkbox" id="cbAllChannel" style="margin-left: 5px;" /><span style="color: Blue;">全选</span>
                    <div id="channelContainer" style="margin-left: 0px; padding-left: 5px; text-align: left;">
                    </div>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 30px;">
                    店铺类型
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <%--<asp:TextBox ID="txtFormat" runat="server" MaxLength="30" style="width:300px;"></asp:TextBox>--%>
                    <input type="checkbox" id="cbAllFormat" style="margin-left: 5px;" /><span style="color: Blue;">全选</span>
                    <div id="formatContainer" style="margin-left: 0px; padding-left: 5px; text-align: left;">
                    </div>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 30px;">
                    区 域
                </td>
                <td>
                    <div id="regionContainer" style="margin-left: 0px; padding-left: 5px; text-align: left;">
                    </div>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 30px;">
                    省 份
                </td>
                <td>
                    <div id="provinceContainer" style="margin-left: 0px; padding-left: 5px; text-align: left;">
                    </div>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="vertical-align: top; padding-top: 8px;">
                    城 市
                </td>
                <td style="text-align: left;">
                    <%-- <input type="checkbox" id="cbAllCity"/>全选--%>
                    <div id="cityContainer" style="margin-left: 0px;">
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
<link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
<script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
<script src="../../Scripts/common.js" type="text/javascript"></script>
<script src="assignConfig.js" type="text/javascript"></script>
