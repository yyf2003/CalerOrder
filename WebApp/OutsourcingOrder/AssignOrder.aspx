<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssignOrder.aspx.cs" Inherits="WebApp.OutsourcingOrder.AssignOrder" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="../easyui1.4/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.pagination.js" type="text/javascript"></script>
    <link href="../CSS/pagination.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            外协分单
        </p>
    </div>
    <div class="tr">
        >>选择项目</div>
    <table class="table">
        <tr class="tr_bai">
            <td>
                客户
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:DropDownList ID="ddlCustomer" runat="server">
                </asp:DropDownList>
                <span style="color: Red;">*</span>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                活动月份
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:TextBox ID="txtMonth" runat="server" CssClass="Wdate" onfocus="WdatePicker({skin:'whyGreen',dateFmt:'yyyy-MM',isShowClear:false,readOnly:true,onpicked:getMonth})"
                    Style="width: 80px;"></asp:TextBox>
                <span id="spanUp" style="margin-left: 10px; cursor: pointer; color: Blue;">上一个月</span>
                <span id="spanDown" style="margin-left: 20px; cursor: pointer; color: Blue;">下一个月</span>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目
            </td>
            <td style="padding-left: 0px; height: 80px; vertical-align: top;">
                <table style="width: 100%;">
                    <tr class="tr_hui">
                        <td style="width: 80px;">
                            活动名称：
                        </td>
                        <td style="text-align: left;">
                            <div id="guidanceDiv">
                            </div>
                        </td>
                    </tr>
                    <tr class="trType" style="display: none;">
                        <td>
                            活动类型：
                        </td>
                        <td style="text-align: left;">
                            <div id="activityDiv">
                            </div>
                        </td>
                    </tr>
                    <tr class="trType" style="display: none;">
                        <td>
                            项目分类：
                        </td>
                        <td style="text-align: left;">
                            <div id="typeDiv">
                            </div>
                        </td>
                    </tr>
                </table>
                <div id="projectLoadImg" class="loadingImg" style="display: none;">
                    <img src="../image/WaitImg/loadingA.gif" />
                </div>
                <div class="trType" style="width: 80%; margin: 5px; text-align: left; display: none;">
                    <input type="checkbox" id="cbALL" /><span style="color: Blue;">全选</span>
                </div>
                <div id="projectsDiv" style="width: 80%; margin: 5px; text-align: left;">
                </div>
            </td>
        </tr>
    </table>
    <div class="tr">
        >>条件</div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                区域
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="regionLoadImg" class="loadingImg" style="display: none;">
                    <img src="../image/WaitImg/loadingA.gif" />
                </div>
                <div id="RegionDiv">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                省份
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="provinceLoadImg" class="loadingImg" style="display: none;">
                    <img src="../image/WaitImg/loadingA.gif" />
                </div>
                <div id="ProvinceDiv" style="width: 90%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                城市
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="cityLoadImg" class="loadingImg" style="display: none;">
                    <img src="../image/WaitImg/loadingA.gif" />
                </div>
                <div id="cbAllDiv" style="display: none;">
                    <input type="checkbox" id="cbAllCity" />全选
                </div>
                <div id="CityDiv" style="width: 90%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                客服
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="csLoadImg" class="loadingImg" style="display: none;">
                    <img src="../image/WaitImg/loadingA.gif" />
                </div>
                <div id="CustomerServiceNameDiv" style="width: 90%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                城市级别
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="cityTierLoadImg" class="loadingImg" style="display: none;">
                    <img src="../image/WaitImg/loadingA.gif" />
                </div>
                <div id="cityTierDiv" style="width: 90%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                是否安装
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="IsInstallLoadImg" class="loadingImg" style="display: none;">
                    <img src="../image/WaitImg/loadingA.gif" />
                </div>
                <div id="IsInstallDiv" style="width: 90%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                Channel
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="ChannelLoadImg" class="loadingImg" style="display: none;">
                    <img src="../image/WaitImg/loadingA.gif" />
                </div>
                <div id="ChannelDiv" style="width: 90%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                Format
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="FormatLoadImg" class="loadingImg" style="display: none;">
                    <img src="../image/WaitImg/loadingA.gif" />
                </div>
                <div id="FormatDiv" style="width: 90%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                POP位置
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="SheetLoadImg" class="loadingImg" style="display: none;">
                    <img src="../image/WaitImg/loadingA.gif" />
                </div>
                <div id="cbAllSheetDiv" style="display: none;">
                    <input type="checkbox" id="cbAllSheet" />全选
                </div>
                <div id="SheetDiv" style="width: 90%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                软膜
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:CheckBoxList ID="cblRuanMo" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    <asp:ListItem Value="1">软膜&nbsp;&nbsp;&nbsp;</asp:ListItem>
                    <asp:ListItem Value="2">非软膜</asp:ListItem>
                </asp:CheckBoxList>
                &nbsp; &nbsp; （北京店铺不用选择）
            </td>
        </tr>
       
        <tr class="tr_bai">
            <td style="width: 120px;">
                按材质
            </td>
            <td style="text-align: left; padding-left: 0px;">
                <table style="width: 100%;">
                    <tr>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:CheckBoxList ID="cblMaterialAssign" runat="server" RepeatDirection="Horizontal"
                                RepeatLayout="Flow">
                                <asp:ListItem Value="网格布">网格布&nbsp;&nbsp;&nbsp;</asp:ListItem>
                                <asp:ListItem Value="即时贴">即时贴&nbsp;&nbsp;&nbsp;</asp:ListItem>
                                <asp:ListItem Value="背胶">背胶PP+雪弗板&nbsp;&nbsp;&nbsp;</asp:ListItem>
                                <asp:ListItem Value="非背胶">非背胶PP+雪弗板</asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: left; padding-left: 5px;">
                            背胶PP+雪弗板：
                            <asp:CheckBoxList ID="cblMaterialPlan" runat="server" RepeatDirection="Horizontal"
                                RepeatLayout="Flow">
                                <asp:ListItem Value="背胶PP">背胶PP&nbsp;&nbsp;&nbsp;</asp:ListItem>
                                <asp:ListItem Value="雪弗板">雪弗板</asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                   
                </table>
              
            </td>
        </tr>
         <tr class="tr_bai">
                        <td style="width: 120px;">
                            其他材质
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:TextBox ID="txtOtherMaterial" runat="server" MaxLength="20"></asp:TextBox>
                        </td>
                    </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
            </td>
            <td style="height: 30px; text-align: left; padding-left: 5px;">
                <input type="button" value="查 询" id="btnSearch" class="easyui-linkbutton" style="width: 65px;
                    height: 26px;" />
                <input type="button" value="导出已分配店铺" id="btnExportAssignShop" class="easyui-linkbutton"
                    style="width: 100px; height: 26px; margin-left: 15px; display: none;" />
            </td>
        </tr>
    </table>
    <br />
    <div>
        <table class="table">
            <tr class="tr_hui">
                <td style="width: 120px;">
                    店铺数量
                </td>
                <td style="text-align: left; padding-left: 5px; width: 100px;">
                    <asp:Label ID="labShopCount" runat="server" Text="0"></asp:Label>
                </td>
                <td style="width: 120px;">
                    订单数量
                </td>
                <td style="text-align: left; padding-left: 5px; width: 100px;">
                    <asp:Label ID="labOrderCount" runat="server" Text="0"></asp:Label>
                    <span id="spanCheckOrderDetail" style="color: Blue; cursor: pointer; margin-left: 20px;
                        display: none;">查看</span>
                </td>
                <td style="width: 120px;">
                    重复订单
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labHasRepeated" runat="server" Text=""></asp:Label>
                    <span id="spanCheckRepeat" style="color: Red; cursor: pointer; display: none; margin-left: 15px;">
                        查看</span>
                </td>
            </tr>
            <tr class="tr_hui">
                <td style="width: 120px;">
                    已分店铺数量
                </td>
                <td style="text-align: left; padding-left: 5px; width: 100px;">
                    <asp:Label ID="labAssignShopCount" runat="server" Text="0"></asp:Label>
                </td>
                <td style="width: 120px;">
                    未分店铺数量
                </td>
                <td style="text-align: left; padding-left: 5px; width: 100px;">
                    <asp:Label ID="labNotAssignShopCount" runat="server" Text="0"></asp:Label>
                </td>
                <td style="width: 120px;">
                    已分订单
                </td>
                <td style="text-align: left; padding-left: 5px; width: 100px;">
                    <asp:Label ID="labAssignTotalOrderCount" runat="server" Text="0"></asp:Label>
                </td>
                <td style="width: 100px;">
                    未分订单
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labNotAssignTotalOrderCount" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    <div class="tr">
        >>店铺信息</div>
    <div style="overflow: auto;">
        <div id="divClean" style="margin-bottom: 2px;">
            <div style="float: left; width:40px; color:Blue;padding-top: 5px; padding-left: 10px;">
             搜索：
            </div>
            <div style="float: left; padding-top: 5px; ">
                <input type="checkbox" name="cbAssignState" value="0" />未完成分配&nbsp;
                <input type="checkbox" name="cbAssignState" value="1" />已完成分配&nbsp;
                <input type="text" id="txtSearchShopNo" />
                <img id="imgLoading" style="display: none;" src='../image/WaitImg/loadingA.gif' />
            </div>
            <%-- <div style="float: left; width:40px; color:Blue;padding-top: 5px; padding-left: 10px;">
             操作：
            </div>
            <div style="float: left;">
                <a id="btnClear" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-cancel">
                    撤销</a> <a id="btnClearAll" style="float: left;" class="easyui-linkbutton" plain="true"
                        icon="icon-cancel">全部撤销</a>
            </div>
            <div style="color:Blue;float: left;padding-top: 5px; margin-left: 15px;">
              提示：撤销操作是按店整体撤销
            </div>--%>
        </div>
        <table id="noData" class="table">
            <tr class="tr_bai">
                <td style="text-align: center; height: 30px;">
                    --无可显示的信息--
                </td>
            </tr>
        </table>
        <div id="divload" style="display: none; text-align: center;">
            <img src="../image/WaitImg/loading1.gif" />
        </div>
        <table id="popListTB" class="table" style="display: none;">
            <thead>
                <tr class="tr_hui">
                    <td style="width: 30px;">
                        序号
                    </td>
                    <td style="width: 30px;">
                        <input type="checkbox" id="cbAll1" />
                    </td>
                    <td>
                        外协名称
                    </td>
                    <td>
                        店铺编号
                    </td>
                    <td>
                        店铺名称
                    </td>
                    <td>
                        区域
                    </td>
                    <td>
                        省份
                    </td>
                    <td>
                        城市
                    </td>
                    <td>
                        城市级别
                    </td>
                    <td>
                        安装级别
                    </td>
                    <td>
                        店铺类型
                    </td>
                    <td>
                        订单数量
                    </td>
                    <td>
                        已分数量
                    </td>
                </tr>
            </thead>
            <tbody id="listBody">
            </tbody>
        </table>
    </div>
    <div id="Pagination" class="sabrosus" style="display: none;">
    </div>
    <div class="tr" style="margin-top: 20px;">
        >>外协信息</div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                请选择生产外协：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <select id="seleCompany">
                    <option value="0">--请选择--</option>
                </select>
                &nbsp;&nbsp;&nbsp; 订单类型：
                <asp:RadioButtonList ID="rblOrderType" runat="server" RepeatDirection="Horizontal"
                    RepeatLayout="Flow">
                    <asp:ListItem Value="1">安装&nbsp;&nbsp;</asp:ListItem>
                    <asp:ListItem Value="2">发货 </asp:ListItem>
                </asp:RadioButtonList>
                <%--&nbsp;&nbsp;&nbsp;
                <input type="checkbox" id="cbNoInstallPrice"/><span style=" color:Blue;">无安装费</span>--%>
                &nbsp;&nbsp;
                <span id="expressType" style=" display:none;">
                   <input type="checkbox" id="cbNoExpressPrice"/><span style=" color:Blue;">无快递费</span>
                   &nbsp;
                   <input type="checkbox" id="cbKeepExpressPrice"/><span style=" color:Blue;">快递费归原外协</span>
                   &nbsp;
                   <input type="checkbox" id="Checkbox1"/><span style=" color:Blue;">保留归原外协快递费，新外协新增快递费</span>
                </span>

            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                请选择安装外协：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <select id="seleInstallCompany" disabled="disabled">
                    <option value="0">--请选择--</option>
                </select>
                (提示：安装外协只有安装费，没有POP制作费)
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <input type="button" value="分 配" id="btnSubmit" class="easyui-linkbutton" style="width: 65px;
                    height: 26px;" />
                <img id="imgSubmit1" style="display: none;" src='../image/WaitImg/loadingA.gif' />
                <input type="button" value="全部分配" id="btnSubmitAll" class="easyui-linkbutton" style="width: 65px;
                    height: 26px; margin-left: 20px;" />
                <img id="imgSubmit2" style="display: none;" src='../image/WaitImg/loadingA.gif' />
            </td>
        </tr>
    </table>
    <table id="MsgTB" style="display: none; margin-top: 10px;">
        <tr>
            <td style="width: 120px; height: 50px; text-align: right;">
                <asp:Label ID="labState" runat="server" Text="" Style="color: red; font-size: 14px;"></asp:Label>
            </td>
            <td class="nav_table_tdleft">
                <asp:Label ID="labTips" runat="server" Text="" Style="color: red; font-size: 14px;"></asp:Label>
            </td>
        </tr>
    </table>
    <table id="AssignStateTB" class="table" style="display: none; margin-top: 10px;">
        <tr class="tr_hui">
            <td style="width: 80px;">
                订单数量：
            </td>
            <td style="text-align: left; padding-left: 5px; width: 100px;">
                <span id="spanTotalOrderCount"></span>
            </td>
            <td style="width: 150px;">
                成功分配订单数量：
            </td>
            <td style="text-align: left; padding-left: 5px; width: 100px;">
                <span id="spanAssignOrderCount"></span>
            </td>
            <td style="width: 100px;">
                重复订单数量：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <span id="spanRepeatOrderCount"></span>
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hfCityData" runat="server" />
    <p style="margin-bottom: 100px;">
    </p>
    </form>
</body>
</html>
<link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
<script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
<script src="../Scripts/common.js" type="text/javascript"></script>
<script src="js/AssignOrder.js" type="text/javascript"></script>
