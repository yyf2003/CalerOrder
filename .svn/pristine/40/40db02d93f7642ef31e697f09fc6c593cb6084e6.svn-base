<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Subjects.Material.List" %>

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
        function FinishImport() {
            $("#hfIsFinishImport").val("1");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            <asp:Label ID="labTitel" runat="server" Text="物料信息"></asp:Label>
        </p>
    </div>
    <div class="tr">
        >>搜索</div>
    <div>
        <table class="table">
           
            <tr class="tr_bai">
                <td style="width: 150px;">
                    活动名称
                </td>
                <td style="width: 200px; text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtSubjectName" runat="server" MaxLength="20"></asp:TextBox>
                </td>
                <td style="width: 150px;">
                   开始时间
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtBegin" runat="server" MaxLength="20"  onclick="WdatePicker()"></asp:TextBox>
                    —
                    <asp:TextBox ID="txtEnd" runat="server" MaxLength="20"  onclick="WdatePicker()"></asp:TextBox>
                </td>
            </tr>
            
            <tr class="tr_bai">
                <td colspan="4" style="padding-right: 20px; text-align: right; height: 30px;">
                    <asp:Button ID="btnSearch" runat="server" Text="查 询" 
                        onclick="btnSearch_Click" class="easyui-linkbutton" style="width: 65px; height:26px;"/>&nbsp;&nbsp;
                    <input type="button" id="btnAdd" value="添 加" class="easyui-linkbutton" style="width: 65px; height:26px;"/>
                </td>
            </tr>
        </table>
    </div>
    <br />
    <div class="tr">
        >>信息列表</div>
    <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" 
        CellPadding="0" CssClass="table" BorderWidth="0" 
        HeaderStyle-BorderStyle="None" EmptyDataText="--无符合条件的信息--" 
        onrowdatabound="gv_RowDataBound" onrowcommand="gv_RowCommand" 
        >
       <Columns>
          <asp:TemplateField HeaderText="序号" HeaderStyle-Width="60px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                </ItemTemplate>
                <HeaderStyle Width="60px"></HeaderStyle>
            </asp:TemplateField>
            <asp:BoundField DataField="SubjectName"  HeaderText="活动名称" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="BeginDate" DataFormatString="{0:yyyy-MM-dd}" HeaderText="开始时间" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="EndDate" DataFormatString="{0:yyyy-MM-dd}" HeaderText="结束时间" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:TemplateField HeaderText="已导物料" HeaderStyle-BorderColor="#dce0e9">
               <ItemTemplate>
                   <asp:Label ID="labMaterial" runat="server" Text=""></asp:Label>
               </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="物料信息" HeaderStyle-BorderColor="#dce0e9">
               <ItemTemplate>
                   <span  onclick="checkMaterial(<%#Eval("ItemId") %>)" style="color:blue; cursor:pointer;">查看</span>
               </ItemTemplate>
            </asp:TemplateField>
             <asp:TemplateField HeaderText="操作" HeaderStyle-Width="180px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                  <%-- onclick="edit(<%#Eval("ItemId") %>,this)"--%>
                    <span name="spanEdit" data-customerid='<%#Eval("CustomerId") %>' data-subjectid='<%#Eval("SubjectId") %>' data-itemid='<%#Eval("ItemId") %>'  style="color:blue; cursor:pointer;">编辑</span>
                    |
                    <span  onclick="importMaterial(<%#Eval("ItemId") %>)" style="color:blue; cursor:pointer;">导入物料</span>
                    |
                    <asp:LinkButton ID="lbDelete"  CommandArgument='<%#Eval("ItemId") %>' CommandName="deleteItem" runat="server" OnClientClick="return confirm('确定删除？')">删除</asp:LinkButton>
                </ItemTemplate>
               
            </asp:TemplateField>
       </Columns>
        <AlternatingRowStyle CssClass="tr_bai" />
        <HeaderStyle CssClass="tr_hui"/>
        <RowStyle CssClass="tr_bai" />
        <SelectedRowStyle CssClass="tr_hui" />
        <EmptyDataRowStyle CssClass="tr_bai" />
    </asp:GridView>
    <br />
    <div style="text-align:center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" 
            onpagechanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
     <div id="editDiv" title="添加" style="display:none;">
        <table style="width:550px;text-align:center;margin-top:20px;">
           <tr class="tr_bai">
            <td>
                客户
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:DropDownList ID="ddlCustomer" runat="server">
                </asp:DropDownList>
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
                 请选择活动
               </td>
               <td style="text-align: left; padding-left: 5px; height: 80px; vertical-align: top;">
                  <div id="projectsDiv" style="width: 80%; margin: 5px;">
                </div>
               </td>
            </tr>
        </table>
    </div>
      <div id="importDiv" title="添加" style="display:none;">
         <iframe src=""  frameborder="0" scrolling="auto" name="iframe1" id="iframe1" height="300" width="100%"></iframe>
      </div>
    <asp:HiddenField ID="hfIsFinishImport" runat="server" />
    </form>
</body>
</html>
<script src="js/list.js" type="text/javascript"></script>
