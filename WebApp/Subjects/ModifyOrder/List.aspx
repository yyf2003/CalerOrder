<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Subjects.ModifyOrder.List" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <style type="text/css">
        .inputClass
        {
            width: 180px;
        }
        #editDiv li
        {
            margin-bottom: 2px;
            height: 20px;
            font-size: 14px;
            cursor: pointer;
            padding-left: 5px;
            color: Blue;
        }
        #editDiv li:hover
        {
            background-color: #f0f1f2;
            text-decoration: underline;
        }
    </style>
    <script type="text/javascript">
        var subjectId = '<%=subjectId %>';
    </script>
</head>
<body style="margin-bottom: 20px;">
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            修改订单
        </p>
    </div>
    <blockquote class="layui-elem-quote" style="font-weight: bold; height: 15px; padding-top: 7px;">
        项目信息
    </blockquote>
    <table class="layui-table" style="margin-top: -10px;">
        <tr class="tr_bai">
            <td style="width: 100px;">
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px; width: 350px;">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 100px;">
                项目编号
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                开始时间
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labBeginDate" runat="server" Text=""></asp:Label>
            </td>
            <td>
                结束时间
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labEndDate" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目创建人
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labAddUserName" runat="server" Text=""></asp:Label>
            </td>
            <td>
                所属客户
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labCustomerName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                订单类型
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectType" runat="server" Text=""></asp:Label>
            </td>
            <td>
                备注
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labRemark" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <blockquote class="layui-elem-quote" style="font-weight: bold; height: 15px; padding-top: 7px;">
        订单信息<span style="font-size: 12px; font-weight: normal;">—完成拆单后的订单</span>
    </blockquote>
    <div>
        <div class="layui-inline">
            <span id="btnAdd" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe61f;</i>添加</span>
            <span id="btnEdit" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe642;</i>修改POP</span>
            <span id="btnDelete" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe640;</i>删除</span>
            <span id="btnRecover" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe652;</i>恢复</span>
            <span id="btnEditShop" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe642;</i>修改店铺信息</span>
        </div>
        <div class="layui-inline" style=" padding-top:6px;">
            <input type="text" id="txtSearchShopNo" placeholder="请输入店铺编号" maxlength="20" style="height: 25px;
                border-radius: 3px; border: 1px solid #888;">
            <span>订单类型：
                <select id="seleOrderType" style="height: 25px;">
                    <option value="0">--请选择--</option>
                </select>
                &nbsp;&nbsp; POP位置：
                <select id="seleSheet" style="height: 25px;">
                    <option value="">--请选择--</option>
                </select>
                &nbsp;&nbsp; 性别：
                <select id="seleGender" style="height: 25px;">
                    <option value="">--请选择--</option>
                </select>
            </span>&nbsp;&nbsp; <span id="btnSearch" class="layui-btn layui-btn-small">
                <i class="layui-icon">&#xe615;</i>搜索</span>
        </div>
    </div>
    <div class="containerDiv" class="layui-form">
        <table class="layui-table" style="width: 2500px;">
            <colgroup>
                <col width="50">
                <col>
            </colgroup>
            <thead style="font-size: 12px;">
                <tr>
                    <th>
                        <input type="checkbox" id="cbAllPOP" />
                    </th>
                    <th>
                        序号
                    </th>
                    <th>
                        所属项目
                    </th>
                    <th>
                        类型
                    </th>
                    <th>
                        店铺编号
                    </th>
                    <th>
                        店铺名称
                    </th>
                    <th>
                        区域
                    </th>
                    <th>
                        省份
                    </th>
                    <th>
                        城市
                    </th>
                    <th>
                        Format
                    </th>
                    <th>
                        物料支持级别
                    </th>
                    <th>
                        店铺规模大小
                    </th>
                    <th>
                        城市级别
                    </th>
                    <th>
                        是否安装
                    </th>
                    <th>
                        Sheet
                    </th>
                    <th>
                        器架类型
                    </th>
                    <th>
                        男/女
                    </th>
                    <th>
                        数量
                    </th>
                    <th>
                        应收金额
                    </th>
                    <th>
                        应付金额
                    </th>
                    <th>
                        POP宽
                    </th>
                    <th>
                        POP高
                    </th>
                    <th>
                        材质
                    </th>
                    <th>
                        位置描述
                    </th>
                    <th>
                        选图
                    </th>
                    <th>
                        备注
                    </th>
                    <th>
                        订单状态
                    </th>
                    <th>
                        店铺状态
                    </th>
                </tr>
            </thead>
            <tbody id="tbodyOrderEmpty">
                <tr>
                    <td colspan="28" style="text-align: center;">
                        --暂无数据--
                    </td>
                </tr>
            </tbody>
            <tbody id="tbodyOrderData" style="display: none;">
            </tbody>
        </table>
    </div>
    <div id="page1" style="text-align: center; margin-top: 10px;">
    </div>
    <div style="height: 50px; text-align: center;">
        <span id="Span2" onclick="javascript:window.history.go(-1)" class="layui-btn layui-btn-normal">
            返 回</span>
    </div>
    <div id="editDiv" title="编辑POP信息" style="display: none;">
        <table id="POPtable" class="table" style="width: 750px; text-align: center; margin-bottom: 50px;">
            <tr>
                <td style="height: 30px; width: 100px;">
                    类型
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:RadioButtonList ID="rblOrderType" runat="server" RepeatDirection="Horizontal"
                        RepeatLayout="Flow">
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr class="showSubjectList" style="display: none;">
                <td style="width: 100px; height: 30px;">
                    所属项目
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlSubjectList" runat="server">
                        <asp:ListItem Value="0">--请选择项目--</asp:ListItem>
                    </asp:DropDownList>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr>
                <td style="width: 100px; height: 30px;">
                    店铺编号
                </td>
                <td style="text-align: left; width: 250px; padding-left: 5px;">
                    <asp:TextBox ID="txtShopNo" runat="server" MaxLength="20" CssClass="inputClass"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
                <td style="width: 100px; height: 30px;">
                    POP位置
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <div style="position: relative;">
                        <input type="text" id="txtSheet" class="inputClass" />
                        <div id="divSheetMenu" class="inputClass" style="display: none; position: absolute;
                            height: 150px; overflow: auto; background-color: White; border: 1px solid #ccc;
                            padding-top: 2px; z-index: 100;">
                            <ul id="ddlSheetMenu" class="inputClass" style="margin-top: 5; margin-left: 0px;
                                list-style: none;">
                            </ul>
                        </div>
                        <span style="color: Red;">*</span>
                    </div>
                </td>
            </tr>
            <tr class="pop">
                <td style="height: 30px;">
                    店铺规模大小
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPOSScale" runat="server" MaxLength="20" CssClass="inputClass"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
                <td style="height: 30px;">
                    物料支持级别
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlMaterialSupport" runat="server">
                        <asp:ListItem Value="">--请选择--</asp:ListItem>
                        <asp:ListItem Value="Basic">Basic</asp:ListItem>
                        <asp:ListItem Value="Premium">Premium</asp:ListItem>
                        <asp:ListItem Value="VVIP">VVIP</asp:ListItem>
                        <asp:ListItem Value="MCS">MCS</asp:ListItem>
                        <asp:ListItem Value="Generic">Generic</asp:ListItem>
                        <asp:ListItem Value="Others">其他</asp:ListItem>
                    </asp:DropDownList>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="pop">
                <td style="height: 30px;">
                    器架名称
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlMachineFrame" runat="server">
                        <asp:ListItem Value="">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td style="height: 30px;">
                    位置描述
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPositionDescription" runat="server" MaxLength="20" CssClass="inputClass"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="pop">
                <td style="height: 30px;">
                    性别
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <div style="position: relative;">
                        <input type="text" id="txtGender" style="width: 120px;" />
                        <div id="divGenderMenu" style="display: none; position: absolute; width: 125px; background-color: White;
                            border: 1px solid #ccc; padding-top: 2px; z-index: 100;">
                            <ul id="ddlGenderMenu" style="margin-top: 0; width: 125px; margin-left: 0px; list-style: none;">
                                <li>男</li>
                                <li>女</li>
                                <li>男女不限</li>
                                <li>无</li>
                            </ul>
                        </div>
                        <span style="color: Red;">*</span>
                    </div>
                </td>
                <td style="height: 30px;">
                    数量
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtQuantity" runat="server" MaxLength="3"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="pop">
                <td style="height: 30px;">
                    POP宽
                </td>
                <td style="text-align: left; padding-left: 5px; width: 250px;">
                    <asp:TextBox ID="txtGraphicWidth" runat="server" MaxLength="8" CssClass="inputClass"></asp:TextBox>(mm)
                    <span style="color: Red;">*</span>
                </td>
                <td style="height: 30px;">
                    POP高
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtGraphicLength" runat="server" MaxLength="8" CssClass="inputClass"></asp:TextBox>(mm)
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="pop">
                <td style="height: 30px;">
                    POP材质
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlMaterialCategory" runat="server">
                        <asp:ListItem Value="0">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlMaterial" runat="server">
                        <asp:ListItem Value="">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr class="pop">
                <td style="height: 30px;">
                    选图
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtChooseImg" runat="server" MaxLength="20" CssClass="inputClass"></asp:TextBox>
                </td>
                <td>
                    备注
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtRemark" runat="server" MaxLength="50" Style="width: 200px;"></asp:TextBox>
                </td>
            </tr>
            <tr class="price" style="display: none;">
                <td style="height: 30px; width: 100px;">
                    应收费用
                </td>
                <td style="text-align: left; width: 250px; padding-left: 5px;">
                    <asp:TextBox ID="txtReceivePrice" runat="server" MaxLength="20" CssClass="inputClass"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
                <td style="width: 100px;">
                    应付费用
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPayPrice" runat="server" MaxLength="50" CssClass="inputClass"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="price" style="display: none;">
                <td style="height: 30px;">
                    费用备注
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPriceRemark" runat="server" MaxLength="100" Style="width: 500px;"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    <div id="editShopDiv" title="编辑店铺信息" style="display: none;">
        <table id="tbEditShop" class="table" style="width: 800px; text-align: center; margin-bottom: 50px;">
            <tr>
                <td style="width: 100px; height: 30px;">
                    店铺编号
                </td>
                <td style="text-align: left; width: 180px; padding-left: 5px;">
                    <asp:Label ID="labShopNo" runat="server" Text=""></asp:Label>
                </td>
                <td style="width: 100px; height: 30px;">
                    店铺名称
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labShopName" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    店铺规模大小
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="tb_shopPOSScale" runat="server" MaxLength="20"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    物料支持级别
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddl_shopMaterialSupport" runat="server">
                        <asp:ListItem Value="">--请选择--</asp:ListItem>
                        <asp:ListItem Value="Basic">Basic</asp:ListItem>
                        <asp:ListItem Value="Premium">Premium</asp:ListItem>
                        <asp:ListItem Value="VVIP">VVIP</asp:ListItem>
                        <asp:ListItem Value="MCS">MCS</asp:ListItem>
                        <asp:ListItem Value="Generic">Generic</asp:ListItem>
                        <asp:ListItem Value="Others">其他</asp:ListItem>
                    </asp:DropDownList>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    Channel
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="tb_ShopChannel" runat="server" MaxLength="20"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    Format
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="tb_ShopFormat" runat="server" MaxLength="20"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    城市级别
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddl_ShopCityTier" runat="server">
                        <asp:ListItem Value="">--请选择--</asp:ListItem>
                        <asp:ListItem Value="T1">T1</asp:ListItem>
                        <asp:ListItem Value="T2">T2</asp:ListItem>
                        <asp:ListItem Value="T3">T3</asp:ListItem>
                        <asp:ListItem Value="T4">T4</asp:ListItem>
                        <asp:ListItem Value="T5">T5</asp:ListItem>
                        <asp:ListItem Value="T6">T6</asp:ListItem>
                        <asp:ListItem Value="T7">T7</asp:ListItem>
                    </asp:DropDownList>
                    <span style="color: Red;">*</span>
                </td>
                <td style="height: 30px;">
                    是否安装
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddl_IsInstall" runat="server">
                        <asp:ListItem Value="">--请选择--</asp:ListItem>
                        <asp:ListItem Value="Y">Y</asp:ListItem>
                        <asp:ListItem Value="N">N</asp:ListItem>
                    </asp:DropDownList>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    店铺状态
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:RadioButtonList ID="rbl_ShopStatus" runat="server" RepeatDirection="Horizontal"
                        RepeatLayout="Flow">
                        <asp:ListItem Value="正常">正常&nbsp;</asp:ListItem>
                        <asp:ListItem Value="关闭">关闭&nbsp;</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td style="height: 30px;">
                </td>
                <td style="text-align: left; padding-left: 5px;">
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField ID="hfIsRegionSubject" runat="server" />
    </form>
</body>
</html>
<script src="js/list.js" type="text/javascript"></script>
