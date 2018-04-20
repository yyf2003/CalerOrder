<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddNewShopOrder.aspx.cs"
    Inherits="WebApp.Subjects.RegionSubject.AddNewShopOrder" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
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
        #editPropDiv li
        {
            margin-bottom: 2px;
            height: 20px;
            font-size: 14px;
            cursor: pointer;
            padding-left: 5px;
            color: Blue;
            text-align: left;
        }
        #editPropDiv li:hover
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
    <blockquote class="layui-elem-quote" style="height: 20px; font-weight: bold;">
        项目信息</blockquote>
    <table class="layui-table" style="margin-top: -10px;">
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
    <!--提交按钮-->
    <div style="margin-top: 10px; text-align: center; margin-bottom: 30px;">
        <asp:Button ID="btnSubmit" runat="server" Text="提 交" OnClick="btnSubmit_Click" OnClientClick="return CheckOrder();"
            CssClass="layui-btn layui-btn-normal" />
        <%--<span id="spanSubmit" class="layui-btn layui-btn-normal">提 交</span>--%>
        <img id="imgLoading" style="display: none;" src='../../Image/WaitImg/loadingA.gif' />
        <asp:Button ID="btnBack" runat="server" Text="返 回"
            CssClass="layui-btn layui-btn-primary" style="margin-left: 30px;" 
            onclick="btnBack_Click"/>
       <%-- <span id="spanReturn" onclick="javascript:window.history.go(-1)" class="layui-btn layui-btn-primary"
            style="margin-left: 30px;">返 回</span>--%>
    </div>
    <div id="popDetailDiv" style="display: none; padding-top: 10px; position: relative;
        z-index: 9999;">
        <blockquote class="layui-elem-quote">
            请输入店铺编号：
            <input type="text" id="txtShopNo" maxlength="30" />
            &nbsp; <span id="btnGetPOP" class="layui-btn layui-btn-small"><i class="layui-icon">
                &#xe615;</i>获取POP信息</span>
                &nbsp; <span id="btnSetProp" class="layui-btn layui-btn-small">
                    <i class="layui-icon">&#xe620;</i>童店道具设置</span>
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
                        <option value="F3">F3</option>
                        <option value="YA">YA</option>
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
            <table class="layui-table" style="width: 2200px;">
                <thead>
                    <tr>
                        <th>
                            <input type="checkbox" id="cbAllPOP" />
                        </th>
                        <th>
                            类型
                        </th>
                        <th>
                            选图项目
                        </th>
                        <th>
                            选图备注
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
                            备注
                        </th>
                        <th>
                            操作
                        </th>
                    </tr>
                </thead>
                <tbody id="tbodyPOPEmpty">
                    <tr>
                        <td colspan="16" style="text-align: center;">
                            --暂无数据--
                        </td>
                    </tr>
                </tbody>
                <tbody id="tbodyPOPData" style="display: none;">
                </tbody>
            </table>
        </div>
        <div style="height: 60px; bottom: 10px; margin-top: 10px; background-color: White;
            font-size: 12px;">
            <fieldset class="layui-elem-field">
                <legend style="font-weight: bold;">添加POP订单</legend>
                <div class="layui-field-box" style="width: 97%; background-color: White;">
                    <table id="addPOPTable" style="text-align: center; font-size: 12px; margin-top: -5px;
                        width: 100%;">
                        <thead style="font-weight: bold;">
                            <tr>
                                <td style="height: 30px; text-align: left; padding-left: 5px;">
                                    选图项目
                                </td>
                                <td>
                                    选图备注
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
                                </td>
                                <td>
                                </td>
                                <td>
                                </td>
                                <td>
                                </td>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td style="height: 30px; text-align: left; padding-left: 5px;">
                                    活动：
                                    <input id="txtAddGuidanceMonth" class="Wdate" onclick="WdatePicker({dateFmt:'yyyy年MM月',readOnly:true,onpicked:getMonthAdd})"
                                        style="width: 90px;" />
                                    <select id="seleAddGuidance">
                                        <option value="0">--请选择活动--</option>
                                    </select>
                                    项目：
                                    <select id="seleAddSubject">
                                        <option value="0">--请选择项目--</option>
                                    </select>
                                </td>
                                <td>
                                    <input type="text" id="txtAddChooseImg" maxlength="30" style="width: 95%; text-align: center;" />
                                </td>
                                <td>
                                    <select id="seleAddOrderType">
                                        <option value="1">POP</option>
                                        <option value="2">道具</option>
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
                            </tr>
                            <tr style="font-weight: bold;">
                                <td style="height: 30px; text-align: left; padding-left: 5px;">
                                    材质
                                </td>
                                <td>
                                    位置描述
                                </td>
                                <td colspan="3">
                                    备注
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td style="height: 30px; text-align: left; padding-left: 5px;">
                                    <select id="seleAddMaterial">
                                        <option value="">--请选择材质--</option>
                                    </select>
                                </td>
                                <td>
                                    <input type="text" id="txtAddPositionDescription" maxlength="30" style="width: 90%;
                                        text-align: center;" />
                                </td>
                                <td colspan="3">
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
            <fieldset class="layui-elem-field">
                <legend style="font-weight: bold;">添加费用</legend>
                <table id="addPriceTable" style="width: 100%;text-align: center; margin-bottom:10px;">
                   <tr>
                      <td style=" width:100px;">费用类型：</td>
                      <td style=" width:150px; text-align:left; padding-left:5px;">
                         <select id="selePriceType">
                            <option value="0">--请选择--</option>
                            <option value="4">安装费</option>
                            <option value="5">发货费</option>
                            <option value="6">测量费</option>
                            <option value="7">其他费用</option>
                         </select>
                      </td>
                      <td style=" width:80px;">应收金额：</td>
                      <td style=" width:100px; text-align:left; padding-left:5px;">
                          <input type="text" id="txtAddPriceNum" maxlength="10" style="width: 95%; text-align: center;" />
                      </td>
                      <td style=" width:80px;">应付金额：</td>
                      <td style=" width:100px; text-align:left; padding-left:5px;">
                          <input type="text" id="txtPayPriceNum" maxlength="10" style="width: 95%; text-align: center;" />
                      </td>
                      <td style=" width:80px;">备注：</td>
                      <td style=" width:180px; text-align:left; padding-left:5px;">
                          <input type="text" id="txtAddPriceRemark" maxlength="50" style="width: 95%; text-align: center;" />
                      </td>
                      <td style="text-align:left; padding-left:5px;">
                         <span id="spanAddPrice" class="layui-btn layui-btn-small">添加</span>
                      </td>
                   </tr>
                </table>
            </fieldset>
        </div>
    </div>
    <!--查看单店POP订单明细-->
    <div id="showPOPOrderDetailDiv" title="查看POP信息" style="display: none;">
        <div class="layui-btn-group" style="margin-top: 10px;">
            <span id="btnAddFromDetail" class="layui-btn layui-btn-small"><i class="layui-icon">
                &#xe654;</i>添加POP</span> <span id="btnEdit" class="layui-btn layui-btn-small"><i
                    class="layui-icon">&#xe642;</i>修改</span> <span id="btnDelete" class="layui-btn layui-btn-small">
                        <i class="layui-icon">&#xe640;</i>删除</span>
        </div>
        <div style="overflow: auto; padding-bottom: 10px;">
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
                            类型
                        </th>
                        <%--<th>
                            店铺编号
                        </th>--%>
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
    <div id="editDiv" title="编辑POP信息" style="display: none; z-index: 999;">
        <table id="POPtable" class="table" style="width: 750px; text-align: center;">
            <tr>
                <td style="height: 30px;">
                    类型
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labOrderTypeEdit" runat="server" Text=""></asp:Label>
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
                        <asp:ListItem Value="F3">F3</asp:ListItem>
                        <asp:ListItem Value="YA">YA</asp:ListItem>
                        
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


    <!--设置童店新开店道具-->
    <div id="editPropDiv" style="display: none;">
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 100px;">
                    物料支持级别：
                </td>
                <td style="width: 300px; text-align: left; padding-left: 5px;">
                    <select id="selPropMaterialSupport">
                        <option value="">--请选择--</option>
                        <option value="F3">F3</option>
                        <option value="YA">YA</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
                <td style="width: 100px;">
                    道具名称：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtPropName" maxlength="20"/>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    性别：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <div style="position: relative;">
                        <input type="text" id="txtPropGender" maxlength="10"/>
                        <div id="divPropGenderMenu" style="display: none; position: absolute; width: 150px;
                            height: 80px; overflow: auto; background-color: White; border: 1px solid #ccc;
                            padding-top: 2px; z-index: 99991;">
                            <ul id="dllPropGenderMenu" style="margin-top: 0; width: 140px; list-style: none;">
                                <li>男</li>
                                <li>女</li>
                                <li>男女不限</li>
                            </ul>
                        </div>
                    </div>
                </td>
                <td>
                    数量：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtPropQuantity" value="1" maxlength="2"/>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    材质：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    位置描述：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtPropPositionDescription" maxlength="40"/>
                </td>
                <td>
                    备注：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtPropRemark" style="width: 200px;" maxlength="40"/>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px; height: 35px;">
                    <span id="btnAddProp" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe654;</i>提
                        交</span>
                    <img id="imgPropLoading" src="../../image/loadingA.gif" style=" display:none;"/>
                </td>
            </tr>
        </table>
        <table class="layui-table">
            <thead>
                <tr>
                    <th style=" width:30px;">
                        序号
                    </th>
                    <th>
                        物料支持级别
                    </th>
                    <th>
                        道具名称
                    </th>
                    <th>
                        性别
                    </th>
                    <th>
                        数量
                    </th>
                    <th>
                        材质
                    </th>
                    <th>
                        位置描述
                    </th>
                    <th>
                        备注
                    </th>
                    <th style=" width:70px;">
                        操作
                    </th>
                </tr>
            </thead>
            <tbody id="tbodyPropEmpty">
                <tr>
                    <td colspan="9" style="text-align: center;">
                        --暂无数据--
                    </td>
                </tr>
            </tbody>
            <tbody id="tbodyPropData" style="display: none;">
            </tbody>
        </table>
    </div>



    <div id="editPriceDiv" title="编辑费用订单" style="display: none; z-index:999;">
        <table id="Table1" class="table" style="width: 850px; text-align: center;">
            <tr>
                <td style="height: 30px;">
                    类型
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <select id="seleAddPriceType">
                       <option value="安装费">安装费</option>
                       <option value="测量费">测量费</option>
                       <option value="发货费">发货费</option>
                       <option value="其他费用">其他费用</option>
                       
                     </select>
                </td>
            </tr>
            <tr>
                <td style="width: 100px; height: 30px;">
                    应收金额
                </td>
                <td style="text-align: left; width: 250px; padding-left: 5px;">
                    <asp:TextBox ID="txtOrderPrice" runat="server" MaxLength="10"></asp:TextBox>
                </td>
                <td style="width: 100px; height: 30px;">
                    应付金额
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPayOrderPrice" runat="server" MaxLength="10"></asp:TextBox>
                </td>
            </tr>
           
            <tr>
                <td style="height: 30px;">
                    备注
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPriceRemarkEdit" runat="server" MaxLength="50" Style="width: 200px;"></asp:TextBox>
                </td>
                
            </tr>
        </table>
    </div>


    <asp:HiddenField ID="hfIsChange" runat="server" />
    </form>
</body>
</html>
<script src="../../Scripts/common.js" type="text/javascript"></script>
<script src="js/addNewShopOrder.js" type="text/javascript"></script>
