<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Subjects.Supplement.List" %>

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
    <script type="text/javascript">
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
           补单信息
        </p>
    </div>
        <div class="tr">
        >>搜索</div>
    <div>
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 120px;">
                    客户
                </td>
                <td style="text-align: left; padding-left: 5px; width: 250px;">
                    <asp:DropDownList ID="ddlCustomer" runat="server">
                    </asp:DropDownList>
                </td>
                <td style="width: 120px;">
                    
                </td>
                <td style="text-align: left; padding-left: 5px;">
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 120px;">
                    项目名称
                </td>
                <td style="text-align: left; padding-left: 5px; width: 250px;">
                    <asp:TextBox ID="txtSubjectName" runat="server" MaxLength="50"></asp:TextBox>
                </td>
                <td style="width: 120px;">
                    项目编号
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   <asp:TextBox ID="txtSubjectNo" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            
            <tr class="tr_bai">
                <td colspan="4" style="padding-right: 20px; text-align: right; height: 30px;">
                    <asp:Button ID="btnSearch" runat="server" Text="查 询" class="easyui-linkbutton" 
            style="width: 65px; height:26px;" onclick="btnSearch_Click"/>
            &nbsp;&nbsp;
                    <asp:Button ID="btnAdd" runat="server" Text="添加补单" Visible="false" OnClientClick="return false;" class="easyui-linkbutton" style="width: 65px; height:26px;"/>
                    
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
        HeaderStyle-BorderStyle="None" EmptyDataText="--无符合条件的信息--" 
            onrowdatabound="gv_RowDataBound" onrowcommand="gv_RowCommand" 
            >
           <Columns>
              <asp:TemplateField HeaderText="序号" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                </ItemTemplate>
                <HeaderStyle Width="50px"></HeaderStyle>
            </asp:TemplateField>
            <%--<asp:BoundField DataField="CustomerName" HeaderText="所属客户" HeaderStyle-BorderColor="#dce0e9"/>--%>
            <%--<asp:BoundField DataField="SubjectNo" HeaderText="项目编号" HeaderStyle-BorderColor="#dce0e9"/>--%>
            <asp:BoundField DataField="SubjectName" HeaderText="活动名称" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="Price" HeaderText="补单金额" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="RealName" HeaderText="申请人" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="AddDate" HeaderText="申请时间" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:TemplateField HeaderText="订单状态"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <%#Eval("IsSubmit").ToString() == "1" ? "<span style='color:green;'>已提交</span>" : Eval("IsSubmit").ToString() == "2" ? "<span style='color:red;'>已删除</span>" : "<span style='color:red;'>未提交</span>"%>
                </ItemTemplate>
                <HeaderStyle Width="60px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="审批状态"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <%--<asp:Label ID="labApprove" runat="server" Text=""></asp:Label>--%>
                    <%#Eval("ApproveState").ToString() == "1" ? "<span style='color:green;'>通过</span>" : Eval("ApproveState").ToString() == "2" ? "<span style='color:red;'>不通过</span>" : "<span>待审批</span>"%>
                </ItemTemplate>
                <HeaderStyle Width="60px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="编辑" Visible="false"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:Label ID="labEdit" runat="server" Text="">
                       <span name="spanEdit"  data-customerid='<%#Eval("CustomerId") %>' data-itemid='<%#Eval("SupplementId") %>'  style="color:blue; cursor:pointer;">编辑</span>
                    </asp:Label>
                    
                </ItemTemplate>
                <HeaderStyle Width="80px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="补单明细"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <span name="spanCheckOrder" onclick="checkOrder(<%#Eval("SupplementId") %>)"  style="cursor:pointer;color:Blue;">查看订单</span>
                </ItemTemplate>
                <HeaderStyle Width="80px"></HeaderStyle>
            </asp:TemplateField>
            
             <asp:TemplateField HeaderText="操作" Visible="false"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:Label ID="labEditOrder" runat="server" Text="">
                       <span name="spanAddOrder" onclick="editOrder(<%#Eval("SupplementId") %>)"  style="cursor:pointer;color:Blue;">添加订单</span>
                    </asp:Label>
                    |
                    <asp:LinkButton CommandArgument='<%#Eval("SupplementId") %>' CommandName="DeleteItem" ID="lbDelete"  runat="server" style="color:red;">删除</asp:LinkButton>
                </ItemTemplate>
                <HeaderStyle Width="120px"></HeaderStyle>
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
     <br />

     <div id="editDiv" title="添加" style="display:none;">
        <table style="width:550px;text-align:center;margin-top:5px;">
           <tr class="tr_bai">
            <td>
                客户
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:DropDownList ID="ddlAddCustomer" runat="server">
                </asp:DropDownList>
                <span style="color:Red;">*</span>
            </td>
        </tr>
           <tr>
                <td style="width:120px;height:30px;">请选择起止时间</td>
                <td style="text-align:left;padding-left:5px;">
                    <asp:TextBox ID="txtBeginDate" runat="server" onfocus="WdatePicker()" style=" width:120px;"></asp:TextBox>
                    —
                    <asp:TextBox ID="txtEndDate" runat="server" onfocus="WdatePicker()" style=" width:120px;"></asp:TextBox>
                    &nbsp;
                <input type="button" id="btnSearchSubject" value="查  询" class="easyui-linkbutton"
                    style="width: 65px; height: 26px;" />
                </td>
            </tr>
            <tr>
               <td style="vertical-align: top; padding-top:8px;">
                 请选择要补单活动
               </td>
               <td style="text-align: left; padding-left: 5px; height: 80px; vertical-align: top;">
                 <div id="projectsDiv" style="width: 80%; margin: 5px;">
                </div>
               </td>
            </tr>
            <tr>
               <td>补单金额</td>
               <td style="text-align:left;padding-left:5px;">
                   <asp:TextBox ID="txtPrice" runat="server" MaxLength="12"></asp:TextBox>
               </td>
            </tr>
            <tr>
               <td>补单原因</td>
               <td style="text-align:left;padding-left:5px; padding-top:8px;">
                   <asp:TextBox ID="txtReason" runat="server" TextMode="MultiLine"  MaxLength="10" style=" height:50px; width:300px;"></asp:TextBox>
                   (<span style="color:Red;">*</span>不超过50字)
               </td>
            </tr>
        </table>
    </div>
   
    <div id="editOrderDiv" title="添加补单明细" style="display:none;">
         <iframe src=""  frameborder="0" scrolling="auto" name="iframe1" id="iframe1" width="100%"></iframe>
      </div>
      <div id="checkOrderDiv" title="查看补单明细" style="display:none;">
         <iframe src=""  frameborder="0" scrolling="auto" name="iframe2" id="iframe2" width="100%"></iframe>
      </div>
    <asp:HiddenField ID="hfIsFinishImport" runat="server" />
    </form>
</body>
</html>
<script src="js/list.js" type="text/javascript"></script>
