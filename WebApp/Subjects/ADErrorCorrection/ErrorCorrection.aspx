<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorCorrection.aspx.cs"
    Inherits="WebApp.Subjects.ADErrorCorrection.ErrorCorrection" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="/easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="/easyui1.4/jquery.easyui.min.js"></script>
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
       .divMaterialList
        {
          width:220px; position:absolute;top:20px;left:5px; height:210px;border:1px solid #ccc; z-index:100; background-color:#fff; display:none;    
        }
        body{ margin-bottom:200px;}
    </style>
    <script type="text/javascript">
        var itemId = '<%=itemId %>';
        var customerId = '<%=customerId %>';
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            订单纠错
        </p>
    </div>
    <div class="tr">
        >>搜索</div>
    <div>
        <table class="table">
           
            <tr class="tr_bai">
                <td style="width: 120px;">
                    店铺编号
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtShopNo" runat="server" MaxLength="50"></asp:TextBox>
                    <asp:Label ID="labMsg" runat="server" Text="" ForeColor="Red"></asp:Label>
                    <asp:HiddenField ID="hfShopId" runat="server" />
                    <asp:Button ID="Button1" runat="server" Text="查 询" class="easyui-linkbutton" Style="width: 65px;
                        height: 26px; margin-left:30px;" OnClick="Button1_Click" />
                </td>
                
            </tr>
            <tr class="tr_bai">
                <td colspan="4" style="padding-right: 20px; text-align: right; height: 30px;">
                    
                </td>
            </tr>
        </table>
    </div>
    <br />
    <div class="tr">
        >>原订单信息
    </div>
    <input type="button" id="addEditOrder" value="纠错" class="easyui-linkbutton" style="width: 65px;height: 26px; margin-bottom:5px;"/>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div style="width: 100%; overflow: auto;">
                <asp:Repeater ID="gvPOP" runat="server" OnItemDataBound="gvPOP_ItemDataBound">
                    <HeaderTemplate>
                        <%if (gvPOP.Items.Count > 0)
                          {%>
                        <table class="table1" id="gvtable" style="width: 100%;">
                            <tr class="tr_hui">
                               
                                <td style="width: 30px;">
                                    序号
                                </td>
                                <td style="width: 50px;">
                                    订单类型
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
                                    项目名称
                                </td>
                                 <%--<td>
                                    城市
                                </td>
                               <td>
                                    城市级别
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
                                    POP编号
                                </td>--%>
                                
                                <td>
                                    位置
                                </td>
                                <td>
                                    级别
                                </td>
                                <td>
                                    位置描述
                                </td>
                                <td>
                                    器架类型
                                </td>
                                <td>
                                    性别
                                </td>
                                <td>
                                    数量
                                </td>
                                <td>
                                    POP宽(mm)
                                </td>
                                <td>
                                    POP高(mm)
                                </td>
                                <td>
                                    面积(M2)
                                </td>
                                
                                <td>
                                    POP材质
                                </td>
                                <td>
                                    单价
                                </td>
                                <td>
                                    选图
                                </td>
                               
                            </tr>
                            <%}
                          else
                          {%>
                            <table class="table1" style="width: 100%;">
                                <%} %>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="tr_bai">
                            
                            <td style="width: 30px;">
                                <%#Container.ItemIndex + 1%>
                            </td>
                            <td style="width: 50px;">
                                <%#Eval("order.OrderType") != null ? Eval("order.OrderType").ToString() == "1" ? "pop" : "道具" : "POP"%>
                            </td>
                            <td>
                                <%--店铺编号--%>
                                <%#Eval("order.ShopNo")%>
                            </td>
                            <td>
                                <%--店铺名称--%>
                                <%#Eval("order.ShopName")%>
                            </td>
                            <td>
                                <%--区域--%>
                                <%#Eval("order.Region")%>
                            </td>
                            <td>
                                <%--省份--%>
                                <%#Eval("order.Province")%>
                            </td>
                            <td>
                                <%--项目名称--%>
                                <%#Eval("subject.SubjectName")%>
                            </td>
                            
                            
                            <td>
                                <%--位置--%>
                                <%#Eval("order.Sheet")%>
                            </td>
                            <td>
                                <%--级别--%>
                                <%#Eval("LevelNum")%>
                            </td>
                            <td>
                                <%--位置描述--%>
                                <%--<%#Eval("pop") != null ? Eval("pop.PositionDescription") : ""%>--%>
                                <%#Eval("order.PositionDescription")%>
                            </td>
                            <td>
                                <%--器架类型--%>
                                <%#Eval("order.MachineFrame")%>
                            </td>
                            <td>
                                <%--性别--%>
                                <%#Eval("order.Gender")%>
                            </td>
                            <td>
                                <%--数量--%>
                                <%#Eval("order.Quantity")%>
                            </td>
                            <td>
                                <%--POP宽(mm)--%>
                                <%#Eval("order.GraphicWidth")%>
                            </td>
                            <td>
                                <%-- POP高(mm)--%>
                                <%#Eval("order.GraphicLength")%>
                            </td>
                            <td>
                                <%--面积(M2)--%>
                                <%#Eval("order.Area")%>
                            </td>
                            
                            <td>
                                <%--POP材质--%>
                                <%#Eval("order.GraphicMaterial")%>
                            </td>
                            <td>
                                <%--单价--%>
                                <%#Eval("order.UnitPrice")%>
                            </td>
                            <td>
                                <%--选图--%>
                                <%#Eval("order.ChooseImg")%>
                            </td>
                           
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <%if (gvPOP.Items.Count == 0)
                          { %>
                        <tr class="tr_bai">
                            <td style="text-align: center;">
                                --暂无数据--
                            </td>
                        </tr>
                        <%} %>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <br />
            <div style="text-align: center;">
                <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="10" CssClass="paginator"
                    CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                    NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                    CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
                </webdiyer:AspNetPager>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="AspNetPager1" EventName="PageChanged" />
        </Triggers>
    </asp:UpdatePanel>
    
    <div id="editDiv" style=" display:none;">
        <div class="tr">
        >>订单纠错  &nbsp;&nbsp;<span id="btnClearData" style="color:Blue;cursor:pointer;">清除数据</span>
        </div>
        <table id="editTable" class="table" style=" margin-top:-5px;">
          <thead>
             <tr class="tr_hui">
                <td style="width:3%">序号</td>
                <%--<td style="width:8%">店铺编号</td>--%>
                <td style="width:15%">项目名称</td>
                <td style="width:3%">类型</td>
                <td style="width:4%">位置</td>
                <td style="width:4%">级别</td>
                <td style="width:3%">宽</td>
                <td style="width:3%">高</td>
                <td style="width:3%">性别</td>
                <td style="width:15%">材质</td>
                <td style="width:3%">单价</td>
                <td style="width:3%">数量</td>
                <td style="width:9%">选图</td>
                <td style="width:9%">备注/POP描述</td>
                <td style="width:4%">状态</td>
                <td>纠错原因</td>
                <td style="width:2%">操作</td>
                
             </tr>
          </thead>
          <tbody id="orderContent">
          </tbody>
          <tfoot id="addContent">
            <tr class="tr_hui">
                <td style=" color:blue; font-weight:bold;">新增</td>
                <%--<td>
                   <input id="txtaddShopNo" maxlength="15" style=" width:90%; text-align:center;"/>
                   <input type="hidden" id="hfaddShopId" />
                </td>--%>
                <td>
                  <%--<input id="txtaddSubjectName" maxlength="50" style=" width:90%; text-align:center;"/>--%>
                  <select id="seleaddSubjectName">
                     <option value="0">请选择</option>
                  </select>
                </td>
                <td>
                  <select id="seleAddOrderType">
                     <option value="POP">POP</option>
                     <option value="道具">道具</option>
                  </select>
                </td>
                <td>
                    <select id="seleaddSheet">
                     <option value="鞋墙">鞋墙</option>
                     <option value="服装墙">服装墙</option>
                     <option value="陈列桌">陈列桌</option>
                     <option value="橱窗">橱窗</option>
                     <option value="中岛">中岛</option>
                     <option value="户外">户外</option>
                  </select>
                </td>
                <td>
                  <input id="txtaddLevelNum" maxlength="3" style=" width:90%; text-align:center;"/>
                </td>
                <td>
                  <input id="txtaddWidth" maxlength="6" style=" width:90%; text-align:center;"/>
                </td>
                <td>
                   <input id="txtaddLength" maxlength="6" style=" width:90%; text-align:center;"/>
                </td>
                <td>
                   <select id="seleaddGender">
                     <option value="男">男</option>
                     <option value="女">女</option>
                     <option value="男女不限">男女不限</option>
                  </select>
                </td>
                <td>
                   <div style=" position:relative;">
                    <input type="hidden" name="hfMaterialId" />
                    <input type="text" id="txtaddMaterial" maxlength="50" readonly="readonly"  style="width: 99%; text-align: center;" />
                    <%--<span name="btnSelectMaterial" style="color:Blue; cursor:pointer;">选择</span>--%>
                    <div class="divMaterialList">
                       <table style=" width:100%;">
                          <tr class="tr_hui">
                             <td style="">
                               <span style=" font-weight:bold;">客户材质名称：</span>
                             </td>
                             <td style="width:100px;">
                                <span name="btnSubmitMaterial" style="color:Blue; cursor:pointer; text-decoration:underline;">确定</span>
                                &nbsp;
                                <span id="spanCloseMaterial" style="color:Blue; cursor:pointer;">关闭</span>
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
                <td>
                  <input id="txtaddPrice" maxlength="15" style=" width:90%; text-align:center;"/>
                </td>
                <td>
                  <input id="txtaddNum" value="1" maxlength="3" style=" width:90%; text-align:center;"/>
                </td>
                <td>
                  <input id="txtaddChooseImg" maxlength="40" style=" width:90%; text-align:center;"/>
                </td>
                <td>
                   <input id="txtaddRemark" maxlength="50" style=" width:90%; text-align:center;"/>
                </td>
                <td>
                </td>
                <td>
                 <input id="txtaddEditRemark" maxlength="50" style=" width:90%; text-align:center;"/>
                </td>
                <td>
                    <input type="button" id="btnAddContent" value="添加" class="easyui-linkbutton" style="width: 65px;height: 26px;"/>
                </td>
                
             </tr>
          </tfoot>
        </table>
        <div style=" text-align:center; margin-top:20px; margin-bottom:20px;">
           <input type="button" id="btnSubmit" value="提交修改" class="easyui-linkbutton" style="width: 80px;height: 26px;"/>
        </div>
        <br />
       </div>
         
   
    </form>
</body>
</html>
<script src="../../Scripts/common.js" type="text/javascript"></script>
<script src="js/errorcorrection.js" type="text/javascript"></script>
<script type="text/javascript">
    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (sender, e) {
//        $("#gvtable").find("input[name$='cbAll']").on("change", function () {
//            var checked = this.checked;
//            $("#gvtable").find("input[name$='cbOrder']").each(function () {
//                this.checked = checked;
//            })
//        })
    })
</script>