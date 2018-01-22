<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="POPList.aspx.cs" Inherits="WebApp.Customer.POPList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../fancyBox/source/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../easyui1.4/jquery.min.js"></script>
    <script src="../easyui1.4/jquery.easyui.min.js"></script>
    <style type="text/css">
       #editDiv li
       {
         margin-bottom:2px;
         height:20px;
         font-size:14px;
         cursor:pointer;    
         padding-left:5px;
         color:Blue;
       }
       #editDiv li:hover{background-color:#f0f1f2;text-decoration:underline;}
     </style>
      
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            POP基础数据
        </p>
    </div>
    <div class="tr">
        >>搜索
    </div>
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
          <ContentTemplate>
          
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 150px;">
                    客户名称
                </td>
                <td style="width: 200px; text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
                <td style="width: 150px;">
                </td>
                <td style="text-align: left; padding-left: 5px;">
                </td>
            </tr>
           
            <tr class="tr_bai">
                <td>
                    区域
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <%--<asp:DropDownList ID="ddlRegion" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlRegion_SelectedIndexChanged">
                        <asp:ListItem Value="0">--请选择--</asp:ListItem>
                    </asp:DropDownList>--%>
                    <div id="loadRegion" style="display: none;">
                       <img src="../image/WaitImg/loadingA.gif" />
                    </div>
                    <asp:CheckBoxList ID="cblRegion" runat="server" CssClass="cbl" CellSpacing="20" RepeatDirection="Horizontal"
                                RepeatLayout="Flow" RepeatColumns="5" OnSelectedIndexChanged="cblRegion_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:CheckBoxList>
                </td>
                
            </tr>
             <tr class="tr_bai">
                <td>
                    省份
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <div id="loadProvince" style="display: none;">
                           <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblProvince" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="8" OnSelectedIndexChanged="cblProvince_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:CheckBoxList>
                </td>
                
            </tr>
            <tr class="tr_bai">
                <td>
                    Channel
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <div id="loadChannel" style="display: none;">
                       <img src="../image/WaitImg/loadingA.gif" />
                    </div>
                    <asp:CheckBoxList ID="cblChannel" runat="server" RepeatDirection="Horizontal" AutoPostBack="true"
                        RepeatLayout="Flow" onselectedindexchanged="cblChannel_SelectedIndexChanged">
                    </asp:CheckBoxList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    Format
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <div id="loadFormat" style="display: none;">
                       <img src="../image/WaitImg/loadingA.gif" />
                    </div>
                    <asp:CheckBoxList ID="cblFormat" runat="server" RepeatDirection="Horizontal" RepeatColumns="10" AutoPostBack="true"
                        RepeatLayout="Flow" onselectedindexchanged="cblFormat_SelectedIndexChanged">
                    </asp:CheckBoxList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    店铺级别
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <div id="loadShopLevel" style="display: none;">
                       <img src="../image/WaitImg/loadingA.gif" />
                    </div>
                    <asp:CheckBoxList ID="cblShopLevel" runat="server" RepeatDirection="Horizontal" AutoPostBack="true"
                        RepeatLayout="Flow" onselectedindexchanged="cblShopLevel_SelectedIndexChanged">
                    </asp:CheckBoxList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    位置
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <div id="loadSheet" style="display: none;">
                       <img src="../image/WaitImg/loadingA.gif" />
                    </div>
                    <asp:CheckBoxList ID="cblSheet" runat="server" RepeatDirection="Horizontal" AutoPostBack="true"
                        RepeatLayout="Flow" onselectedindexchanged="cblSheet_SelectedIndexChanged">
                    </asp:CheckBoxList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    性别
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                     <div id="loadGender" style="display: none;">
                       <img src="../image/WaitImg/loadingA.gif" />
                    </div>
                     <asp:CheckBoxList ID="cblGender" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                     </asp:CheckBoxList>
                </td>
               
            </tr>
            
             <tr class="tr_bai">
                <td>
                    店铺编号
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtShopNo" runat="server"></asp:TextBox>
                </td>
                <td>
                    店铺名称
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtShopName" runat="server"></asp:TextBox>
                </td>
            </tr>
            
            <tr class="tr_bai">
                <td colspan="4" style="padding-right: 10px; text-align: right; height: 30px;">
                   
                </td>
            </tr>
        </table>
        </ContentTemplate>
        </asp:UpdatePanel>
        <div style="text-align: right; padding-right: 25px; margin-top: 8px;">
            <asp:Button ID="btnSearch" runat="server" Text="查 询" OnClick="btnSearch_Click" OnClientClick="return loadSearch()" class="easyui-linkbutton"
                        Style="width: 65px; height: 26px;"/>
                        <img id="loadingSearch" style="display: none;" src="../image/WaitImg/loadingA.gif" />
                        &nbsp;&nbsp;&nbsp;
                    <%--<asp:Button ID="btnExport" runat="server" Text="导 出"  class="easyui-linkbutton"
                        Style="width: 65px; height: 26px;" onclick="btnExportShopAndPOP_Click" OnClientClick="return loading()"/>
                        <img id="loadingImg" style="display: none;" src="../image/WaitImg/loadingA.gif" />--%>
        </div>
    </div>
    <div>
        <br />
        <div class="tr">
            >>信息列表
        </div>
        <div class="containerDiv">
            <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" CellPadding="0"
                CssClass="table" BorderWidth="0" HeaderStyle-BorderStyle="None" EmptyDataText="--无信息--"
                Style="width: 1800px;" onrowcommand="gv_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="序号" HeaderStyle-Width="40px" HeaderStyle-BorderColor="#dce0e9">
                        <ItemTemplate>
                            <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CustomerName" HeaderText="所属客户" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="shop.ShopNo" HeaderText="店铺编码" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="shop.ShopName" HeaderText="店铺名称" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="shop.RegionName" HeaderText="区域" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="shop.ProvinceName" HeaderText="省份" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="shop.CityName" HeaderText="城市" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="shop.CityTier" HeaderText="城市级别" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="shop.Channel" HeaderText="Channel" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="shop.Format" HeaderText="Format" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="shop.ShopLevel" HeaderText="店铺级别" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.Sheet" HeaderText="位置" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.GraphicNo" HeaderText="POP编号" HeaderStyle-BorderColor="#dce0e9" />


                    <asp:BoundField DataField="pop.OOHInstallPrice" HeaderText="户外安装费" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.CornerType" HeaderText="角落类型" HeaderStyle-BorderColor="#dce0e9" />
                    <%-- <asp:BoundField DataField="pop.POPType" HeaderText="POP类型" HeaderStyle-BorderColor="#dce0e9" />
                    --%>
                    <asp:BoundField DataField="pop.PositionDescription" HeaderText="位置描述" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.Gender" HeaderText="性别" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.Quantity" HeaderText="数量" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.GraphicWidth" HeaderText="POP宽mm" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.GraphicLength" HeaderText="POP高mm" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.Area" HeaderText="面积" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.WindowWide" HeaderText="位置宽mm" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.WindowHigh" HeaderText="位置高mm" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.WindowDeep" HeaderText="位置深mm" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.WindowSize" HeaderText="位置规模" HeaderStyle-BorderColor="#dce0e9" />
                    <%--<asp:BoundField DataField="pop.DoubleFace" HeaderText="单/双面" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.Glass" HeaderText="是否有玻璃" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.Backdrop" HeaderText="背景" HeaderStyle-BorderColor="#dce0e9" />--%>
                    <asp:BoundField DataField="pop.GraphicMaterial" HeaderText="POP材质" HeaderStyle-BorderColor="#dce0e9" />
                    <%--<asp:BoundField DataField="pop.FixtureType" HeaderText="设备类别" HeaderStyle-BorderColor="#dce0e9" />--%>
                    <%--<asp:BoundField DataField="pop.IsElectricity" HeaderText="通电否" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.LeftSideStick" HeaderText="左侧贴" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.RightSideStick" HeaderText="右侧贴" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.Floor" HeaderText="地铺" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.WindowStick" HeaderText="窗贴" HeaderStyle-BorderColor="#dce0e9" />--%>
                    <%--<asp:BoundField DataField="pop.IsHang" HeaderText="悬挂否" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="pop.DoorPosition" HeaderText="门位置" HeaderStyle-BorderColor="#dce0e9" />--%>
                    <%--<asp:BoundField DataField="pop.Remark" HeaderText="备注" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:TemplateField HeaderText="编辑" Visible="false" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                        <ItemTemplate>
                            <span name="editPOP" data-popid='<%#Eval("pop.Id") %>' style="color: Blue; cursor: pointer;">
                                编辑</span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="操作" Visible="false" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                        <ItemTemplate>
                            <asp:LinkButton ID="lbDelete" runat="server" CommandArgument='<%#Eval("pop.Id") %>' CommandName="DeleteItem" OnClientClick="return confirm('确定删除吗？')">删除</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                </Columns>
                <AlternatingRowStyle CssClass="tr_bai" />
                <HeaderStyle CssClass="tr_hui" />
                <RowStyle CssClass="tr_bai" />
                <SelectedRowStyle CssClass="tr_hui" />
                <EmptyDataRowStyle CssClass="tr_bai" />
            </asp:GridView>
            <br />
        </div>
    </div>
    <br />
    <div style="text-align: center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    <div id="editDiv" title="编辑POP信息" style="display: none;">
        <table class="table" style="width: 700px; text-align: center;">
            <tr>
                <td style="width: 100px; height: 30px;">
                    店铺编号
                </td>
                <td style="text-align: left; width: 230px; padding-left: 5px;">
                    <asp:TextBox ID="txteditShopNo" runat="server" ReadOnly="true" MaxLength="20"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
                <td style="width: 100px; height: 30px;">
                    POP位置
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   <div style=" position:relative;">
                    <input type="text" id="txtSheet" style="width: 120px;" />
                    <div id="divSheetMenu" style="display: none; position: absolute;width: 125px; background-color:White; border:1px solid #ccc; padding-top:2px; z-index:100;">
                        <ul id="ddlSheetMenu"  style="margin-top: 0; width: 125px; margin-left:0px; list-style:none;">
                        </ul>
                    </div>
                    <span style="color: Red;">*</span>
                    </div>

                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    位置描述
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPositionDescription" runat="server" MaxLength="50"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    POP编号
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtGraphicNo" runat="server" MaxLength="20"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    性别
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <div style=" position:relative;">
                    <input type="text" id="txtGender" style="width: 120px;" />
                    <div id="divGenderMenu" style="display: none; position: absolute;width: 125px; background-color:White; border:1px solid #ccc; padding-top:2px; z-index:100;">
                        <ul id="ddlGenderMenu"  style="margin-top: 0; width: 125px; margin-left:0px; list-style:none;">
                           <li>男</li>
                           <li>女</li>
                           <li>男女不限</li>
                           <li>无</li>
                        </ul>
                    </div>
                    <span style="color: Red;">*</span>
                    </div>
                </td>
                <td style="height: 30px;">
                    数量
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtQuantity" runat="server" MaxLength="50"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    POP宽
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtGraphicWidth" runat="server" MaxLength="8"></asp:TextBox>（mm）
                    <span style="color: Red;">*</span>
                </td>
                <td style="height: 30px;">
                    POP高
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtGraphicLength" runat="server" MaxLength="8"></asp:TextBox>（mm）
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    位置宽
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtWindowWide" runat="server" MaxLength="8"></asp:TextBox>（mm）
                </td>
                <td style="height: 30px;">
                    位置高
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtWindowHigh" runat="server" MaxLength="8"></asp:TextBox>（mm）
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    位置深
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtWindowDeep" runat="server" MaxLength="8"></asp:TextBox>（mm）
                </td>
                <td style="height: 30px;">
                    位置规模
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   <asp:TextBox ID="txtWindowSize" runat="server" MaxLength="10"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    POP材质
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                     <%--<asp:TextBox ID="txtGraphicMaterial" runat="server" MaxLength="50"></asp:TextBox>--%>
                    <asp:DropDownList ID="ddlMaterialCategory" runat="server">
                      <asp:ListItem Value="0">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlMaterial" runat="server">
                      <asp:ListItem Value="0">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <%--<td style="height: 30px;">
                    单/双面
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtDoubleFace" runat="server" MaxLength="10"></asp:TextBox>
                </td>--%>
            </tr>
            <tr>
                <td style="height: 30px;">
                    是否有玻璃
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtGlass" runat="server" MaxLength="10"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    背景
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtBackdrop" runat="server" MaxLength="30"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    系列
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtCategory" runat="server" MaxLength="50"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    通电否
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   <asp:TextBox ID="txtIsElectricity" runat="server" MaxLength="10"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    悬挂否
                </td>
                <td style="text-align: left; padding-left: 5px;">
                     <asp:TextBox ID="txtIsHang" runat="server" MaxLength="10"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    门位置
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   <asp:TextBox ID="txtDoorPosition" runat="server" MaxLength="20"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    户外安装费
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtOOHInstallPrice" runat="server" MaxLength="10"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    是否生产
                </td>
                <td style="text-align: left; padding-left: 5px;">
                  <input type="radio" name="radioIsValid" value="1"/>是
                    <input type="radio" name="radioIsValid" style="margin-left:10px;" value="0"/>否
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    备注
                </td>
                <td style="text-align: left; padding-left: 5px;">
                     <asp:TextBox ID="txtRemark" runat="server" MaxLength="100" style=" width:200px;"></asp:TextBox>
                </td>
               <td style="height: 30px;">
                    角落类型
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   <asp:TextBox ID="txtCornerType" runat="server" MaxLength="20"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
<script src="../Scripts/common.js" type="text/javascript"></script>
<%--<script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>--%>
<script src="js/poplist.js" type="text/javascript"></script>

