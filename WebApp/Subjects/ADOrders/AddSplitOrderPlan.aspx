<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddSplitOrderPlan.aspx.cs"
    Inherits="WebApp.Subjects.ADOrders.AddSplitOrderPlan" %>

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
    <style type="text/css">
        #cblRegion input
        {
            margin-right: 3px;
        }
        .style1
        {
            height: 26px;
        }
        .divMaterialList
        {
          width:200px; position:absolute;top:20px;left:5px; height:210px;border:1px solid #ccc; z-index:9999; background-color:#fff; display:none;    
        }
        #materialBigType{ list-style-type:none; margin-left:0px;}
        #materialBigType li{ margin-bottom:2px;margin-left:0px; text-decoration:underline; cursor:pointer; color:Blue;}
        .style2
        {
            width: 120px;
            height: 27px;
        }
        .style3
        {
            height: 27px;
        }
        
    </style>
    <script type="text/javascript">
        var customerId = '<%=customerId %>';
        var subjectId = '<%=subjectId %>';
    </script>
</head>
<body>
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
            <td class="style2">
                项目编号
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style3">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td class="style1">
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style1">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                所属客户
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labCustomer" runat="server" Text=""></asp:Label>
                
            </td>
        </tr>
    </table>
    <div class="tr">
        >>选择条件 
     </div>
    <table class="table" id="ConditionTB">
         <tr class="tr_bai">
            <td>
                店铺编号
            </td>
            <td style="text-align: left; padding-left: 5px;">
            <div style=" position:relative;">
                <asp:TextBox ID="txtShopNos" runat="server"  style=" width:300px;"></asp:TextBox>
                <span id="btnAddShopNos" style=" color:red; cursor:pointer; display:none; text-decoration:underline;">选择</span>
                <div id="divEditShopNos" style="display:none; position:absolute; width:1px; height:1px; border:1px solid #ccc; top:1px; left:330px; background-color:White;">
                               <div class="tr" style=" text-align:left;">>>填写店铺编号
                                  <span id="closeEditShopNos" style=" float:right; cursor:pointer;">
                                      <img src="../../image/close.gif" />
                                   </span>
                               </div>
                               <div id="DivBingPriceNameList" style=" height:201px;">
                                   <asp:TextBox ID="txtDivShopNos" runat="server" TextMode="MultiLine" style=" height:200px; width:99%;"></asp:TextBox>
                               </div>
                               <div class="tr" style=" text-align:center;">
                                   <input type="button" id="btnSubmitShopNos" value="确定" class="easyui-linkbutton" style="width: 65px;
                                       height: 26px;"/>
                               </div>
                            </div>
                (<span style="color:Blue;">可以填写多个，用英文输入逗号(,)分隔</span>)
                <input type="checkbox" id="cbExcept" style=" display:none"/>
             </div>
            </td>
        </tr>
         <tr class="tr_bai">
            <td>
                POP位置
            </td>
            <td style="text-align: left; padding-left: 5px; ">
                <div id="SheetLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
                <div id="SheetDiv" style="width: 80%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                区域
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="RegionNameDiv">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                省份
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="provinceAllDiv" style="display: none; border-bottom: 1px solid blue; width: 100px;">
                    <input type="checkbox" id="provinceCBAll" />全选 <span id="spanClareProvince" onclick="javascript:$('#ProvinceNameDiv').html('');" style=" color:Blue;cursor:pointer; margin-left:15px;">清空</span>
                </div>
                <div id="ProvinceNameDiv" style="width: 80%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                城市
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div id="cityAllDiv" style="display: none; border-bottom: 1px solid blue; width: 100px;">
                    <input type="checkbox" id="cityCBAll" />全选<span id="spanClareCity" onclick="javascript:$('#CityNameDiv').html('');" style=" color:Blue;cursor:pointer;margin-left:15px;">清空</span>
                    </div>
                <div  id="CityNameDiv" style="width: 80%;overflow:auto;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                性别
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div  id="GenderDiv" style="width: 80%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                角落类型
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div class="showLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
                <div  id="CornerTypeDiv" style="width: 80%;">
                </div>
                
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                器架类型
            </td>
            <td style="text-align: left; padding-left: 5px;">
                 <div id="Div3" style=" border-bottom: 1px solid blue; width: 50px;">
                    <span id="MFCancelSelect" style=" cursor:pointer;color:Blue;">取消选择</span>
                </div>
                <div class="showLoading" style="display:none">
                    <img src="/image/WaitImg/loadingA.gif" />
                </div>
                <div id="MachineFrameDiv" style="width: 80%;">
                </div>
            </td>
        </tr>
        
        <tr class="tr_bai">
            <td>
                城市级别
            </td>
            <td style="text-align: left; padding-left: 5px;">
                
                <div class="conditionDiv" id="CityTierDiv" style="width: 80%;">
                </div>
            </td>
        </tr>
       
        <tr class="tr_bai">
            <td>
                店铺类型
            </td>
            <td style="text-align: left; padding-left: 5px;">
                
                <div class="conditionDiv" id="FormatDiv" style="width: 80%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                物料支持类别
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div class="conditionDiv" id="MaterialSupportDiv" style="width: 80%;">
                </div>
               
            </td>
        </tr>
        
        <tr class="tr_bai">
            <td>
                店铺规模大小
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div class="conditionDiv" id="POSScaleDiv" style="width: 80%;">
                </div>
               
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                安装级别
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div class="conditionDiv" id="IsInstallDiv" style="width: 80%;">
                </div>
            </td>
        </tr>
       
       <tr class="tr_bai">
            <td>
                数量
            </td>
            <td style="text-align: left; padding-left: 5px;">
                
                <div class="conditionDiv" id="QuantityDiv" style="width: 80%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai" style="display:none;">
            <td>
                POP编号
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:TextBox ID="txtGraphicNo" runat="server" MaxLength="100" style=" width:300px;"></asp:TextBox>(<span style="color:Blue;">可以填写多个，用英文输入法分好,分隔</span>)
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                POP材质
            </td>
            <td style="text-align: left; padding-left: 5px;">
                
               <div class="conditionDiv" id="GraphicMaterialDiv" style="width: 80%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                POP尺寸<br />(Width*Length)
            </td>
            <td style="text-align: left; padding-left: 5px;"> 
                <div id="Div1" style="border-bottom: 1px solid blue; width: 50px;">
                    <input type="checkbox" id="POPSizeCBAll" />全选
                </div>
               <div class="conditionDiv" id="POPSizeDiv" style="width: 80%;">
               </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                位置尺寸<br />(Wide*Hide*Deep)
            </td>
            <td style="text-align: left; padding-left: 5px;"> 
               <div id="Div2" style=" border-bottom: 1px solid blue; width: 50px;">
                    <input type="checkbox" id="WindowSizCBAll" />全选
                </div>
               <div class="conditionDiv" id="WindowSizeDiv" style="width: 80%;">
                </div>
            </td>
        </tr>
        
        <tr class="tr_bai">
            <td>
                通电否
            </td>
            <td style="text-align: left; padding-left: 5px;"> 
               <div class="conditionDiv" id="IsElectricityDiv" style="width: 80%;">
                 
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                选图
            </td>
            <td style="text-align: left; padding-left: 5px;"> 
               <div class="conditionDiv" id="ChooseImgDiv" style="width: 80%;">
               </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                是否保留POP原尺寸
            </td>
            <td style="text-align: left; padding-left: 5px;"> 
               <input type="checkbox" name="KeepSizecb" id="cbKeepSize"/>是
            </td>
        </tr>
       
        <tr class="tr_bai">
            <td>
                橱窗
            </td>
            <td style="text-align: left; padding-left: 5px;">
               <input type="button" id="btnAddChuChuang" value="添 加"/>(左右窗贴/地铺)，说明：不需要填写尺寸，按照店铺实际尺寸生产
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
                    类型
                </td>
                <td style="width: 60px;">
                    宽
                </td>
                <td style="width: 60px;">
                    高
                </td>
                <td style="width: 200px;">
                    材质
                </td>
                <td style="width: 100px;">
                    供货方
                </td>
                <td style="width: 80;">
                    销售价
                </td>
                <td style="width: 50px;">
                    数量
                </td>
                <td style="width: 50px;">
                    性别
                </td>
                <td style="width: 60px;">
                    新选图
                </td>
                <td>
                    备注
                </td>
                <td>
                    男女共一套
                </td>
                <td>
                    单店设置
                </td>
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
                <td style="width: 60px;">
                    <input type="text" id="txtWidth" maxlength="10" style="width: 90%; text-align: center;" />
                </td>
                <td style="width: 60px;">
                    <input type="text" id="txtLength" maxlength="10" style="width: 90%; text-align: center;" />
                </td>
                <td style="width: 130px;">
                    <div style=" position:relative;">
                    <input type="hidden" name="hfMaterialId" />
                    <input type="text" name="txtMaterial" maxlength="50"  style="width: 70%; text-align: center;" /><span name="btnSelectMaterial" style="color:Blue; cursor:pointer;">选择</span>
                    <div class="divMaterialList">
                       <table style=" width:100%;">
                          <tr class="tr_hui">
                             <td style=" width:50px;">区域：</td><td style=" text-align:left; padding-left:5px;">
                                <select id="selRegion"></select>
                                
                             </td>
                             <td style="width:50px;">
                                <span name="btnSubmitMaterial" style="color:Blue; cursor:pointer; text-decoration:underline;">确定</span>
                             </td>
                          </tr>
                          <tr>
                            
                             <td colspan="3" style=" text-align:left; padding-left:15px; vertical-align:top;">
                                <span style=" font-weight:bold;">客户材质名称：</span>
                                <div class="customerMaterials"  style=" height:150px; overflow:auto;">
                                
                                </div>
                             </td>
                          </tr>
                       </table>
                    </div>
                    </div>
                </td>
                <td style="width: 100px;">
                    <input type="text" id="txtSupplier" maxlength="50" style="width: 90%; text-align: center;" />
                </td>
                <td style="width: 80px;">
                    <input type="text" id="txtRackPrice" name="txtRackPrice" maxlength="10" style="width: 90%; text-align: center;" />
                </td>
                <td style="width: 50px;">
                    <input type="text" id="txtNum" maxlength="10" value="1" style="width: 90%; text-align: center;" />
                </td>
                <td style="width: 50px;">
                    <input type="text" id="txtNewGender" maxlength="10" value="" style="width: 90%; text-align: center;" />
                </td>
                <td>
                    <input type="text" contentEditable="false" id="txtChooseImg" maxlength="50" style="width: 95%; text-align: center;" />
                </td>
                <td>
                    <input type="text" id="txtRemark" maxlength="50" style="width: 95%; text-align: center;" />
                </td>
                <td>
                    <input type="checkbox" id="cbInSet"/>
                </td>
                <td>
                    <%--<input type="checkbox" id="cbPerShop"/>--%>
                </td>
                <td style="width: 60px;">
                   
                </td>
            </tr>
        </tbody>
    </table>
    <div style=" text-align:right;">
       <input type="button" id="btnAddPlan" value="添加内容" class="easyui-linkbutton" style="width: 65px;
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
           <%-- &nbsp;&nbsp;&nbsp;&nbsp;
        <input type="button" id="btnImport" value="批量导入" class="easyui-linkbutton" style="width: 65px;
            height: 26px; display:none;" />--%>
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
    <br />
    <div style=" text-align:center;">
      <div id="showButton">
       <input type="button" id="btnCheck" value="检查遗漏" class="easyui-linkbutton" style="width: 65px;
            height: 26px;" />
            </div>
       <div id="showWaiting" style="color: Red; display: none;">
                    <img src='../../Image/WaitImg/loadingA.gif' />正在处理，请稍等...
        </div>
    </div>
    <div id="CheckDiv" title="检查拆单" style="display:none;">
       <table class="table" style=" width:200px;">
          <tr>
             <td style=" height:30px;">不符合拆单条件订单：<span id="notSplitNumber"></span></td>
          </tr>
          <tr>
             <td style=" height:30px;">
                 <asp:LinkButton ID="lbDownLoadNotSplit" runat="server" 
                     onclick="lbDownLoadNotSplit_Click">下载</asp:LinkButton>
             </td>
          </tr>
       </table>
    </div>

    <asp:HiddenField ID="hfCustomerId" runat="server" />
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


    <div id="editPerShopDiv" title="单店拆单设置" style="display:none; height:650px; overflow:auto;">
       <table class="table">
          <thead>
             <tr class="tr_bai">
                <td style=" width:40px;">序号</td>
                <td style=" width:50px;">订单类型</td>
                <td style=" width:80px;">店铺编号</td>
                <td style=" width:120px;">店铺名称</td>
                <td style=" width:80px;">POP位置</td>
                <td style=" width:60px;">性别</td>
                <td style=" width:50px;">数量</td>
                <td style=" width:80px;">材质</td>
                <td style=" width:60px;">POP宽</td>
                <td style=" width:60px;">POP高</td>
                <td style=" width:120px;">备注</td>
                
             </tr>
          </thead>
          <tbody id="TablePerShop"></tbody>
       </table>
    </div>





    </form>
</body>
</html>
<link href="/fancyBox/source/jquery.fancybox.css" rel="stylesheet" type="text/css" />
<script src="/fancyBox/source/jquery.fancybox.js" type="text/javascript"></script>
<script src="/fancyBox/source/jquery.fancybox.pack.js" type="text/javascript"></script>

<script src="../js/addSplitOrderPlan.js" type="text/javascript"></script>

