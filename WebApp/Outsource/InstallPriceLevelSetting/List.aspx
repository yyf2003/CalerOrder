<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Outsource.InstallPriceLevelSetting.List" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="/easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="/easyui1.4/jquery.easyui.min.js"></script>
    <link href="/layui230/css/layui.css" rel="stylesheet" type="text/css" media="all" />
    <script src="/layui230/layui.all.js" type="text/javascript"></script>
    
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'north',split:true," style="height: 44px; overflow: hidden;">
        <div class="nav_title">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
                class="nav_table_p">
                外协安装费级别
            </p>
        </div>
    </div>
    <div data-options="region:'west',split:true,title:'外协区域'" style="width: 150px;">
        <table id="tbOutsourceRegion" style="width: 100%;">
        </table>
    </div>
    <div data-options="region:'center',title:'',height:'100%',iconCls:'icon-ok'" style="overflow: hidden;">
        <div id="orderTitle" class="easyui-panel" title=">>区域：" data-options="height:'100%',overflow:'auto'">
            
                <div id="operatorToolbar" runat="server" class="layui-btn-group" style=" margin-top:10px; margin-left:10px; display:none;">
                    <span id="btnAdd" class="layui-btn layui-btn-sm"><i class="layui-icon">&#xe654;</i>添加设置</span>
                    <span id="btnEdit" class="layui-btn layui-btn-sm" style="margin-left: 30px;"><i class="layui-icon">
                        &#xe642;</i><span id='editSpan'>编辑设置</span></span> <span id="btnDelete" class="layui-btn layui-btn-sm" style="margin-left: 30px;">
                            <i class="layui-icon">&#xe640;</i>删除设置</span>
                </div>
            
            <div style="clear: both; margin-top: 5px;">
                <table id="tbSetList" class="layui-hide" lay-filter="tbSetList">
                </table>
            </div>
        </div>
    </div>
    <div id="editDiv" title="添加设置" style="display: none;">
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 100px;">
                    客户：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <%--<select id="selCustomer" style=" height:23px;">
                   <option value="0">--请选择客户--</option>
                </select>--%>
                    <asp:DropDownList ID="ddlCustomer" runat="server">
                        <asp:ListItem>--请选择客户--</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 100px;">
                    区域：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selRegion" style="height: 23px;">
                        <option value="0">--请选择区域--</option>
                    </select>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    省份：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <div>
                        <input type="checkbox" id="cbAllProvince" /><span style="color: Blue;">全选</span>
                    </div>
                    <div id="provinceDiv">
                    </div>
                </td>
            </tr>
        </table>
        <div style="color: Blue; padding-left: 5px; text-align: left; margin-top: 20px;">
            <input type="radio" name="rdSettingType" value="1" checked="checked" />&nbsp; 按省份统一设置：
        </div>
        <table class="table">
            <thead>
                <tr class="tr_hui">
                    <td style="width: 120px;">
                        按省份
                    </td>
                    <td>
                        Basic
                    </td>
                    <td>
                        Premium
                    </td>
                    <td>
                        VVIP
                    </td>
                    <td>
                        MCS
                    </td>
                    <td>
                        Generic
                    </td>
                    <td>
                        Others
                    </td>
                </tr>
            </thead>
            <tbody id="provinceSettingData">
                <tr class="tr_bai">
                    <td>
                        统一价格
                    </td>
                    <td>
                        <asp:TextBox ID="txtPBasicPrice" runat="server" MaxLength="5" Style="width: 60px;
                            text-align: center;"></asp:TextBox>
                    </td>
                    <td>
                        <asp:TextBox ID="txtPPremiumPrice" runat="server" MaxLength="5" Style="width: 60px;
                            text-align: center;"></asp:TextBox>
                    </td>
                    <td>
                        <asp:TextBox ID="txtPVVIPPrice" runat="server" MaxLength="5" Style="width: 60px;
                            text-align: center;"></asp:TextBox>
                    </td>
                    <td>
                        <asp:TextBox ID="txtPMCSPrice" runat="server" MaxLength="5" Style="width: 60px; text-align: center;"></asp:TextBox>
                    </td>
                    <td>
                        <asp:TextBox ID="txtPGenericPrice" runat="server" MaxLength="5" Style="width: 60px;
                            text-align: center;"></asp:TextBox>
                    </td>
                    <td>
                        <asp:TextBox ID="txtPOthersPrice" runat="server" MaxLength="5" Style="width: 60px;
                            text-align: center;"></asp:TextBox>
                    </td>
                </tr>
            </tbody>
        </table>
        <div style="color: Blue; padding-left: 5px; text-align: left; margin-top: 20px;">
            <input type="radio" name="rdSettingType" value="2" />&nbsp; 按城市设置：
        </div>
        <table class="table">
            <thead>
                <tr class="tr_hui">
                    <td style="width: 120px;">
                        城市
                    </td>
                    <td>
                        Basic
                    </td>
                    <td>
                        Premium
                    </td>
                    <td>
                        VVIP
                    </td>
                    <td>
                        MCS
                    </td>
                    <td>
                        Generic
                    </td>
                    <td>
                        Others
                    </td>
                </tr>
            </thead>
            <tbody id="citySettingData">
            </tbody>
        </table>
    </div>  


    </form>
</body>
</html>
<script src="list.js" type="text/javascript"></script>