<script type="text/javascript">
    //解决jquery库的冲突，很重要！
    //var $j = jQuery.noConflict();
    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function (sender, e) {
        var eleId = e.get_postBackElement().id;

        if (eleId.indexOf("ddlCustomer") != -1) {
            $("#loadRegion").show();
        }
        if (eleId.indexOf("cblRegion") != -1) {
            $("#loadProvince").show();
            $("#loadChannel").show();
            $("#loadFormat").show();
            $("#loadShopLevel").show();
            $("#loadSheet").show();
            $("#loadGender").show(); 
        }
        if (eleId.indexOf("cblProvince") != -1) {
            $("#loadChannel").show();
            $("#loadFormat").show();
            $("#loadShopLevel").show();
            $("#loadSheet").show();
            $("#loadGender").show();
        }
        if (eleId.indexOf("cblChannel") != -1) {
            $("#loadFormat").show();
            $("#loadShopLevel").show();
            $("#loadSheet").show();
            $("#loadGender").show();
        }
        if (eleId.indexOf("cblFormat") != -1) {
            $("#loadShopLevel").show();
            $("#loadSheet").show();
            $("#loadGender").show();
        }

        if (eleId.indexOf("cblShopLevel") != -1) {
           
            $("#loadSheet").show();
            $("#loadGender").show();
        }

        if (eleId.indexOf("cblSheet") != -1) {

            $("#loadGender").show();
        }
    })

    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
       
        $("#loadProvince").hide();
        $("#loadChannel").hide();
        $("#loadFormat").hide();
        $("#loadShopLevel").hide();
        $("#loadSheet").hide();
        $("#loadGender").hide();
        
    })

</script>