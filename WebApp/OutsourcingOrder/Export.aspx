<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Export.aspx.cs" Inherits="WebApp.OutsourcingOrder.Export" %>

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
            外协订单导出
        </p>
    </div>
     <div class="tr">
        >>选择项目</div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                客户
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:DropDownList ID="ddlCustomer" runat="server">
                </asp:DropDownList>
                <span style="color:Red;">*</span>
            </td>
        </tr>
        
        <tr class="tr_bai">
            <td>
                活动月份
            </td>
            <td style="text-align: left;padding-left: 5px;">
               <input type="text" id="txtMonth" value=""  class="Wdate"  onclick="WdatePicker({skin:'whyGreen',dateFmt:'yyyy-MM',isShowClear:false,readOnly:true})" style=" width:80px;"/>
               <span id="spanUp" style="margin-left:10px;cursor:pointer; color:Blue;">上一个月</span>
               <span id="spanDown" style=" margin-left:20px; cursor:pointer; color:Blue;">下一个月</span>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                活动名称
            </td>
            <td style="padding-left: 5px;text-align: left; ">
               <div id="guidanceDiv">
               </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                区域
            </td>
            <td style="padding-left: 5px;text-align: left; ">
               <div id="RegionDiv">
               </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                外协名称
            </td>
            <td style="padding-left: 5px;text-align: left; ">
                <asp:DropDownList ID="ddlOutsource" runat="server">
                   <asp:ListItem Value="0">--请选择--</asp:ListItem>
                </asp:DropDownList>
                <asp:RadioButtonList ID="rblOrderType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" style=" margin-left:20px;">
                   <asp:ListItem Value="1">安装&nbsp;&nbsp;</asp:ListItem>
                   <asp:ListItem Value="2">发货 </asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                
            </td>
            <td style="padding-left: 5px;text-align: left; height:35px;">
               <input type="button" value="查 询" id="btnSearch" class="easyui-linkbutton" style="width: 65px;
                    height: 26px;" />
                <img id="imgSearchLoading" style="display:none;" src='../image/WaitImg/loadingA.gif' />
            </td>
        </tr>
    </table>
   
   <div class="tr" style=" margin-top:20px;">
        >>订单导出</div>
   <table class="table">
      <tr class="tr_hui">
       <td style=" width:120px; font-weight:bold;">
         材质类型
       </td>
       <td colspan="3" style=" text-align:left; padding-left:5px;">
          
           
       </td>
      </tr>
      <tr class="tr_bai">
         <td style=" font-weight:bold;">
          订单导出
         </td>
         <td style=" text-align:left; padding-left:5px; width:200px;">
                
                <input type="button"  value="系统订单" id="btnExportNew350" class="easyui-linkbutton"
                    style="width: 100px; height: 26px; margin-left:10px;" />
                <img id="downloading2" src="../image/WaitImg/loadingA.gif" style=" display:none;"/>

                
                
         </td>
         <td style=" width:120px;font-weight:bold;">
            导出喷绘王模板
         </td>
         <td style=" text-align:left; padding-left:5px;">
             <input type="button" value="北京模板" id="btnPHWbj" class="easyui-linkbutton" style="width: 65px;
                    height: 26px; margin-left:10px;" />
                <img id="downloading3" src="../image/WaitImg/loadingA.gif" style=" display:none;"/>
                
                <input type="button"  value="外协模板" id="btnPHWwx" class="easyui-linkbutton"
                    style="width: 80px; height: 26px; margin-left:10px;" />
                <img id="downloading4" src="../image/WaitImg/loadingA.gif" style=" display:none;"/>
         </td>
      </tr>
    </table>

    <div class="tr" style=" margin-top:20px;">
        >>店铺明细</div>
    <div style="overflow: auto;">
        <table id="noData" class="table" style="display:;">
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
                    <td style="width: 40px;">
                        序号
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
                        店铺类型
                    </td>
                    <td>
                        店铺地址
                    </td>
                </tr>
            </thead>
            <tbody id="listBody">
            </tbody>
        </table>
        
    </div>
    <div id="Pagination" class="sabrosus" style="display: none;">
    </div>
   
    <br />
    <div style="display: none;">
        <iframe id="exportFrame" name="exportFrame" src=""></iframe>
    </div>
   
    </form>
</body>
</html>
<script src="js/export.js" type="text/javascript"></script>
