<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SplitPlan.aspx.cs" Inherits="WebApp.Subjects.ADOrders.SplitPlan" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="/easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="/easyui1.4/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="/easyui1.4/datagrid-detailview.js" type="text/javascript"></script>
     <script type="text/javascript">
         var customerId = '<%=customerId %>';
         var subjectId = '<%=subjectId %>';
    </script>
    <style type="text/css">
       .divMaterialList
        {
          width:250px; position:absolute;top:25px;left:30px; height:215px;border:1px solid #ccc; z-index:100; background-color:#fff; display:none;    
        }
        #materialBigType{ list-style-type:none; margin-left:0px;}
        #materialBigType li{ margin-bottom:2px;margin-left:0px; text-decoration:underline; cursor:pointer; color:Blue;}
    </style>
</head>
<body style=" margin-bottom :50px;">
    <form id="form1" runat="server">
    
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            添加拆分订单方案
        </p>
    </div>
    <div class="tr">
        >>项目信息</div>
    <table class="table">
        <tr class="tr_bai">
            <td style=" width:120px;">
                项目编号：
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style3">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>

            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目名称：
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style1">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                所属客户：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labCustomer" runat="server" Text=""></asp:Label>
                
            </td>
        </tr>
    </table>

    <div class="tr" style=" margin-top :10px;">
        >>拆分条件 
     </div>
     <table class="table" id="ConditionTB">
         <tr class="tr_bai">
            <td>
                店铺编号
            </td>
            <td style="text-align: left; padding-left: 5px;">
              <asp:TextBox ID="txtShopNos" runat="server" MaxLength="100" style=" width:300px;"></asp:TextBox>
              <img id="ShopNosLoading" style=" display:none;" src="/image/WaitImg/loadingA.gif" />
              <span id="shopMsg" style="color:Red;"></span>
            </td>
        </tr>
         <tr class="tr_bai">
            <td>
                POP位置
            </td>
            <td style="text-align: left; padding-left: 5px; ">
                <div id="SheetDiv" style="width: 80%;">
                </div>
                <div id="SheetLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                区域
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="RegionDiv" style="width: 80%;">
                </div>
                <div id="RegionLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                省份
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="ProvinceDiv" style="width: 80%;">
                </div>
                <div id="ProvinceLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                城市
            </td>
            <td style="text-align: left; padding:0px;">
               <div id="CityDiv" style="width: 80%;">
                </div>
                <div id="CityLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                性别
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="GenderDiv" style="width: 80%;">
                </div>
                <div id="GenderLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                角落类型
            </td>
            <td style="text-align: left; padding-left: 5px;">
               
                <div id="CornerTypeDiv" style="width: 80%;">
                </div>
                 <div id="CornerTypeLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                器架类型
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="SelectAllMachineFrame" style=" display:none;">
                   <input type="checkbox" id="cbSelectAllMachineFrame"/><span style="color:blue;">全选</span>
                </div>
                <div id="MachineFrameDiv" style="width: 80%;">
                </div>
                <div id="MachineFrameLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
            </td>
        </tr>
       
        <tr class="tr_bai">
            <td>
                城市级别
            </td>
            <td style="text-align: left; padding-left: 5px;">
                
                <div id="CityTierDiv" style="width: 80%;">
                </div>
                <div id="CityTierLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
            </td>
        </tr>
       
        <tr class="tr_bai">
            <td>
                店铺类型
            </td>
            <td style="text-align: left; padding-left: 5px;">
                
                <div id="FormatDiv" style="width: 80%;">
                </div>
                <div id="FormatLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
            </td>
        </tr>
        
        <tr class="tr_bai">
            <td>
                物料支持级别
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="MaterialSupportDiv" style="width: 80%;">
                </div>
                <div id="MaterialSupportLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
            </td>
        </tr>
        
        <tr class="tr_bai">
            <td>
                店铺规模大小
            </td>
            <td style="text-align: left; padding-left: 5px;">
               
                <div id="POSScaleDiv" style="width: 80%;">
                </div>
                <div id="POSScaleLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                安装级别
            </td>
            <td style="text-align: left; padding-left: 5px;">
               
                <div id="IsInstallDiv" style="width: 80%;">
                </div>
                <div id="IsInstallLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
            </td>
        </tr>
       
       <tr class="tr_bai">
            <td>
                数量
            </td>
            <td style="text-align: left; padding-left: 5px;">
                
                
                <div id="QuantityDiv" style="width: 80%;">
                </div>
                <div id="QuantityLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
            </td>
        
        </tr>
        <tr class="tr_bai">
            <td>
                POP尺寸<br />(Width*Length)
            </td>
            <td style="text-align: left; padding-left: 5px;"> 
                <div id="SelectAllPOPSize" style=" display:none;">
                   <input type="checkbox" id="cbSelectAllPOPSize"/><span style="color:blue;">全选</span>
                </div>
                <div id="POPSizeDiv" style="width: 80%;">
                </div>
                <div id="POPSizeLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
            </td>
        </tr>
        
        <tr class="tr_bai">
            <td>
                通电否
            </td>
            <td style="text-align: left; padding-left: 5px;"> 
                <div id="IsElectricityDiv" style="width: 80%;">
                </div>
                <div id="IsElectricityLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                选图
            </td>
            <td style="text-align: left; padding-left: 5px;"> 
               
               <div id="ChooseImgLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
                <div id="ChooseImgDiv" style="width: 80%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                是否保留POP原尺寸
            </td>
            <td style="text-align: left; padding-left: 5px;"> 
               <input type="checkbox" name="KeepSizecb" id="cbKeepSize"/>是(使用数据库尺寸)
            </td>
        </tr>
       <tr class="tr_bai">
            <td>
                主KV延续(去掉主KV)
            </td>
            <td style="text-align: left; padding-left: 5px;"> 
               <input type="checkbox" name="NoMainKVcb" id="cbNoMainKV"/>主KV延续
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                橱窗
            </td>
            <td style="text-align: left; padding-left: 5px;">
               <input type="button" id="btnAddChuChuang" value="添 加" class="easyui-linkbutton" style="width: 65px;
                        height: 26px;"/>(左右窗贴/地铺/窗贴)，说明：不需要填写尺寸，按照店铺实际尺寸
            </td>
        </tr>
      
        <tr class="tr_bai">
          <td>
             添加陈列桌尺寸
          </td>
          <td style="text-align: left; padding-left: 5px;">
             <input type="button" id="btnAddNormalSize" value="正常尺寸" class="easyui-linkbutton" style="width: 65px; height: 26px;"/>
             <input type="button" id="btnAddWithEdgelSize" value="反包尺寸" class="easyui-linkbutton" style="width: 65px; height: 26px; margin-left:15px;"/>
          </td>
        </tr>
        <tr class="tr_bai">
            <td>
                不参与方案店铺
            </td>
            <td style="text-align: left; padding-left: 5px;">
              <asp:TextBox ID="txtNoInvolveShopNos" runat="server" MaxLength="100"  style=" width:300px;"></asp:TextBox>
              (<span style="color:Blue;">可以填写多个，用英文输入逗号(,)分隔</span>)
              <span id="NoInvolveShopNosMsg" style="color:red;"></span>
            </td>
        </tr>
    </table>
    <br />

    <div class="tr">
        >>方案内容  &nbsp;&nbsp;&nbsp;<span id="spanClareDate" style=" color:Blue; cursor:pointer;">清除数据</span>
    </div>
    <table class="table">
        <thead>
            <tr class="tr_hui">
                <td style="width: 60px;">
                    类型<span style=" color:Red;">*</span>
                </td>
                <td style="width: 90px;">
                    宽
                </td>
                <td style="width: 90px;">
                    高
                </td>
                <td style="width: 300px;">
                    材质
                </td>
                <%--<td style="width: 100px;">
                    供货方
                </td>--%>
                <td style="width: 80px;">
                    销售价
                </td>
                <td style="width: 50px;">
                    数量<span style=" color:Red;">*</span>
                </td>
                <%--<td style="width: 50px;">
                    性别
                </td>--%>
                <td style="width: 150px;">
                    新选图
                </td>
                <td>
                    备注<span style=" color:Red;">*</span>
                </td>
                <%--<td>
                    男女共一套
                </td>--%>
                <td style="width: 60px;">
                    操作
                </td>
            </tr>
        </thead>
        <tbody id="addContent">
        </tbody>
        <tbody id="container">
            <tr class="tr_bai">
                <td style="width: 60px;">
                    <select id="selPlanType" style="width: 50px;">
                        <option value="1">pop</option>
                        <option value="2">道具</option>
                    </select>
                </td>
                <td style="width: 90px;">
                    <input type="text" id="txtWidth" maxlength="10" style="width: 90%; text-align: center;" />
                </td>
                <td style="width: 90px;">
                    <input type="text" id="txtLength" maxlength="10" style="width: 90%; text-align: center;" />
                </td>
                <td style="width: 300px;">
                    <div style=" position:relative;">
                    <input type="text" id="txtMaterial" maxlength="50" readonly="readonly"  style="width: 70%; text-align: center;" /><span name="btnSelectMaterial" style="color:Blue; cursor:pointer;">选择</span>
                    <div class="divMaterialList">
                       <table style=" width:100%;">
                          <tr class="tr_hui">
                             <td style="">
                               <span style=" font-weight:bold;">客户材质名称：</span>
                             </td>
                             <td style="width:100px;">
                               
                                <span name="btnSubmitMaterial" style="color:Blue; cursor:pointer; text-decoration:underline;">确定</span>
                             </td>
                          </tr>
                          <tr>
                            
                             <td colspan="2" style=" text-align:left; padding-left:15px; vertical-align:top;">
                                
                                <div class="customerMaterials"  style=" height:175px; overflow:auto;">
                                
                                </div>
                             </td>
                          </tr>
                       </table>
                    </div>
                    </div>
                </td>
                <%--<td style="width: 100px;">
                    <input type="text" id="txtSupplier" maxlength="50" style="width: 90%; text-align: center;" />
                </td>--%>
                <td style="width: 80px;">
                    <input type="text" id="txtRackPrice" readonly="readonly"  maxlength="10" style="width: 90%; text-align: center;" />
                </td>
                <td style="width: 50px;">
                    <input type="text" id="txtNum" maxlength="10" value="1" style="width: 90%; text-align: center;" />
                </td>
                <%--<td style="width: 50px;">
                    <input type="text" id="txtNewGender" maxlength="10" value="" style="width: 90%; text-align: center;" />
                </td>--%>
                <td>
                    <input type="text" contentEditable="false" id="txtChooseImg" maxlength="50" style="width: 95%; text-align: center;" />
                </td>
                <td>
                    <input type="text" id="txtRemark" maxlength="50" style="width: 95%; text-align: center;" />
                </td>
                <%--<td>
                    <input type="checkbox" id="cbInSet"/>
                </td>--%>
                
                <td style="width: 60px;">
                   
                </td>
            </tr>
        </tbody>
    </table>
    <div style=" text-align:right;">
       <input type="button" id="btnAddPlanContent" value="添加内容" class="easyui-linkbutton" style="width: 65px;
                        height: 26px;" />
    </div>
    <div style=" display:none; text-align:center;">
        <img src="/image/WaitImg/loading1.gif" />
    </div>
    <div style="text-align: center;">
       
        <input type="button" id="btnSubmitPlan" value="新增方案" class="easyui-linkbutton" style="width: 65px;
            height: 26px;" />
        &nbsp;&nbsp;&nbsp;&nbsp;
        <input type="button" id="btnUpdatePlan" value="更新方案" class="easyui-linkbutton" style="width: 65px;
            height: 26px;" />
            
    </div>
    <br />
    <div class="tr">
        >>方案信息</div>
    <table id="tbPlanList" style="width: 100%;">
    </table>
    <div id="toolbar">
        <a id="btnRefresh" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-reload">
            刷新</a> <a id="btnDelete" style="float: left;" class="easyui-linkbutton" plain="true"
                icon="icon-remove">删除</a>
    </div>
    <asp:HiddenField ID="hfSubjectCategoryName" runat="server" />
    </form>
</body>
</html>

<script src="../js/splitPlan.js" type="text/javascript"></script>
