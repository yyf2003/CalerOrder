<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OneShopMachineFrameList.aspx.cs"
    Inherits="WebApp.Customer.OneShopMachineFrameList" %>

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
            list-style: none;
            margin-bottom: 5px;
            text-decoration: underline;
            color: Blue;
            cursor: pointer;
            margin-left: 5px;
        }
    </style>
    <script type="text/javascript">
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
            >>店铺器架信息
        </div>
        <div>
            <asp:HiddenField ID="hfPromission" runat="server" />
            <div id="toolbar" style="display: none;">
                <div style="display: none;">
                    <asp:Button ID="btnSubmitDelete" runat="server" Text="delete" OnClick="btnDelete_Click" />
                </div>
                <a id="btnAddFrame" onclick="addFrame()" style="float: left; display: none;" class="easyui-linkbutton"
                    plain="true" icon="icon-add">新增</a> <a id="btnEditFrame" onclick="editFrame()" style="float: left;
                        display: none;" class="easyui-linkbutton" plain="true" icon="icon-edit">编辑</a>
                <a id="btnDeleteFrame" onclick="deleteFrame()" style="float: left; display: none;"
                    class="easyui-linkbutton" plain="true" icon="icon-remove">删除</a>
                <div class='datagrid-btn-separator'>
                </div>
                <a id="btnCheckEditLog" onclick="ShowEditLog()" style="float: left;"
                    class="easyui-linkbutton" plain="true" icon="icon-tip">修改记录</a>
            </div>
            <div class="containerDiv1">
                <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" CellPadding="0"
                    CssClass="table" BorderWidth="0" HeaderStyle-BorderStyle="None" DataKeyNames="FrameId"
                    EmptyDataText="--无符合条件的信息--" onrowdatabound="gv_RowDataBound" 
                   >
                    <Columns>
                        <asp:TemplateField HeaderStyle-BorderColor="#dce0e9">
                            <HeaderTemplate>
                                <asp:CheckBox ID="cbAll" runat="server" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="cbOne" runat="server" />
                                <asp:HiddenField ID="hfFrameId" runat="server" Value='<%#Eval("FrameId") %>' />
                            </ItemTemplate>
                            <HeaderStyle Width="30px"></HeaderStyle>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="序号" HeaderStyle-Width="40px" HeaderStyle-BorderColor="#dce0e9">
                            <ItemTemplate>
                                <%#Container.DataItemIndex + 1%>
                            </ItemTemplate>
                            <HeaderStyle Width="40px"></HeaderStyle>
                        </asp:TemplateField>
                        <asp:BoundField DataField="shop.ShopNo" HeaderText="店铺编码" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="shop.ShopName" HeaderText="店铺名称" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="shop.RegionName" HeaderText="区域" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="shop.ProvinceName" HeaderText="省份" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="shop.CityName" HeaderText="城市" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="shop.Channel" HeaderText="Channel" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="shop.Format" HeaderText="Format" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="frame.PositionName" HeaderText="位置" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="frame.MachineFrame" HeaderText="器架名称" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="frame.Gender" HeaderText="性别" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="frame.Count" HeaderText="数量" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="frame.CornerType" HeaderText="角落类型" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:BoundField DataField="frame.LevelNum" HeaderText="级别" HeaderStyle-BorderColor="#dce0e9" />
                        <asp:TemplateField HeaderText="是否生产" HeaderStyle-BorderColor="#dce0e9">
                          <ItemTemplate>
                              <asp:Label ID="labIsValid" runat="server" Text=""></asp:Label>
                          </ItemTemplate>
                        </asp:TemplateField>
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
    <div id="editDiv" title="编辑" style="display: none;">
        <table style="width: 400px; text-align: center;">
            <tr>
                <td style="width: 100px; height: 30px;">
                    店铺编号
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPOSCode" runat="server" MaxLength="50" ReadOnly="true"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width: 100px; height: 30px;">
                    POP位置
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selSheet">
                        <option value="0">--请选择--</option>
                       
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr>
                <td style="width: 100px; height: 30px;">
                    角落类型
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <%--<div style="position: relative;">
                      <asp:TextBox ID="txtCornerType" runat="server" MaxLength="50" Style="width: 200px;"></asp:TextBox>
                      <div id="divCornerType" style="display: none; position: absolute; width: 200px; background-color: White;
                            border: 1px solid #ccc; padding-top: 2px; z-index: 100;">
                            <ul id="ddlCornerType" style="margin-top: 0; width: 200px; height: 60px; overflow: auto;
                                margin-left: 0px; list-style: none;">
                                
                            </ul>
                        </div>
                    </div>--%>
                    <select id="selCornerType">
                       <option value="">--请选择--</option>
                    </select>
                </td>
            </tr>
            <tr>
                <td style="width: 100px; height: 30px;">
                    器架名称
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <%--<div style="position: relative;">
                        <asp:TextBox ID="txtFrameName" runat="server" MaxLength="50" Style="width: 200px;"></asp:TextBox>
                        <div id="divFrameList" style="display: none; position: absolute; width: 200px; background-color: White;
                            border: 1px solid #ccc; padding-top: 2px; z-index: 100;">
                            <ul id="ddlFrameMenu" style="margin-top: 0; width: 200px; height: 120px; overflow: auto;
                                margin-left: 0px; list-style: none;">
                            </ul>
                        </div>
                    </div>--%>
                    <select id="selFrameName">
                       <option value="">--请选择--</option>
                    </select>
                </td>
            </tr>
            
            <tr>
                <td style="width: 100px; height: 30px;">
                    性别
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="radio" name="tblSex" value="男" />男
                    <input type="radio" name="tblSex" value="女" />女
                    <input type="radio" name="tblSex" value="男女不限" />男女不限
                    <input type="radio" name="tblSex" value="男女混合" />男女混合
                    <input type="radio" name="tblSex" value="" />无
                </td>
            </tr>
            <tr>
                <td style="width: 100px; height: 30px;">
                    数量
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtNum" runat="server" MaxLength="10" Text="1"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width: 100px; height: 30px;">
                    级别
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtLevelNum" runat="server" MaxLength="2" Style="width: 80px;"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width: 100px; height: 30px;">
                    是否生产
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="radio" name="radioIsValid" value="1"/>是
                    <input type="radio" name="radioIsValid" value="0" />否
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
<script src="js/MachineFrame.js" type="text/javascript"></script>
<script src="js/OneShopMachineFrameList.js" type="text/javascript"></script>
