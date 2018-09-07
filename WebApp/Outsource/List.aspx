<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Outsource.List" %>

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
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
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
                外协信息
            </p>
        </div>
    </div>
    <div data-options="region:'center',title:'',height:'100%',iconCls:'icon-ok'" style="overflow: hidden;">
        <div id="materialTitle" class="easyui-panel" title=">>信息列表" data-options="height:'100%',overflow:'auto'">
            <table id="tbCompany" style="width: 100%;">
            </table>
            <div id="toolbar" style="height: 60px">
                <div style="padding-left: 5px; padding-top: 5px;">
                    区域：
                    <select id="seleSearchRegion">
                      <option value="0">全部</option>
                    </select>
                     &nbsp;&nbsp; 
                    <input class="easyui-textbox" id="txtSearchName" data-options="prompt:'按名称查询'" style="width: 200px;
                        height: 25px;">
                    &nbsp;&nbsp; 
                    <a href="#" id="btnSearchOutsource" class="easyui-linkbutton" data-options="iconCls:'icon-search'"
                        style="width: 80px">Search</a>
                </div>
                <div>
                    <a id="btnRefresh" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-reload">
                        刷新</a> <a id="btnCheck" style="float: left;" class="easyui-linkbutton" plain="true"
                            icon="icon-tip">查看</a>
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
    </div>
    <div id="editDiv" title="添加外协公司" style="display: none;">
        <table class="table" style="width: 700px; text-align: center; margin-top: 0px;">
            <tr class="tr_bai">
                <td style="height: 25px;">
                    公司类型：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    外协
                </td>
                <td style="height: 25px;">
                    公司编码：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtCompanyCode" readonly="readonly" style="width: 150px;" />
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 25px; width: 100px;">
                    公司名称：
                </td>
                <td style="text-align: left; padding-left: 5px; width: 200px;">
                    <input type="text" id="txtCompanyName" maxlength="40" style="width: 180px;" />
                    <span style="color: Red;">*</span>
                </td>
                <td style="height: 25px; width: 80px;">
                    简称：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtShortName" maxlength="20" style="width: 150px;" />
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 25px;">
                    区域：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selRegion">
                        <option value="0">--请选择--</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
                <td>
                    省/市：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selProvince">
                        <option value="0">--请选择--</option>
                    </select>
                    &nbsp;
                    <select id="selCity">
                        <option value="0">--请选择--</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 25px;">
                    联系人：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtContact" maxlength="40" style="width: 150px;" />
                </td>
                <td>
                    电话：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtTel" maxlength="40" style="width: 150px;" />
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 25px;">
                    加盟时间：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtJoinDate" onfocus="WdatePicker()" maxlength="20" />
                </td>
                <td>
                    负责客服：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="seleCustomerService">
                        <option value="0">--请选择--</option>
                    </select>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 25px;">
                    公司地址：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtAddress" maxlength="100" style="width: 250px;" />
                </td>
            </tr>
        </table>
        <div id="regionDiv">
            <table style="width: 100%; margin-top: 10px;">
                <tr>
                    <td>
                        负责区域：
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top; padding-top: 8px; width: 100px; text-align: center;">
                        省 份：
                    </td>
                    <td>
                        <div id="provinceContainer" style="margin-left: 0px; padding-left: 5px; text-align: left;">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top; padding-top: 8px; text-align: center;">
                        城 市：
                    </td>
                    <td>
                        <div id="cityContainer" style="margin-left: 0px; padding-left: 5px; text-align: left;">
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="checkDiv" title="查看公司信息" style="display: none;">
        <table class="table" style="width: 700px; text-align: center; margin-top: 0px;">
            <tr class="tr_bai">
                <td style="width: 100px; height: 25px;">
                    公司类型：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labTypeName" runat="server" Text="外协"></asp:Label>
                </td>
                <td style="height: 25px;">
                    公司编号：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labCompanyCode" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 25px; width: 100px;">
                    公司名称：
                </td>
                <td style="text-align: left; padding-left: 5px; width: 250px;">
                    <asp:Label ID="labCompanyName" runat="server" Text=""></asp:Label>
                </td>
                <td style="height: 25px; width: 80px;">
                    简称：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labShortName" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 25px;">
                    省份：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labProvinceName" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    城市：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labCityName" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 25px;">
                    联系人：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labContacts" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    电话：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labTel" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 25px;">
                    加盟时间：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labJoinDate" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    负责客服：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labSCUserName" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 25px;">
                    公司地址：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labAddress" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 25px;">
                    负责省份：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labInchargeProvince" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 25px;">
                    负责城市：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labInchargeCity" runat="server" Text=""></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
<script src="../Scripts/common.js" type="text/javascript"></script>
<script src="js/list.js" type="text/javascript"></script>
<script src="../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
<link href="../fancyBox/source/jquery.fancybox.css" rel="stylesheet" type="text/css" />
<script src="../fancyBox/source/jquery.fancybox.js" type="text/javascript"></script>
<script type="text/javascript">
    var $j = jQuery.noConflict();
    function CheckPrice(companyId) {
        var url = "PriceList.aspx?companyId=" + companyId;
        $j.fancybox.open({
            href: url,
            type: 'iframe',
            padding: 5,
            width: "95%"
        });
    }
</script>
