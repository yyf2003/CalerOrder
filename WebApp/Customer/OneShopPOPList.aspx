<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OneShopPOPList.aspx.cs"
    Inherits="WebApp.Customer.OneShopPOPList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
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
    <style type="text/css">
        #editDiv li
        {
            margin-bottom: 2px;
            height: 20px;
            font-size: 14px;
            cursor: pointer;
            padding-left: 5px;
            color: Blue;
        }
        #editDiv li:hover
        {
            background-color: #f0f1f2;
            text-decoration: underline;
        }
    </style>
    <script type="text/javascript">
        var url = '<%=url %>';
       
        var shopId = '<%=shopId %>';
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="body1" style="min-height: 500px;">
        <div class="tr">
            >>搜索
        </div>
        <div>
            <table class="table">
                <tr class="tr_bai">
                    <td style="width: 120px;">
                        POP编号
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtPOPNo" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        位置
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:CheckBoxList ID="cblSheet" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td colspan="2" style="padding-right: 10px; text-align: right; height: 30px;">
                        <asp:Button ID="btnSearch" runat="server" Text="查 询" OnClick="btnSearch_Click" class="easyui-linkbutton"
                            Style="width: 65px; height: 26px;" />
                    </td>
                </tr>
            </table>
        </div>
        <br />
        <div class="tr">
            >>店铺POP信息
        </div>
        <div>
            <asp:HiddenField ID="hfPromission" runat="server" />
            <div id="toolbar" style="display:none;">
                
                <div style="display: none;">
                    <asp:Button ID="btnDeletePOP" runat="server" Text="delete" OnClick="btnDeletePOP_Click" />
                </div>
                <a id="btnAdd" onclick="addPOP()" style="float: left; display:none;" class="easyui-linkbutton" plain="true" icon="icon-add">新增</a>    
                <a id="btnEdit" onclick="editPOP()" style="float: left;display:none;"class="easyui-linkbutton" plain="true" icon="icon-edit">编辑</a>
                <a id="btnDelete" onclick="deletePlan()" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-remove">删除</a>
                <div id="separator"  class='datagrid-btn-separator'>
                </div>
                <a id="btnCheckEditLog" onclick="ShowEditLog()" style="float: left;display: none;"
                    class="easyui-linkbutton" plain="true" icon="icon-tip">修改记录</a>
            </div>
            <div class="containerDiv">
                <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" CellPadding="0"
                    CssClass="table" BorderWidth="0" DataKeyNames="Id" HeaderStyle-BorderStyle="None"
                    EmptyDataText="--无信息--" Style="min-width: 1800px;" 
                    onrowdatabound="gv_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderStyle-BorderColor="#dce0e9">
                            <HeaderTemplate>
                                <asp:CheckBox ID="cbAll" runat="server" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="cbOne" runat="server" />
                                <asp:HiddenField ID="hfPOPId" runat="server" Value='<%#Eval("Id") %>' />
                            </ItemTemplate>
                            <HeaderStyle Width="30px"></HeaderStyle>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="序号" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                            <ItemTemplate>
                                <%#Container.DataItemIndex + 1%>
                            </ItemTemplate>
                            <HeaderStyle Width="50px"></HeaderStyle>
                        </asp:TemplateField>
                        <asp:BoundField DataField="shop.ShopNo" HeaderText="店铺编号" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.Sheet" HeaderText="位置" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.GraphicNo" HeaderText="POP编号" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.MachineFrameName" HeaderText="器架名称" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.OOHInstallPrice" HeaderText="应收户外安装费" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.OSOOHInstallPrice" HeaderText="应付户外安装费" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.Gender" HeaderText="性别" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.Quantity" HeaderText="数量" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.CornerType" HeaderText="角落类型" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.PositionDescription" HeaderText="位置描述" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.GraphicWidth" HeaderText="POP宽mm" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.GraphicLength" HeaderText="POP高mm" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.GraphicMaterial" HeaderText="POP材质" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.WindowWide" HeaderText="位置宽mm" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.WindowHigh" HeaderText="位置高mm" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.WindowDeep" HeaderText="位置深mm" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.WindowSize" HeaderText="位置规模" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:TemplateField HeaderText="是否生产" HeaderStyle-BorderColor="#dce0e9">
                          <ItemTemplate>
                              <asp:Label ID="labIsValid" runat="server" Text=""></asp:Label>
                          </ItemTemplate>
                        </asp:TemplateField>
                        <%--<asp:BoundField DataField="pop.DoubleFace" HeaderText="单/双面" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.Glass" HeaderText="是否有玻璃" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.Backdrop" HeaderText="背景" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.FixtureType" HeaderText="设备类别" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.IsElectricity" HeaderText="通电否" HeaderStyle-BorderColor="#dce0e9" />--%>
                        <asp:BoundField DataField="pop.LeftSideStick" HeaderText="左侧贴" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.RightSideStick" HeaderText="右侧贴" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.Floor" HeaderText="地铺" HeaderStyle-BorderColor="#dce0e9" />
                        <%--<asp:BoundField DataField="pop.WindowStick" HeaderText="窗贴" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.IsHang" HeaderText="悬挂否" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="pop.DoorPosition" HeaderText="门位置" HeaderStyle-BorderColor="#dce0e9" />--%>
                        <asp:BoundField DataField="pop.Remark" HeaderText="备注" HeaderStyle-BorderColor="#dce0e9" />
                        <%--<asp:TemplateField HeaderText="操作" HeaderStyle-Width="60px" HeaderStyle-BorderColor="#dce0e9">
                        <ItemTemplate>
                            删除
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
    </div>
    <br />
    <div id="editDiv" title="编辑POP信息" style="display: none;">
        <table id="POPtable" class="table" style="width: 700px; text-align: center;">
            <tr>
                <td style="width: 100px; height: 30px;">
                    店铺编号
                </td>
                <td style="text-align: left; width: 230px; padding-left: 5px;">
                    <asp:TextBox ID="txteditShopNo" runat="server" MaxLength="20" ReadOnly="true"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
                <td style="width: 100px; height: 30px;">
                    POP位置
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <div style="position: relative;">
                        <input type="text" id="txtSheet" style="width: 120px;" />
                        <div id="divSheetMenu" style="display: none; position: absolute; width: 125px; background-color: White;
                            border: 1px solid #ccc; padding-top: 2px; z-index: 100; ">
                            <ul id="ddlSheetMenu" style="margin-top: 0; width: 125px; margin-left: 0px; list-style: none;overflow:scroll; max-height:200px;">
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
                    <div style="position: relative;">
                        <input type="text" id="txtGender" style="width: 120px;" />
                        <div id="divGenderMenu" style="display: none; position: absolute; width: 125px; background-color: White;
                            border: 1px solid #ccc; padding-top: 2px; z-index: 100;">
                            <ul id="ddlGenderMenu" style="margin-top: 0; width: 125px; margin-left: 0px; list-style: none;">
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
                    橱窗信息
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    左侧贴：
                    <select id="seleWindowLeftSide">
                      <option value="">--请选择--</option>
                      <option value="Y">有</option>
                      <option value="N">无</option>
                    </select>
                    &nbsp;&nbsp;&nbsp;
                    右侧贴：
                    <select id="seleWindowRightSide">
                      <option value="">--请选择--</option>
                      <option value="Y">有</option>
                      <option value="N">无</option>
                    </select>
                    &nbsp;&nbsp;&nbsp;
                    地铺：
                    <select id="seleWindowFloor">
                       <option value="">--请选择--</option>
                      <option value="Y">有</option>
                      <option value="N">无</option>
                    </select>
                </td>
               
            </tr>
            <tr>
                <td style="height: 30px;">
                    POP材质
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlMaterialCategory" runat="server">
                        <asp:ListItem Value="0">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlMaterial" runat="server">
                        <asp:ListItem Value="0">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                    <%--<asp:TextBox ID="txtGraphicMaterial" runat="server" MaxLength="50"></asp:TextBox>--%>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            
            <tr>
                <td style="height: 30px;">
                    角落类型
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlCornerType" runat="server">
                      <asp:ListItem Value="">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td style="height: 30px;">
                    器架名称
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlFrameName" runat="server">
                      <asp:ListItem Value="">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    应收户外安装费
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtOOHInstallPrice" runat="server" MaxLength="10"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    应付户外安装费
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtOSOOHInstallPrice" runat="server" MaxLength="10"></asp:TextBox>
                </td>
            </tr>
            
            <tr>
                <td style="height: 30px;">
                    是否生产
                </td>
                <td  style="text-align: left; padding-left: 5px;">
                   <input type="radio" name="radioIsValid" value="1" />是
                    <input type="radio" name="radioIsValid" style="margin-left: 10px;" value="0" />否
                </td>
                <td style="height: 30px;">
                    备注
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtRemark" runat="server" MaxLength="100" Style="width: 200px;"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    <asp:HiddenField ID="hfShopNo" runat="server" />
    </form>
</body>
</html>
 <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
  <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
<script src="../Scripts/common.js" type="text/javascript"></script>
<script src="js/poplist.js" type="text/javascript"></script>
<script src="js/OneShopPOPList.js" type="text/javascript"></script>
