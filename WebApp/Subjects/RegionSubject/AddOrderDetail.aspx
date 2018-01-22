﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddOrderDetail.aspx.cs"
    Inherits="WebApp.Subjects.RegionSubject.AddOrderDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <style type="text/css">
        #popDetailDiv li
        {
            margin-bottom: 2px;
            height: 20px;
            font-size: 14px;
            cursor: pointer;
            padding-left: 5px;
            color: Blue;
            text-align: left;
        }
        #popDetailDiv li:hover
        {
            background-color: #f0f1f2;
            text-decoration: underline;
        }
        #editDiv li
        {
            margin-bottom: 2px;
            height: 20px;
            font-size: 14px;
            cursor: pointer;
            padding-left: 5px;
            color: Blue;
            text-align: left;
        }
        #editDiv li:hover
        {
            background-color: #f0f1f2;
            text-decoration: underline;
        }
    </style>
    <script type="text/javascript">
        var subjectId = '<%=SubjectId %>';
        function MakeChange() {
            $("#hfIsChange").val("1");
        }
    </script>
</head>
<body class="tab">
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            添加订单信息
        </p>
    </div>
    <blockquote class="layui-elem-quote" style=" height:15px;font-weight:bold; padding-top :7px; padding-left:5px;">
        项目信息</blockquote>
    <table class="layui-table" style=" margin-top:-10px;">
        <tr>
            <td style="width: 100px;">
                项目编号：
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 100px;">
                项目名称：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                项目创建人：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labAddUserName" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
                所属客户：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labCustomerName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                订单类型：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectType" runat="server" Text=""></asp:Label>
            </td>
            <td>
                区域：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labRegion" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                备注：
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labRemark" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <div style="margin-top: 15px;">
        <div class="layui-btn-group">
            <span id="btnAdd" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe654;</i>添加POP</span>
            <span id="btnImport" class="layui-btn layui-btn-small" style=" margin-left:30px;"><i class="layui-icon">&#xe654;</i>批量导入</span>
        </div>
    </div>
    <div class="layui-tab layui-tab-brief" lay-filter="order">
        <ul class="layui-tab-title">
            <li class="layui-this" lay-id="1">POP信息</li>
            <%--<li lay-id="2">物料信息</li>--%>
        </ul>
        <div class="layui-tab-content" style="overflow: auto;">
            <div class="layui-tab-item layui-show">
                
                <table class="layui-table">
                    <colgroup>
                        <col width="60">
                        <col>
                    </colgroup>
                    <thead>
                        <tr>
                            <th>
                                序号
                            </th>
                            <th>
                                审批状态
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
                                Channel
                            </th>
                            <th>
                                Format
                            </th>
                            <th>
                                渠道
                            </th>
                            <th>
                                POP数量
                            </th>
                            <th>
                                POP明细
                            </th>
                        </tr>
                    </thead>
                    <tbody id="tbodyOrderShopEmpty">
                        <tr>
                            <td colspan="12" style="text-align: center;">
                                --暂无数据--
                            </td>
                        </tr>
                    </tbody>
                    <tbody id="tbodyOrderShopData" style="display: none;">
                    </tbody>
                </table>
            </div>
           
        </div>
    </div>
    <div style="margin-top: 10px; text-align: center; margin-bottom: 30px;">
        <asp:Button ID="btnSubmit" runat="server" Text="提 交" OnClick="btnSubmit_Click"
            OnClientClick="return CheckOrder();" CssClass="layui-btn layui-btn-normal" />
        <%--<span id="spanSubmit" class="layui-btn layui-btn-normal">提 交</span>--%>
        <img id="imgLoading" style="display: none;" src='../../Image/WaitImg/loadingA.gif' />
       <%-- <span id="spanReturn" onclick="javascript:window.history.go(-1)" class="layui-btn layui-btn-primary"
            style="margin-left: 30px;">返 回</span>--%>
        <asp:Button ID="btnBack" runat="server" Text="返 回" 
            CssClass="layui-btn layui-btn-primary" style="margin-left: 30px;" 
            onclick="btnBack_Click"/>
        
    </div>
    <div id="popDetailDiv" style="display: none; padding-top: 10px; position: relative; z-index:9999;">
        <blockquote class="layui-elem-quote">
            请输入店铺编号：
            <input type="text" id="txtShopNo" maxlength="30" />
            &nbsp; <span id="btnGetPOP" class="layui-btn layui-btn-small"><i class="layui-icon">
                &#xe615;</i>获取POP信息</span>
            <img id="getPOPLoading" src="../../image/WaitImg/loadingA.gif" style="margin-left: 5px;
                display: none;" />
            <span id="getPOPMsg" style="color: Red; display: none;"></span>
        </blockquote>
        <table style="width: 90%; margin-left: 10px;">
            <tr class="tr_bai">
                <td style="width: 100px;">
                    物料支持级别：
                </td>
                <td style="width: 150px; text-align: left; padding-left: 5px;">
                    <select id="seleMaterialSupport">
                        <option value="">--请选择--</option>
                        <option value="BASIC">Basic</option>
                        <option value="PREMIUM">Premium</option>
                        <option value="VVIP">VVIP</option>
                        <option value="MCS">MCS</option>
                        <option value="GENERIC">Generic</option>
                    </select>
                </td>
                <td style="width: 100px;">
                    店铺规模大小：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtPOSScale" maxlength="20" />
                </td>
            </tr>
        </table>
        <div style="overflow: auto;">
            <table class="layui-table" style="width: 1500px;">
                <thead>
                    <tr>
                        <th>
                            <input type="checkbox" id="cbAllPOP" />
                        </th>
                        <th>
                            项目
                        </th>
                        <th>
                            类型
                        </th>
                        
                        <th>
                            Sheet
                        </th>
                        <th>
                            GraphicNo
                        </th>
                        <th>
                            男/女
                        </th>
                        <th>
                            数量
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
                            操作
                        </th>
                    </tr>
                </thead>
                <tbody id="tbodyPOPEmpty">
                    <tr>
                        <td colspan="14" style="text-align: center;">
                            --暂无数据--
                        </td>
                    </tr>
                </tbody>
                <tbody id="tbodyPOPData" style="display: none;">
                </tbody>
            </table>
        </div>
        <div style="height: 40px; bottom: 10px; margin-top: 10px; background-color: White;
            font-size: 12px; z-index: 999;">
            <fieldset class="layui-elem-field">
                <legend style="font-weight: bold;">新增</legend>
                <div class="layui-field-box" style="width: 97%; background-color: White;">
                    <table id="addPOPTable" style="text-align: center; font-size: 12px; margin-top: -5px;
                        width: 100%;">
                        <thead>
                            <tr>
                                <td>
                                    项目
                                </td>
                                <td>
                                    类型
                                </td>
                                <td>
                                    POP位置
                                </td>
                                <td>
                                    性别
                                </td>
                                <td>
                                    数量
                                </td>
                                <td>
                                    宽
                                </td>
                                <td>
                                    高
                                </td>
                                <td>
                                    材质
                                </td>
                                <td>
                                    位置描述
                                </td>
                                <td>
                                    选图
                                </td>
                                <td>
                                    备注
                                </td>
                                <td>
                                </td>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>
                                    <select id="seleAddSubject">
                                        <option value="0">--请选择--</option>
                                    </select>
                                </td>
                                <td>
                                    <select id="seleAddOrderType">
                                        <option value="POP">POP</option>
                                        <option value="道具">道具</option>
                                    </select>
                                </td>
                                <td>
                                    <div style="position: relative;">
                                        <input type="text" id="txtAddSheet" maxlength="20" style="width: 60px; text-align: center;" />
                                        <div id="divAddSheetMenu" style="display: none; position: absolute; width: 120px;
                                            height: 150px; overflow: auto; background-color: White; border: 1px solid #ccc;
                                            padding-top: 2px; z-index: 99991;">
                                            <ul id="ddlAddSheetMenu" style="margin-top: 0; width: 100px; margin-left: 0px; list-style: none;">
                                            </ul>
                                        </div>
                                    </div>
                                </td>
                                <td>
                                    <select id="seleAddGender">
                                        <option value="男">男</option>
                                        <option value="女">女</option>
                                        <option value="男女不限">男女不限</option>
                                    </select>
                                </td>
                                <td>
                                    <input type="text" id="txtAddQuantity" value="1" maxlength="3" style="width: 40px;
                                        text-align: center;" />
                                </td>
                                <td>
                                    <input type="text" id="txtAddWidth" maxlength="6" style="width: 60px; text-align: center;" />
                                </td>
                                <td>
                                    <input type="text" id="txtAddLength" maxlength="6" style="width: 60px; text-align: center;" />
                                </td>
                                <td>
                                    <select id="seleAddMaterial">
                                        <option value="">--请选择材质--</option>
                                    </select>
                                </td>
                                <td>
                                    <input type="text" id="txtAddPositionDescription" maxlength="30" style="width: 90%;
                                        text-align: center;" />
                                </td>
                                <td>
                                    <input type="text" id="txtAddChooseImg" maxlength="30" style="width: 90%; text-align: center;" />
                                </td>
                                <td>
                                    <input type="text" id="txtAddRemark" maxlength="30" style="width: 90%; text-align: center;" />
                                </td>
                                <td>
                                    <span id="spanAddPOP" class="layui-btn layui-btn-small">添加</span>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </fieldset>
        </div>
    </div>
    <div id="editDiv" title="编辑POP信息" style="display: none; z-index:999;">
        <table id="POPtable" class="table" style="width: 750px; text-align: center;">
            <tr>
                <td style="height: 30px;">
                    类型
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labOrderTypeEdit" runat="server" Text=""></asp:Label>
                </td>
                <td style="height: 30px;">
                    项目
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="seleSubjectEdit">
                        <option value="0">--请选择--</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr>
                <td style="width: 100px; height: 30px;">
                    POP编号
                </td>
                <td style="text-align: left; width: 250px; padding-left: 5px;">
                    <asp:Label ID="labGraphicNoEdit" runat="server" Text=""></asp:Label>
                </td>
                <td style="width: 100px; height: 30px;">
                    POP位置
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <div style="position: relative;">
                        <input type="text" id="txtSheetEdit" maxlength="20" />
                        <span style="color: Red;">*</span>
                        <div id="divEditSheetMenu" style="display: none; position: absolute; width: 150px;
                            height: 150px; overflow: auto; background-color: White; border: 1px solid #ccc;
                            padding-top: 2px; z-index: 9999;">
                            <ul id="ddlEditSheetMenu" style="margin-top: 0; width: 150px; margin-left: 0px; list-style: none;">
                            </ul>
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    店铺规模大小
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPOSScaleEdit" runat="server" MaxLength="20" CssClass="inputClass"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    物料支持级别
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlMaterialSupportEdit" runat="server">
                        <asp:ListItem Value="">--请选择--</asp:ListItem>
                        <asp:ListItem Value="BASIC">Basic</asp:ListItem>
                        <asp:ListItem Value="PREMIUM">Premium</asp:ListItem>
                        <asp:ListItem Value="VVIP">VVIP</asp:ListItem>
                        <asp:ListItem Value="MCS">MCS</asp:ListItem>
                        <asp:ListItem Value="GENERIC">Generic</asp:ListItem>
                       
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    性别
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="seleEditGender">
                        <option value="男">男</option>
                        <option value="女">女</option>
                        <option value="男女不限">男女不限</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
                <td style="height: 30px;">
                    数量
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtQuantityEdit" runat="server" MaxLength="3"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    POP宽(mm)
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labWidthEdit" runat="server" Text=""></asp:Label>
                </td>
                <td style="height: 30px;">
                    POP高(mm)
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labLengthEdit" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    POP材质
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labMaterialEdit" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    位置描述
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPositionDescriptionEdit" runat="server" MaxLength="20" CssClass="inputClass"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    选图
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtChooseImgEdit" runat="server" MaxLength="20" CssClass="inputClass"></asp:TextBox>
                </td>
                <td>
                    备注
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtRemarkEdit" runat="server" MaxLength="50" Style="width: 200px;"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    <!--查看单店POP订单明细-->
    <div id="showPOPOrderDetailDiv" title="查看POP信息" style="display: none; ">
        <div class="layui-btn-group" style="margin-top: 10px;">
            <span id="btnAddFromDetail" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe654;</i>添加POP</span>
            <span id="btnEdit" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe642;</i>修改</span>
            <span id="btnDelete" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe640;</i>删除</span>
        </div>
        <div style=" overflow:auto; padding-bottom:10px;">
        <table class="layui-table" style="width: 1800px;">
            <colgroup>
                <col width="50">
                <col width="60">
                <col>
            </colgroup>
            <thead style="font-size: 12px;">
                <tr>
                    <th>
                        <input type="checkbox" id="cbAllOrder" />
                    </th>
                    <th>
                        序号
                    </th>
                    <th>
                        审批状态
                    </th>
                    <th>
                        项目
                    </th>
                    <th>
                        类型
                    </th>
                    <th>
                        店铺编号
                    </th>
                    <th>
                        物料支持级别
                    </th>
                    <th>
                        店铺规模大小
                    </th>
                    <th>
                        Sheet
                    </th>
                    <th>
                        GraphicNo
                    </th>
                    <th>
                        男/女
                    </th>
                    <th>
                        数量
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
                </tr>
            </thead>
            <tbody id="tbodyOrderEmpty">
                <tr>
                    <td colspan="18" style="text-align: center;">
                        --暂无数据--
                    </td>
                </tr>
            </tbody>
            <tbody id="tbodyOrderData" style="display: none;">
            </tbody>
        </table>
        </div>
    </div>
    <asp:HiddenField ID="hfIsChange" runat="server" />
    </form>
</body>
</html>
<script src="../../Scripts/common.js" type="text/javascript"></script>
<script src="js/addOrderDetail.js" type="text/javascript"></script>
