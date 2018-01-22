<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubjectList.aspx.cs" Inherits="WebApp.Subjects.OutsourceMaterialOrder.SubjectList" %>

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
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <a href="/index.aspx">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /></a>
        <p class="nav_table_p">
            项目信息
        </p>
    </div>
    <div class="tr">
        >>搜索</div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table class="table">
                <tr class="tr_bai conditionTr">
                    <td style="width: 120px;">
                        客户
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px; width: 250px;">
                        <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true" 
                            onselectedindexchanged="ddlCustomer_SelectedIndexChanged">
                        </asp:DropDownList>
                        <span style="color: Red;">*</span>
                    </td>
                </tr>
                <tr class="tr_bai conditionTr">
                    <td style="width: 120px;">
                        活动月份
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px; width: 250px;">
                        <asp:TextBox ID="txtGuidanceMonth" runat="server" CssClass="Wdate" onclick="WdatePicker({skin:'whyGreen',dateFmt:'yyyy-MM',isShowClear:false,readOnly:true,onpicked:getMonth})"
                            Style="width: 80px;" OnTextChanged="txtGuidanceMonth_TextChanged" AutoPostBack="true"></asp:TextBox>
                        <img id="imgLoading1" style="display: none;" src='../../image/WaitImg/loadingA.gif' />
                    </td>
                </tr>
                <tr class="tr_bai conditionTr">
                    <td style="width: 120px;">
                        活动名称
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px; width: 250px;">
                        <div id="loadGuidance" style="display: none;">
                            <img src='../../image/WaitImg/loadingA.gif' />
                        </div>
                        <asp:CheckBoxList ID="cblGuidance" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"
                            RepeatColumns="8">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td style="width: 120px;">
                        项目名称
                    </td>
                    <td style="text-align: left; padding-left: 5px; width: 250px;">
                        <asp:TextBox ID="txtSubjectName" runat="server" MaxLength="50" Style="width: 200px;"></asp:TextBox>
                    </td>
                    <td style="width: 120px;">
                        项目编号
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtSubjectNo" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td style="width: 120px;">
                        审批状态
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <asp:RadioButtonList ID="rblApproveState" runat="server" RepeatDirection="Horizontal"
                            RepeatLayout="Flow">
                            <asp:ListItem Value="-1">全部 </asp:ListItem>
                            <asp:ListItem Value="0">未审批 </asp:ListItem>
                            <asp:ListItem Value="1">审批通过 </asp:ListItem>
                            <asp:ListItem Value="2">审批不通过 </asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div style="text-align: right; padding-right: 25px; margin-top: 8px; margin-bottom:10px;">
        <asp:Button ID="Button1" runat="server" Text="查 询" class="easyui-linkbutton" Style="width: 65px;
            height: 26px;" OnClick="Button1_Click" OnClientClick="return Check()" />
        <img id="loading" style="display: none;" src="../../image/WaitImg/loadingA.gif" />
        <asp:Button ID="btnAdd" Visible="false" runat="server" Text="新建项目"  class="easyui-linkbutton"
            Style="width: 65px; height: 26px; margin-left: 20px;" OnClick="btnAdd_Click" />
    </div>
    <div class="tr">
        >>信息列表
    </div>
    <div>
        <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" CellPadding="0"
            CssClass="table" BorderWidth="0" HeaderStyle-BorderStyle="None" EmptyDataText="--无符合条件的信息--"
            OnRowDataBound="gv_RowDataBound" OnRowCommand="gv_RowCommand">
            <Columns>
                <asp:TemplateField HeaderText="序号" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                    </ItemTemplate>
                    <HeaderStyle Width="50px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="订单类型" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:Label ID="labSubjectType" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="120px"></HeaderStyle>
                </asp:TemplateField>
                <asp:BoundField DataField="CustomerName" HeaderText="所属客户" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="ItemName" HeaderText="活动名称" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="SubjectNo" HeaderText="项目编号" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="SubjectName" HeaderText="项目名称" HeaderStyle-BorderColor="#dce0e9" />
                
                <asp:BoundField DataField="RealName" HeaderText="创建人" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="AddDate" HeaderText="创建时间" HeaderStyle-BorderColor="#dce0e9" />
                <asp:TemplateField HeaderText="项目状态" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:Label ID="labStatus" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="60px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="审批状态" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:Label ID="labApprove" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="60px"></HeaderStyle>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="查看" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:LinkButton CommandArgument='<%#Eval("Id") %>' CommandName="Check" ID="lbCheck"
                            runat="server" Style="color: Blue;">查看</asp:LinkButton>
                    </ItemTemplate>
                    <HeaderStyle Width="50px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="编辑" Visible="false" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:LinkButton CommandArgument='<%#Eval("Id") %>' ID="lbEdit"
                            runat="server" Style="color: Blue;">编辑</asp:LinkButton>
                    </ItemTemplate>
                    <HeaderStyle Width="60px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="修改订单" Visible="false" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:LinkButton CommandArgument='<%#Eval("Id") %>' CommandName="ModifyOrder" ID="lbModifyOrder" runat="server"
                            Style="color: Blue;">修改</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="操作" Visible="false" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:LinkButton CommandArgument='<%#Eval("Id") %>' CommandName="DeleteItem" ID="lbDelete"
                            runat="server" Style="color: red;">删除</asp:LinkButton>
                    </ItemTemplate>
                    <HeaderStyle Width="50px"></HeaderStyle>
                </asp:TemplateField>
            </Columns>
            <AlternatingRowStyle CssClass="tr_bai" />
            <HeaderStyle CssClass="tr_hui" />
            <RowStyle CssClass="tr_bai" />
            <SelectedRowStyle CssClass="tr_hui" />
            <EmptyDataRowStyle CssClass="tr_bai" />
        </asp:GridView>
    </div>
    <br />
    <div style="text-align: center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    </form>
</body>
</html>
<script src="js/list.js" type="text/javascript"></script>
