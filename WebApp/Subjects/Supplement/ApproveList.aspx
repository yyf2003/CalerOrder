<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApproveList.aspx.cs" Inherits="WebApp.Subjects.Supplement.ApproveList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="../../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="../../easyui1.4/jquery.easyui.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
           补单项目审批
        </p>
    </div>
    <div class="tr">
        >>搜索</div>
    <div>
        <table class="table">
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
                    <asp:RadioButtonList ID="rblApproveState" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
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
                <td colspan="4" style="padding-right: 20px; text-align: right; height: 30px;">
                    <asp:Button ID="Button2" runat="server" Text="查 询" class="easyui-linkbutton" 
            style="width: 65px; height:26px;" onclick="Button2_Click"/>
                </td>
            </tr>
        </table>
    </div>
    <br />
     <div class="tr">
        >>信息列表
    </div>
    <div>
        <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" 
        CellPadding="0" CssClass="table" BorderWidth="0" 
        HeaderStyle-BorderStyle="None" EmptyDataText="--无符合条件的信息--" onrowcommand="gv_RowCommand" 
            >
           <Columns>
              <asp:TemplateField HeaderText="序号" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                </ItemTemplate>
                <HeaderStyle Width="50px"></HeaderStyle>
            </asp:TemplateField>
            <asp:BoundField DataField="SubjectNo" HeaderText="项目编号" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="SubjectName" HeaderText="项目名称" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="CustomerName" HeaderText="所属客户" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="supplement.Price" HeaderText="补单金额" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="AddUserName" HeaderText="提交人" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="supplement.AddDate" HeaderText="提交时间" HeaderStyle-BorderColor="#dce0e9"/>
            
            <asp:TemplateField HeaderText="审批状态"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                   <%#Eval("supplement.ApproveState") != null ? Eval("supplement.ApproveState").ToString() == "0" ? "<span style='color:blue;'>待审批</span>" : Eval("supplement.ApproveState").ToString() == "1" ? "<span style='color:green;'>审批通过</span>" : Eval("supplement.ApproveState").ToString() == "2" ? "<span style='color:red;'>审批不通过</span>" : "<span style='color:blue;'>待审批</span>" : "<span style='color:blue;'>待审批</span>"%>
                </ItemTemplate>
                <HeaderStyle Width="60px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="查看" Visible="false"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                   <asp:LinkButton ID="lbCheck" runat="server" CommandName="Check" CommandArgument='<%#Eval("supplement.SupplementId") %>'>查看</asp:LinkButton>
                </ItemTemplate>
                <HeaderStyle Width="50px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="审批" Visible="false"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:LinkButton ID="lbApprove" runat="server" CommandName="Approve" CommandArgument='<%#Eval("supplement.SupplementId") %>'>审批</asp:LinkButton>
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
    <div style="text-align:center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" 
            onpagechanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    </form>
</body>