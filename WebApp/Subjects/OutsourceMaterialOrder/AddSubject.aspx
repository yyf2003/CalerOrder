<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddSubject.aspx.cs" Inherits="WebApp.Subjects.OutsourceMaterialOrder.AddSubject" %>

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
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            <asp:Label ID="labTitel" runat="server" Text="外协物料订单—新增项目"></asp:Label>
        </p>
    </div>
    <div class="tr">
        >>项目信息</div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table class="table">
                <tr class="tr_bai">
                    <td>
                        所属客户
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:DropDownList ID="ddlCustomer" runat="server">
                        </asp:DropDownList>
                        <span style="color: Red;">*</span>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td style="width: 120px;">
                        选择活动
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        活动月份
                        <asp:TextBox ID="txtGuidanceMonth" runat="server" MaxLength="20" AutoPostBack="true"
                            CssClass="Wdate" onclick="WdatePicker({skin:'whyGreen',dateFmt:'yyyy-MM',isShowClear:false,readOnly:true,onpicked:getMonth})"
                            Style="width: 80px; height: 20px;" OnTextChanged="txtGuidanceMonth_TextChanged"></asp:TextBox>
                        &nbsp; 活动名称：
                        <asp:DropDownList ID="ddlGuidance" runat="server">
                            <asp:ListItem Value="0">--请选择--</asp:ListItem>
                        </asp:DropDownList>
                        <span style="color: Red;">*</span>
                    </td>
                   
                </tr>
                <tr class="tr_bai">
                    <td>
                        项目名称
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                       <asp:TextBox ID="txtSubjectName" runat="server" MaxLength="50" Style="width: 300px;"></asp:TextBox>
                       
                        <span style="color: Red;">*</span>
                        <asp:Label ID="labMsg" runat="server" Text="" Style="color: Red;"></asp:Label>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        外协名称
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:DropDownList ID="ddlOutsource" runat="server">
                            <asp:ListItem Value="0">--请选择--</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        备注
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtRemark" runat="server" Style="width: 280px;"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <div class="tr" style="margin-top: 10px;">
                >>添加物料信息</div>
            <table class="table">
                <tr class="tr_bai">
                    <td style="width: 120px;">
                        物料名称
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:DropDownList ID="ddlMaterial" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlMaterial_SelectedIndexChanged">
                            <asp:ListItem Value="0">--请选择--</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        单价
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:Label ID="labPrice" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        数量
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtMaterialAmount" runat="server" Text="1" MaxLength="3" Style="width: 90px;
                            text-align: center;"></asp:TextBox>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        备注
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtMaterialRemark" runat="server" MaxLength="30" Style="width: 300px;"></asp:TextBox>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                    </td>
                    <td style="text-align: left; padding-left: 5px; height: 30px;">
                        <asp:Button ID="btnAddMaterial" runat="server" Text="添 加" Style="width: 65px; height: 26px;"
                            OnClick="btnAddMaterial_Click" OnClientClick="return CheckMaterialVal()"/>
                        <asp:Button ID="btnEditMaterial" runat="server" Visible="false" Text="更 新" Style="width: 65px; height: 26px; margin-left:20px;"
                            OnClick="btnEditMaterial_Click" OnClientClick="return CheckMaterialVal()"/>
                        <asp:Label ID="labAddMaterialMsg" runat="server" Text="" Style="color: Red;"></asp:Label>
                    </td>
                </tr>
            </table>
            <div class="tr" style="margin-top: 10px;">
                >>物料信息</div>
            <asp:Repeater ID="tbMaterialList" runat="server" 
                onitemcommand="tbMaterialList_ItemCommand">
                <HeaderTemplate>
                    <table class="table" id="tbMaterialData">
                        <tr class="tr_hui">
                            <td style="width: 60px;">
                                序号
                            </td>
                            <td>
                                物料名称
                            </td>
                            <td style="width: 150px;">
                                单价
                            </td>
                            <td style="width: 150px;">
                                数量
                            </td>
                            <td>
                                备注
                            </td>
                            <td style="width: 90px;">
                                操作
                            </td>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="tr_bai materialData">
                        <td style="width: 60px;">
                            <%#Container.ItemIndex+1 %>
                        </td>
                        <td>
                            <%#Eval("MaterialName")%>
                        </td>
                        <td style="width: 150px;">
                            <%#Eval("UnitPrice")%>
                        </td>
                        <td style="width: 150px;">
                            <%#Eval("Amount")%>
                        </td>
                        <td>
                            <%#Eval("Remark")%>
                        </td>
                        <td style="width: 90px;">
                            <asp:LinkButton ID="lbEditMaterial" CommandName="editItem" CommandArgument='<%#Eval("MaterialId")%>' runat="server" ForeColor="Blue">修改</asp:LinkButton>
                            |
                            <asp:LinkButton ID="lbDeleteMaterial" CommandName="deleteItem" CommandArgument='<%#Eval("MaterialId")%>' runat="server" ForeColor="Red">删除</asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%if (tbMaterialList.Items.Count == 0)
                      { %>
                    <tr class="tr_bai">
                        <td colspan="6" style="text-align: center;">
                            --暂无数据--
                        </td>
                    </tr>
                    <%} %>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </ContentTemplate>
        <%--<Triggers>
       <asp:AsyncPostBackTrigger ControlID="btnAddMaterial" EventName="click" />
     </Triggers>--%>
    </asp:UpdatePanel>
    <div style="text-align: center; height: 35px; margin-top: 30px;">
        <asp:Button ID="btnSubmit" runat="server" Text="提 交" class="easyui-linkbutton" Style="width: 65px;
            height: 26px;" OnClick="btnSubmit_Click" OnClientClick="return CheckSubmitVal()" />
        <img id="ImgLoad" src="../../image/WaitImg/loadingA.gif" style="display: none;" />
        <asp:Button ID="btnBack" runat="server" Text="返 回" class="easyui-linkbutton" Style="width: 65px;
            height: 26px; margin-left:30px;" OnClick="btnBack_Click" />
    </div>
    </form>
</body>
</html>
<link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
<script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
<script src="js/addSubject.js" type="text/javascript"></script>
