<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditOrder.aspx.cs" Inherits="WebApp.Subjects.Supplement.EditOrder" %>

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
    <style type="text/css">
    .table1
    {
        border-collapse: collapse;
        text-align: center;
        font-size: 12px;
        width:100%;
    }
    .table1 tr
    {
        height: 34px;
    }
    .table1 td
    {
        border: #dce0e9 solid 1px;
    }
    .center1
    {
        text-align: center;
    }
     .divMaterialList
      {
          width:200px; position:absolute;top:20px;left:5px; height:210px;border:1px solid #ccc; z-index:100; background-color:#fff; display:none;    
      }
    </style>
    <script type="text/javascript">
      
        var customerId = '<%=customerId %>';
        
   
        function FinishSubmit() {
            alert("提交成功");
            window.parent.ClearIframe();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <%--<div class="tr">
        >>补单项目信息
    </div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                项目编号
            </td>
            <td style="text-align: left; padding-left: 5px;">
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
    </table>--%>
    <div style="margin-top: 10px;">
        导入订单信息
        <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 120px;">
                    模板下载
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:LinkButton ID="lbDownLoad" runat="server" OnClick="lbDownLoad_Click">下载模板</asp:LinkButton>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    选择文件
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:FileUpload ID="FileUpload1" runat="server" />
                    &nbsp;
                    <%--<asp:CheckBox ID="cbAdd" runat="server"/>追加（保留原数据）--%>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                </td>
                <td style="text-align: left; padding-left: 5px; height: 30px;">
                    <div id="showButton">
                        <asp:Button ID="btnImport" runat="server" Text="导 入" class="easyui-linkbutton" OnClientClick="return checkFile()"
                            Style="width: 65px; height: 26px;" OnClick="btnImport_Click" />
                             <asp:Label ID="labState" runat="server" Text="导入成功" Visible="false" style="color:Red;margin-left:10px;"></asp:Label>
                             <asp:LinkButton ID="lbExportErrorMsg" runat="server" Visible="false" 
                            style="margin-left:10px;" onclick="lbExportErrorMsg_Click">导出失败信息</asp:LinkButton>
                    </div>
                    <div id="showWaiting" style="color: Red; display: none;">
                        <img src='/Image/WaitImg/loadingA.gif' />正在导入，请稍等...
                    </div>
                    
                   
                </td>
            </tr>
        </table>
    </div>
    <div style="margin-top: 10px;">
        手工录入订单信息
        <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
        <table class="table" id="addTable">
            <tr class="tr_hui">
                <td style="width: 100px;">
                    店铺编号<span style="color: Red;">*</span>
                </td>
                <td style="width: 60px;">
                    POP位置<span style="color: Red;">*</span>
                </td>
                <td style="width: 60px;">
                    级别
                </td>
                <td style="width: 60px;">
                    宽
                </td>
                <td style="width: 60px;">
                    高
                </td>
                <td style="width: 50px;">
                    性别<span style="color: Red;">*</span>
                </td>
                <td style="width: 40px;">
                    数量<span style="color: Red;">*</span>
                </td>
                <td style="width: 200px;">
                    材质
                </td>
                <td style="width: 60px;">
                    单价
                </td>
                <td style="width: 70px;">
                    选图
                </td>
                <td>
                    位置描述
                </td>
            </tr>
            <tr class="tr_hui">
                <td>
                    <asp:TextBox ID="txtPOSCode" runat="server" MaxLength="20" Style="width: 90%; text-align: center;"></asp:TextBox>
                </td>
                <td>
                    <asp:DropDownList ID="ddlSheet" runat="server">
                        <asp:ListItem Value="0">请选择</asp:ListItem>
                        <asp:ListItem Value="鞋墙">鞋墙</asp:ListItem>
                        <asp:ListItem Value="服装墙">服装墙</asp:ListItem>
                        <asp:ListItem Value="陈列桌">陈列桌</asp:ListItem>
                        <asp:ListItem Value="SMU">SMU</asp:ListItem>
                        <asp:ListItem Value="OOH">OOH</asp:ListItem>
                        <asp:ListItem Value="中岛">中岛</asp:ListItem>
                        <asp:ListItem Value="橱窗">橱窗</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:DropDownList ID="ddlLevelNum" runat="server">
                        <asp:ListItem Value="">无</asp:ListItem>
                        <asp:ListItem Value="1">首选</asp:ListItem>
                        <asp:ListItem Value="2">次选</asp:ListItem>
                        <asp:ListItem Value="3">三选</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:TextBox ID="txtGraphicWidth" runat="server" MaxLength="5" Style="width: 90%;
                        text-align: center;"></asp:TextBox>
                </td>
                <td>
                    <asp:TextBox ID="txtGraphicLength" runat="server" MaxLength="5" Style="width: 90%;
                        text-align: center;"></asp:TextBox>
                </td>
                <td>
                    <asp:DropDownList ID="ddlGender" runat="server">
                        <asp:ListItem Value="">请选择</asp:ListItem>
                        <asp:ListItem Value="男">男</asp:ListItem>
                        <asp:ListItem Value="女">女</asp:ListItem>
                        <asp:ListItem Value="男女不限">男女不限</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:TextBox ID="txtQuantity" runat="server" MaxLength="5" Text="1" Style="width: 90%;
                        text-align: center;"></asp:TextBox>
                </td>
                <td>
                <div style=" position:relative;">
                    <asp:HiddenField ID="hfMaterial" runat="server" />
                    <asp:TextBox ID="txtGraphicMaterial" runat="server" ReadOnly="true" MaxLength="30" Style="width: 75%;text-align: center;"></asp:TextBox>
                    <span name="btnSelectMaterial" style=" cursor:pointer;color:Blue;">选择</span>
                    <div class="divMaterialList">
                       <table style=" width:100%;">
                          <tr class="tr_hui">
                             <td style="">
                               <span style=" font-weight:bold;">客户材质名称：</span>
                             </td>
                             <td style="width:100px;">
                                <span name="btnAddMaterial" style="color:Blue; cursor:pointer; text-decoration:underline;">新增材质</span>

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
                <td>
                    <asp:TextBox ID="txtUnitPrice" runat="server" MaxLength="10" Style="width: 90%; text-align: center;"></asp:TextBox>
                </td>
                <td>
                    <asp:TextBox ID="txtChooseImg" runat="server" MaxLength="20" Style="width: 90%; text-align: center;"></asp:TextBox>
                </td>
                <td>
                    <asp:TextBox ID="txtPositionDescription" runat="server" MaxLength="20" Style="width: 90%;
                        text-align: center;"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="11" style="text-align: right; padding-right: 10px; height: 30px;">
                    <asp:Button ID="btnAddOrderDetail" runat="server" Text="添 加" class="easyui-linkbutton"
                        Style="width: 65px; height: 26px;" OnClientClick="return checkVal()" OnClick="btnAddOrderDetail_Click" />
                    <asp:Button ID="btnUpdateOrderDetail" runat="server" Text="更 新"  class="easyui-linkbutton"
                        Style="width: 65px; height: 26px; margin-left:20px;" 
                        OnClientClick="return checkVal()" onclick="btnUpdateOrderDetail_Click"/>
                </td>
            </tr>
        </table>
    </div>
    <div class="tr">
        >>订单列表
    </div>
    <%--<div class="tab-content" style="height: 200px; overflow: auto;">
       
    </div>--%>
     <div>
        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
            <ContentTemplate>
                <asp:Repeater ID="gvPOP" runat="server" onitemcommand="gvPOP_ItemCommand">
                    <HeaderTemplate>
                        <table class="table" id="gvTable">
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
                                    位置
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
                                <td>
                                    位置描述
                                </td>
                                <td>
                                    操作
                                </td>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="tr_bai">
                            <td style="width: 40px;">
                                <%#(AspNetPager1.CurrentPageIndex-1)*AspNetPager1.PageSize+ Container.ItemIndex + 1%>
                            </td>
                            <td>
                                <%#Eval("shop.ShopNo")%>
                            </td>
                            <td>
                                <%#Eval("shop.ShopName")%>
                            </td>
                            <td>
                                <%#Eval("shop.RegionName")%>
                            </td>
                            <td>
                                <%#Eval("shop.ProvinceName")%>
                            </td>
                            <td>
                                <%#Eval("shop.CityName")%>
                            </td>
                            <td>
                                <%#Eval("shop.CityTier")%>
                            </td>
                            <td>
                               <%#Eval("order.Sheet")%>
                            </td>
                            <td>
                                <%#Eval("order.Gender")%>
                            </td>
                            <td>
                                <%#Eval("order.Quantity")%>
                            </td>
                            <td>
                                <%#Eval("order.GraphicWidth")%>
                            </td>
                            <td>
                                <%#Eval("order.GraphicLength")%>
                            </td>
                            <td>
                                <%#Eval("order.Area")%>
                            </td>
                            <td>
                                <%#Eval("order.GraphicMaterial")%>
                            </td>
                            <td>
                                <%#Eval("order.UnitPrice")%>
                            </td>
                            <td>
                                <%#Eval("order.ChooseImg")%>
                            </td>
                            <td>
                                <%#Eval("order.PositionDescription")%>
                            </td>
                            <td>
                               <span name="spanEdit" data-detailid='<%#Eval("order.Id") %>' style=" color:blue;cursor:pointer;">编辑</span>
                               |
                                <asp:LinkButton ID="lbDelete" CommandArgument='<%#Eval("order.Id") %>' CommandName="deleteItem" runat="server">删除</asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <div style="text-align: center;">
                    <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="10" CssClass="paginator"
                        CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                        NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                        CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
                    </webdiyer:AspNetPager>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="AspNetPager1" EventName="pagechanged" />
                <%--<asp:AsyncPostBackTrigger ControlID="btnSreach2" EventName="click" />--%>
            </Triggers>
        </asp:UpdatePanel>
        </div>
    <br />
    <asp:Panel ID="Panel1" runat="server">
        <div style="text-align: center;">
        <asp:Button ID="btnSubmit" runat="server" Text="提 交" class="easyui-linkbutton"
                        Style="width: 65px; height: 26px;" onclick="btnSubmit_Click"/>
        </div>
    </asp:Panel>
   
     <div id="editDiv" title="添加材质" style="display: none; z-index:120;">
        <table style="width: 350px; text-align: center;">
            <tr>
                <td style="width: 100px; height: 30px;">
                    所属客户
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selCustomer">
                        <option value="0">--请选择--</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
          
             <tr>
                <td style="height: 30px;">
                    材质名称
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtMaterialNameAdd" runat="server" MaxLength="50"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    单位
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtUnit" runat="server" MaxLength="50"></asp:TextBox>
                    
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    价格
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPrice" runat="server" MaxLength="20"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            
        </table>
    </div>


    <asp:HiddenField ID="hfCurrDetailId" runat="server" />
    </form>
</body>
</html>
<script src="/Scripts/common.js" type="text/javascript"></script>
<script src="js/editorder.js" type="text/javascript"></script>

