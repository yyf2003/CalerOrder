<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddCheckOrderPlan.aspx.cs"
    Inherits="WebApp.Subjects.ADOrders.AddCheckOrderPlan" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="../../easyui1.4/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../easyui1.4/datagrid-detailview.js" type="text/javascript"></script>
    <script src="../../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        .style1
        {
            height: 31px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            添加核对订单方案
        </p>
    </div>
    <div class="tr">
        >>选择条件</div>
    <table class="table">
        <tr class="tr_bai">
            <td>
                客户
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:DropDownList ID="ddlCustomer" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                时间
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:TextBox ID="txtBeginDate" runat="server" MaxLength="15" onclick="WdatePicker()"
                    ContentEditable="false"></asp:TextBox>
                —
                <asp:TextBox ID="txtEndDate" runat="server" MaxLength="15" onclick="WdatePicker()"
                    ContentEditable="false"></asp:TextBox>
                &nbsp;
                <input type="button" id="btnSearchSubject" value="查  询" class="easyui-linkbutton"
                    style="width: 65px; height: 26px;" />
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目
            </td>
            <td style="text-align: left; padding-left: 5px; height: 80px; vertical-align: top;">
                <div id="projectsDiv" style="width: 80%; margin: 5px;">
                </div>
            </td>
        </tr>
    </table>
    <br />
    <div class="tr">
        >>方案内容</div>
    <table class="table" id="ContentTB">
        <tr class="tr_bai">
            <td style="width: 120px;">
                区域
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="RegionDiv">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                省份
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="provinceAllDiv" style="display: none; border-bottom: 1px solid blue; width: 40px;">
                    <input type="checkbox" id="provinceCBAll" />全选
                </div>
                <div id="ProvinceDiv" style="width: 80%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                城市
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="cityAllDiv" style="display: none; border-bottom: 1px solid blue; width: 40px;">
                    <input type="checkbox" id="cityCBAll" />全选</div>
                <div id="CityDiv" style="width: 80%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                城市级别
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="CityTierDiv">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                是否安装
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="InstallDiv">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                店铺类型
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="FormatDiv">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                物料支持
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="MaterialSupportDiv">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                店铺规模大小
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="ScaleDiv">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                POP位置
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="SheetDiv">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                性别
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="GenderDiv">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                选图
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="ChooseImgDiv">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <input type="button" id="btnAddContent" value="添加内容" class="easyui-linkbutton" style="width: 65px;
                    height: 26px;" />
            </td>
        </tr>
    </table>
    <br />
    <div id="planContents"  style=" display:none;">
        <span id="spanClareContent" style="color:Blue; cursor:pointer; height:30px; margin-bottom:5px;">清空内容</span>
        <table class="table" style=" min-height:100%;">
            <thead>
                <tr class="tr_hui">
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
                        是否安装
                    </td>
                    <td>
                        店铺类型
                    </td>
                    <td>
                        物料支持
                    </td>
                    <td>
                        店铺规模大小
                    </td>
                    <td>
                        POP位置
                    </td>
                    <td>
                        性别
                    </td>
                    <td>
                        选图
                    </td>
                    <td style="width: 50px;">
                        删除
                    </td>
                </tr>
            </thead>
            <tbody id="contentBody"></tbody>
        </table>
        <br />
        
    </div>
    <div style=" text-align:center;">
           <input type="button" id="btnSubmitPlan" value="新增方案" class="easyui-linkbutton" style="width: 65px;
            height: 26px;" />
        
            &nbsp;&nbsp;&nbsp;&nbsp;
        <input type="button" id="btnUpdatePlan" value="更新方案" class="easyui-linkbutton" style="width: 65px;
            height: 26px;" />
            &nbsp;&nbsp;&nbsp;&nbsp;
       <%-- <input type="button" id="btnImport" value="批量导入" class="easyui-linkbutton" style="width: 65px;
            height: 26px;" />--%>
        </div>
    <br />
    <div class="tr">
        >>方案信息</div>
    <div >
      <table id="tbPlanList" style="width: 100%;">
      </table>
    </div>
    
    <div id="toolbar">
        <a id="btnRefresh" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-reload">
            刷新</a> <a id="btnDelete" style="float: left;" class="easyui-linkbutton" plain="true"
                icon="icon-remove">删除</a>
    </div>



    <br />
    <br />
    <br />
    <br />
    <div id="showCityDiv" title="更多城市" style="height: 300px; display: none; width: 640px;">
        <div style="height: 22px; padding-top: 5px;">
            搜索：<input type="text" id="txtSearchCity" style="width: 250px;" maxlength="20" />&nbsp;
            <input type="button" id="btnSearchCity" value="查 询" class="easyui-linkbutton" style="width: 65px;
                height: 26px;" />(可以输入多个城市，英文输入法逗号分隔)
        </div>
        <input type="checkbox" id="moreCityCBAll"/>全选
        <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
        <div id="showCityListDiv" style="text-align: left; padding: 5px; height: 250px; overflow: auto;">
        </div>
    </div>
    </form>
</body>
</html>
<link href="../../fancyBox/source/jquery.fancybox.css" rel="stylesheet" type="text/css" />
<script src="../../fancyBox/source/jquery.fancybox.js" type="text/javascript"></script>
<script src="../../fancyBox/source/jquery.fancybox.pack.js" type="text/javascript"></script>
<script src="../js/addCheckOrderPlan.js" type="text/javascript"></script>
