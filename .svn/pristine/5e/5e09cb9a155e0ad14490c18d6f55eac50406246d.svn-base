<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddSubject.aspx.cs" Inherits="WebApp.Subjects.NewShopSubject.AddSubject" %>

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
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            新增新开店项目
        </p>
    </div>
   <div class="tr">
        >>项目信息</div>
    <table class="table">
    <tr class="tr_bai">
            <td>
                活动名称
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
                <asp:DropDownList ID="ddlGuidance" runat="server" AutoPostBack="true"
                    onselectedindexchanged="ddlGuidance_SelectedIndexChanged">
                   <asp:ListItem Value="0">请选择</asp:ListItem>
                </asp:DropDownList>
               <span style="color:Red;">*</span>
            </td>
            <td style="width: 120px;">
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
               <asp:TextBox ID="txtSubjectName" runat="server" MaxLength="50" style=" width:200px;"></asp:TextBox>
                <span style="color:Red;">*</span>
                <asp:Label ID="labMsg" runat="server" Text="" style="color:Red;"></asp:Label>
            </td>
        </tr>
       
        <tr class="tr_bai">
            <td>
                开始时间
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
               <asp:TextBox ID="txtBeginDate" runat="server"  onclick="WdatePicker()" MaxLength="20"></asp:TextBox>
               <span style="color:Red;">*</span>
            </td>
            <td style="width: 120px;">
                结束时间
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:TextBox ID="txtEndDate" runat="server" onclick="WdatePicker()" MaxLength="20"></asp:TextBox>
                <span style="color:Red;">*</span>
            </td>
        </tr>
       
        <tr class="tr_bai">
            <td>
                所属客户
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true"
                    onselectedindexchanged="ddlCustomer_SelectedIndexChanged">
                </asp:DropDownList>
                <span style="color:Red;">*</span>
            </td>
            <td>
                区域：
            </td>
            <td style="text-align: left; padding-left: 5px;">
               <asp:RadioButtonList ID="rblRegion" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                </asp:RadioButtonList>
                <span style="color: Red;">*</span>
            </td>
        </tr>
   
        <tr class="tr_bai">
            <td>
                项目类型
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
                <asp:DropDownList ID="ddlSubjectType" runat="server">
                  <asp:ListItem Value="0">请选择</asp:ListItem>
                </asp:DropDownList>
                (POP订单必选)
            </td>
            <td style="width: 120px;">
                项目分类
            </td>
            <td style="text-align: left; padding-left: 5px;">
               <asp:DropDownList ID="ddlSubjectCategory" runat="server">
                  <asp:ListItem Value="0">请选择</asp:ListItem>
                </asp:DropDownList>
                (POP订单必选)
            </td>
        </tr>
       
        <tr class="tr_bai">
            <td>
                备注
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                <asp:TextBox ID="txtRemark" runat="server" style=" width:280px;"></asp:TextBox>
            </td>
        </tr>
    </table>
    <br />
    <div style="text-align: center; height: 35px;">
       <asp:Button ID="btnNext" runat="server" Text="下一步" 
            OnClientClick="return CheckVal()" class="easyui-linkbutton" 
            style="width: 65px; height:26px;" onclick="btnNext_Click"/>
            <img id="loadingImg" src="/image/WaitImg/loadingA.gif" style=" display:none;"/>
            &nbsp;&nbsp;&nbsp;
           
           <asp:Button ID="btnReturn" runat="server" Text="返 回" 
            class="easyui-linkbutton" style="width: 65px; height:26px;" 
            onclick="btnReturn_Click"/>
    </div>
    </form>
</body>
</html>
