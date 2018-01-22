﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApproveList.aspx.cs" Inherits="WebApp.Subjects.ApproveList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="../easyui1.4/jquery.easyui.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <a href="/index.aspx">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /></a>
        <p class="nav_table_p">
            待审批的项目信息
        </p>
    </div>
    <div class="tr">
        >>搜索</div>
    <div>
        <table class="table">
        <tr class="tr_bai">
                <td style="width: 120px;">
                    客户名称
                </td>
                <td style="text-align: left; padding-left: 5px; width: 250px;">
                    <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true" 
                       onselectedindexchanged="ddlCustomer_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
                <td style="width: 120px;">
                    活动名称
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtGuidanceName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 120px;">
                    项目名称
                </td>
                <td style="text-align: left; padding-left: 5px; width: 250px;">
                    <asp:TextBox ID="tbSubjectName" runat="server"></asp:TextBox>
                </td>
                <td style="width: 120px;">
                    项目编号
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="tbSubjectNo" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 120px;">
                    审批状态
                </td>
                <td style="text-align: left; padding-left: 5px; width: 250px;">
                    <asp:RadioButtonList ID="rblApproveState" runat="server" RepeatDirection="Horizontal"
                        RepeatLayout="Flow">
                        <asp:ListItem Value="0" Selected="True">待审批 </asp:ListItem>
                        <asp:ListItem Value="1">已审批 </asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td style="width: 120px;">
                </td>
                <td style="text-align: left; padding-left: 5px;">
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 120px;">
                    活动名称
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                   <div id="divShowGuidanceName">
                    <asp:CheckBoxList ID="cblGuidanceNames" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    </asp:CheckBoxList>
                   </div>
                </td>
            </tr>
            <tr class="tr_bai">
                <td colspan="4" style="padding-right: 20px; text-align: right; height: 30px;">
                    <asp:Button ID="Button2" runat="server" Text="查 询" class="easyui-linkbutton" Style="width: 65px;
                        height: 26px;" OnClick="Button2_Click" />
                </td>
            </tr>
        </table>
    </div>
    <br />
    <div class="tr">
        >>信息列表
    </div>
    <div>
        <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" CellPadding="0"
            CssClass="table" BorderWidth="0" HeaderStyle-BorderStyle="None" EmptyDataText="--无符合条件的信息--"
            OnRowCommand="gv_RowCommand" OnRowDataBound="gv_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="序号" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                    </ItemTemplate>
                    <HeaderStyle Width="50px"></HeaderStyle>
                </asp:TemplateField>
                <asp:BoundField DataField="CustomerName" HeaderText="所属客户" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="ItemName" HeaderText="活动名称" HeaderStyle-BorderColor="#dce0e9" />
                <asp:TemplateField HeaderText="订单类型" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:Label ID="labSubjectType" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="120px"></HeaderStyle>
                </asp:TemplateField>
                <asp:BoundField DataField="SubjectNo" HeaderText="项目编号" HeaderStyle-BorderColor="#dce0e9" />
                <asp:TemplateField HeaderText="项目名称" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Contact" HeaderText="项目负责人" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="RealName" HeaderText="创建人" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="AddDate" HeaderText="创建时间" HeaderStyle-BorderColor="#dce0e9" />
                <asp:TemplateField HeaderText="审批状态" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <%#Eval("ApproveState") != null ? Eval("ApproveState").ToString() == "0" ? "<span style='color:blue;'>待审批</span>" : Eval("ApproveState").ToString() == "1" ? "<span style='color:green;'>审批通过</span>" : Eval("ApproveState").ToString() == "2" ? "<span style='color:red;'>审批不通过</span>" : "<span style='color:blue;'>待审批</span>" : "<span style='color:blue;'>待审批</span>"%>
                    </ItemTemplate>
                    <HeaderStyle Width="80px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="查看" Visible="false" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbCheck" runat="server" CommandName="Check" CommandArgument='<%#Container.DataItemIndex %>'>查看</asp:LinkButton>
                    </ItemTemplate>
                    <HeaderStyle Width="50px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="审批" Visible="false" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbApprove" runat="server" CommandName="Approve" CommandArgument='<%#Container.DataItemIndex %>'>审批</asp:LinkButton>
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
<script type="text/javascript">
    $(function () {
        Check();
        $("input[name$='rblApproveState']").on("change", function () {
            Check();
        })
    })

    function Check() {
        var state = $("input[name$='rblApproveState']:checked").val() || 0;
        if (state == 1) {
            $("#divShowGuidanceName").hide();
        }
        else
            $("#divShowGuidanceName").show();
    }
</script>