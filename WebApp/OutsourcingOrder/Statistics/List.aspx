<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.OutsourcingOrder.Statistics.List" %>

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
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="/easyui1.4/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'north',split:true," style="height: 44px; overflow: hidden;">
        <div class="nav_title">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
                class="nav_table_p">
                外协订单统计
            </p>
        </div>
    </div>
    <div data-options="region:'west',split:true,title:'外协信息'" style="width: 200px;">
        <div class="tr_hui" style="height: 30px; text-align: left; padding-left: 5px;">
            外协区域：</div>
        <div id="outsourceRegionDiv" style="padding-left: 5px; padding-top: 8px; padding-bottom: 8px;">
        </div>
        <table id="tbOutsource" style="width: 100%;">
        </table>
    </div>
    <div data-options="region:'center',title:'',height:'100%',iconCls:'icon-ok'" style="overflow: hidden;">
        <div id="orderTitle" class="easyui-panel" title=">>外协名称：" data-options="height:'100%',overflow:'auto'">
            <div class="layui-tab layui-tab-brief" lay-filter="order">
                <ul class="layui-tab-title">
                    <li class="layui-this" lay-id="1">按活动查询</li>
                    <li lay-id="2">按时间查询</li>
                </ul>
                <div class="layui-tab-content" style="overflow: auto;">
                    <div class="layui-tab-item layui-show">
                        <table class="table">
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
                                    项目名称
                                </td>
                                <td colspan="3" style="text-align: left; padding-left: 5px;">
                                    <img id="ImgLoadSubject" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                                    <input type="checkbox" id="subjectCBALL" style="color: Blue;" />全选
                                    <div id="subjectDiv" style="width: 90%;">
                                    </div>
                                </td>
                            </tr>
                            <tr class="tr_bai">
                                <td>
                                    省份
                                </td>
                                <td colspan="3" style="text-align: left; padding-left: 5px;">
                                    <img id="ImgLoadProvince" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                                    <div id="provinceDiv" style="width: 90%;">
                                    </div>
                                </td>
                            </tr>
                            <tr class="tr_bai">
                                <td>
                                    城市
                                </td>
                                <td colspan="3" style="text-align: left; padding-left: 5px;">
                                    <img id="ImgLoadCity" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                                    <div id="cityDiv" style="width: 90%;">
                                    </div>
                                </td>
                            </tr>
                            <tr class="tr_bai">
                                <td>
                                    类型
                                </td>
                                <td colspan="3" style="text-align: left; padding-left: 5px;">
                                    <asp:CheckBoxList ID="cblOutsourceType" runat="server" RepeatDirection="Horizontal"
                                        RepeatLayout="Flow">
                                        <asp:ListItem Value="1">安装&nbsp;</asp:ListItem>
                                        <asp:ListItem Value="2">发货&nbsp;</asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
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
                        <div>
                            <div class="layui-tab layui-tab-card">
                                <ul class="layui-tab-title">
                                    <li class="layui-this" lay-id="1">统计信息</li>
                                </ul>
                                <div id="divContent1" class="layui-tab-content" style="padding: 0px 10px;">
                                    <div class="layui-tab-item layui-show" style="height: 100%;">
                                        <div id="toolbar" style="height: 28px; padding-top: 3px;">
                                            <%--<a id="btnExportDetail" style="float: left;" class="easyui-linkbutton" plain="true"
                                                icon="icon-print">导出明细</a>--%>
                                            <span id="btnExportDetail" class="layui-btn layui-btn-small layui-btn-primary layui-btn-small">
                                                <i class="layui-icon">&#xe61e;</i> 导出明细 </span>
                                        </div>
                                        <div style="margin-top: 15px;">
                                            <table class="table">
                                                <tr class="tr_hui">
                                                    <td style="width: 100px;">
                                                        店铺数量：
                                                    </td>
                                                    <td style="width: 280px; text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labShopCount" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td style="width: 120px;">
                                                        总面积：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labArea" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <div style="margin-top: 10px;">
                                            <span id="btnNo" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe629;</i>应付</span>
                                            <table class="table">
                                                <tr class="tr_bai">
                                                    <td style="width: 100px;">
                                                        POP金额：
                                                    </td>
                                                    <td style="width: 280px; text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labPOPPrice" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td style="width: 120px;">
                                                        安装费：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labInstall" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr class="tr_bai">
                                                    <td>
                                                        快递费/运费：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labExpressPrice" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td>
                                                        测量费：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labMeasurePrice" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr class="tr_bai">
                                                    <td>
                                                        其他费用：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labOtherPrice" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                    </td>
                                                </tr>
                                                <tr class="tr_bai">
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                        总金额：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labTotalPrice" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <div style="margin-top: 20px;">
                                            <span id="Span1" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe629;</i>应收</span>
                                            <table class="table">
                                                <tr class="tr_bai">
                                                    <td style="width: 100px;">
                                                        POP金额：
                                                    </td>
                                                    <td style="width: 280px; text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labRPOPPrice" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td style="width: 120px;">
                                                        安装费：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labRInstall" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr class="tr_bai">
                                                    <td>
                                                        快递费/运费：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labRExpressPrice" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td>
                                                        测量费：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labRMeasurePrice" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr class="tr_bai">
                                                    <td>
                                                        其他费用：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labROtherPrice" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                    </td>
                                                </tr>
                                                <tr class="tr_bai">
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                        总金额：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labRTotalPrice" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="layui-tab-item">
                        <table class="table">
                            <tr class="tr_bai">
                                <td style="width: 100px;">
                                    客户
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <asp:DropDownList ID="ddlCustomer1" runat="server" Style="height: 23px;">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr class="tr_bai">
                                <td>
                                    时间
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <input type="text" id="txtBeginDate1" onclick="WdatePicker()" class="Wdate" />
                                    —
                                    <input type="text" id="txtEndDate1" onclick="WdatePicker()" class="Wdate" />
                                    <input type="button" id="btnGetProvince" value="获取" class="easyui-linkbutton" style="width: 65px;
                                        height: 26px; margin-left: 20px;" />
                                    <img id="Img1" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                                </td>
                            </tr>
                            <tr class="tr_bai">
                                <td>
                                    省份
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <img id="ImgLoadProvince1" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                                    <div id="provinceDiv1" style="width: 90%;">
                                    </div>
                                </td>
                            </tr>
                            <tr class="tr_bai">
                                <td>
                                    城市
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <img id="ImgLoadCity1" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                                    <div id="cityDiv1" style="width: 90%;">
                                    </div>
                                </td>
                            </tr>
                            <tr class="tr_bai">
                                <td>
                                    类型
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <asp:CheckBoxList ID="cblOutsourceType1" runat="server" RepeatDirection="Horizontal"
                                        RepeatLayout="Flow">
                                        <asp:ListItem Value="1">安装&nbsp;</asp:ListItem>
                                        <asp:ListItem Value="2">发货&nbsp;</asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr class="tr_bai">
                                <td>
                                </td>
                                <td style="text-align: left; padding-left: 5px; height: 35px;">
                                    <input type="button" id="btnSearch1" value="查 询" class="easyui-linkbutton" style="width: 65px;
                                        height: 26px;" />
                                    <img id="loadingImg1" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                                </td>
                            </tr>
                        </table>
                        <div>
                            <div class="layui-tab layui-tab-card">
                                <ul class="layui-tab-title">
                                    <li class="layui-this" lay-id="1">统计信息</li>
                                </ul>
                                <div id="div1" class="layui-tab-content" style="padding: 0px;">
                                    <div class="layui-tab-item layui-show" style="height: 100%;">
                                        <div id="Div2" style="height: 28px; padding-top: 3px;">
                                            <a id="btnExportDetail1" style="float: left;" class="easyui-linkbutton" plain="true"
                                                icon="icon-print">导出明细</a>
                                        </div>
                                        <div>
                                            <table class="table">
                                                <tr class="tr_hui">
                                                    <td style="width: 100px;">
                                                        店铺数量：
                                                    </td>
                                                    <td style="width: 280px; text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labShopCount1" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td style="width: 120px;">
                                                        总面积：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labArea1" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <div style="margin-top: 10px;">
                                            <span id="Span2" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe629;</i>应付</span>
                                            <table class="table">
                                                <tr class="tr_bai">
                                                    <td style="width: 100px;">
                                                        POP金额：
                                                    </td>
                                                    <td style="width: 280px; text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labPOPPrice1" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td style="width: 120px;">
                                                        安装费：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labInstall1" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr class="tr_bai">
                                                    <td>
                                                        快递费：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labExpressPrice1" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td>
                                                        测量费：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labMeasurePrice1" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr class="tr_bai">
                                                    <td>
                                                        其他费用：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labOtherPrice1" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                    </td>
                                                </tr>
                                                <tr class="tr_bai">
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                        总金额：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labTotalPrice1" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <div style="margin-top: 20px;">
                                            <span id="Span3" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe629;</i>应收</span>
                                            <table class="table">
                                                <tr class="tr_bai">
                                                    <td style="width: 100px;">
                                                        POP金额：
                                                    </td>
                                                    <td style="width: 280px; text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labRPOPPrice1" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td style="width: 120px;">
                                                        安装费：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labRInstall1" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr class="tr_bai">
                                                    <td>
                                                        快递费：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labRExpressPrice1" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td>
                                                        测量费：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labRMeasurePrice1" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr class="tr_bai">
                                                    <td>
                                                        其他费用：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labROtherPrice1" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                    </td>
                                                </tr>
                                                <tr class="tr_bai">
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                        总金额：
                                                    </td>
                                                    <td style="text-align: left; padding-left: 5px;">
                                                        <asp:Label ID="labRTotalPrice1" runat="server" Text="0"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div style="display: none;">
        <iframe id="exportFrame" name="exportFrame" src=""></iframe>
    </div>
    </form>
</body>
</html>
<script src="../../Scripts/common.js" type="text/javascript"></script>
<script src="list.js" type="text/javascript"></script>
