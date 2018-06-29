<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.InstallPriceManage.List" %>

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
    <script src="/easyui1.4/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <style type="text/css">
        #contentDiv1
        {
            overflow: auto;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="contentDiv1">
        <div class="nav_title">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
                class="nav_table_p">
                活动安装费/快递费查询
            </p>
        </div>
        <table id="conditionTB" class="table">
            <tr class="tr_bai">
                <td style="width: 100px;">
                    客户
                </td>
                <td style="width: 200px; text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlCustomer" runat="server" Style="height: 23px;">
                    </asp:DropDownList>
                </td>
                <td style="width: 100px;">
                    活动月份
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <%-- <input type="text" id="txtMonth" value="" class="Wdate"  />--%>
                    <asp:TextBox ID="txtMonth" runat="server" CssClass="Wdate" onfocus="WdatePicker({skin:'whyGreen',dateFmt:'yyyy-MM',isShowClear:false,readOnly:true,onpicked:getMonth})"
                        Style="width: 80px;"></asp:TextBox>
                    <span id="spanUp" style="margin-left: 10px; cursor: pointer; color: Blue;">上一个月</span>
                    <span id="spanDown" style="margin-left: 20px; cursor: pointer; color: Blue;">下一个月</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    活动名称
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <input type="checkbox" id="guidanceCBALL" style="color: Blue;" />全选
                    <div id="guidanceDiv" style="width: 90%;">
                    </div>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    区域
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <img id="ImgLoadRegion" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                    <div id="regionDiv" style="width: 90%;">
                    </div>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    省份
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <img id="ImgLoadProvince" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                    <div id="ProvinceDiv" style="width: 90%;">
                    </div>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    城市
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                   <img id="ImgLoadCity" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                    <div id="CityDiv" style="width: 90%;">
                    </div>
                </td>
            </tr>
            <%-- <tr class="tr_bai">
            <td>
                店铺编号
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                <asp:TextBox ID="txtShopNo" runat="server" Style="width: 300px;"></asp:TextBox>
                (可以填写多个，用逗号分隔)
            </td>
        </tr>--%>
            <tr class="tr_bai">
                <td>
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px; height: 35px;">
                    <input type="button" id="btnSearch" value="查 询" class="easyui-linkbutton" style="width: 65px;
                        height: 26px;" />
                    <img id="loadingImg" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                </td>
            </tr>
        </table>
    </div>
    <div>
        <div class="layui-tab layui-tab-card" lay-filter="order">
            <ul class="layui-tab-title">
                <li class="layui-this" lay-id="1">安装费</li>
                <li lay-id="2">快递费</li>
            </ul>
            <div class="layui-tab-content" style="padding: 0px;">
                <div class="layui-tab-item layui-show" style="height: 500px;">
                    <table id="tbInstallShopList">
                    </table>
                    <div id="iToolbar" style="height: 28px">
                        <a id="btnIRefresh" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-reload">
                            刷新</a>
                        <div class='datagrid-btn-separator'>
                        </div>
                        <a id="btnIAdd" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-add">
                            新增</a> <a id="btnIEdit" style="float: left;" class="easyui-linkbutton" plain="true"
                                icon="icon-edit">编辑</a> <a id="btnIDelete" style="float: left;" class="easyui-linkbutton"
                                    plain="true" icon="icon-remove">删除</a> <a id="btnIRecover" style="float: left;" class="easyui-linkbutton"
                                        plain="true" icon="icon-redo">恢复</a>
                        <div class='datagrid-btn-separator'>
                        </div>
                        <input type="text" id="txtISearchShopNo" />
                    </div>
                </div>
                <div class="layui-tab-item" style="height: 500px;">
                    <table id="tbExpressShopList">
                    </table>
                    <div id="eToolbar" style="height: 28px">
                        <a id="btnERefresh" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-reload">
                            刷新</a>
                        <div class='datagrid-btn-separator'>
                        </div>
                        <a id="btnEAdd" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-add">
                            新增</a> <a id="btnEEdit" style="float: left;" class="easyui-linkbutton" plain="true"
                                icon="icon-edit">编辑</a> <a id="btnEDelete" style="float: left;" class="easyui-linkbutton"
                                    plain="true" icon="icon-remove">删除</a> <a id="btnERecover" style="float: left;" class="easyui-linkbutton"
                                        plain="true" icon="icon-redo">恢复</a>
                        <div class='datagrid-btn-separator'>
                        </div>
                        <input type="text" id="txtESearchShopNo" />
                    </div>
                </div>
            </div>
        </div>
        <div id="editInstallDiv" title="编辑安装费" style="display: none; width: 700px;">
            <table class="table">
                <tr class="tr_bai">
                    <td style="width: 120px;">
                        店铺编号：
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <asp:Label ID="labShopNo" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td style="width: 120px;">
                        安装费项目：
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <asp:DropDownList ID="ddlSubjectList" runat="server">
                            <asp:ListItem Value="0">--请选择安装费项目--</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        店内安装费(应收)：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtBasicPrice" runat="server"></asp:TextBox>
                        <span style="color: Red;">*</span>
                    </td>
                    <td>
                        橱窗安装费(应收)：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtWindowPrice" runat="server"></asp:TextBox>
                        <span style="color: Red;">*</span>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        高空安装费(应收)：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtOOHPrice" runat="server"></asp:TextBox>
                        <span style="color: Red;">*</span>
                    </td>
                    <td>
                        应付外协(总数)：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtPayPrice" runat="server"></asp:TextBox>
                        <span style="color: Red;">*</span>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        物料级别：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:DropDownList ID="ddlMaterialSupport" runat="server">
                            <asp:ListItem Value="">--请选择物料级别--</asp:ListItem>
                            <asp:ListItem Value="basic">Basic</asp:ListItem>
                            <asp:ListItem Value="premium">Premium</asp:ListItem>
                            <asp:ListItem Value="vvip">VVIP</asp:ListItem>
                            <asp:ListItem Value="mcs">MCS</asp:ListItem>
                            <asp:ListItem Value="others">Others(童店)</asp:ListItem>
                            <asp:ListItem Value="generic">Generic(常规)</asp:ListItem>
                        </asp:DropDownList>
                        <span style="color: Red;">*</span>
                    </td>
                    <td>
                        备注：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtRemark" runat="server" Style="width: 200px;"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div id="editExpressDiv" title="编辑快递费" style="display: none; width: 700px;">
            <table class="table">
                <tr class="tr_bai">
                    <td style="width: 120px;">
                        店铺编号：
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <asp:Label ID="labShopNo1" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
               
                <tr class="tr_bai">
                    <td>
                        应收快递费：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtExpressPrice" runat="server"></asp:TextBox>
                        <span style="color: Red;">*</span>
                    </td>
                    <td>
                        应付快递费：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtPayExpressPrice" runat="server"></asp:TextBox>
                        <span style="color: Red;">*</span>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    </form>
</body>
</html>
<script src="../Scripts/common.js" type="text/javascript"></script>
<script src="js/list.js" type="text/javascript"></script>
