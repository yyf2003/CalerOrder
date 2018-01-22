<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddCustomer.aspx.cs" Inherits="WebApp.Customer.AddCustomer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
  
    <link href="../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../easyui1.4/jquery.min.js"></script>
    <script src="../easyui1.4/jquery.easyui.min.js"></script>
    <style type="text/css">
       #addRegionTb span
       {
         color:Blue; cursor:pointer;    
       }
    </style>
    <script type="text/javascript">
        var currCustomerId = '<%=customerId %>';
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            <asp:Label ID="labTitle" runat="server" Text="添加客户"></asp:Label>
        </p>
    </div>
    <div class="tr">
        >>客户信息
    </div>
    <table class="table">
           <tr>
                <td style="width:120px;height:30px;">客户编号</td>
                <td style="text-align:left;padding-left:5px;">
                    <asp:TextBox ID="txtCode" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width:120px;height:30px;">客户名称</td>
                <td style="text-align:left;padding-left:5px;">
                    <asp:TextBox ID="txtCustomerName" runat="server" MaxLength="50"></asp:TextBox>
                    <span style="color:Red;">*</span>
                </td>
            </tr>
           <tr>
                <td style="width:120px;height:30px;">客户简称</td>
                <td style="text-align:left;padding-left:5px;">
                    <asp:TextBox ID="txtShortName" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width:120px;height:30px;">联系人</td>
                <td style="text-align:left;padding-left:5px;">
                    <asp:TextBox ID="txtContact" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width:120px;height:30px;">联系电话</td>
                <td style="text-align:left;padding-left:5px;">
                    <asp:TextBox ID="txtTel" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
        <div class="tab" style="margin-top: 10px;">
            <span style="font-size:13px;">区域信息</span>  
            <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
        </div>
       <table id="addRegionTb" style=" width:100%">
         <tr class="tr_bai">
           <td style=" width:120px; text-align:center;">区域名称：</td>
           <td style=" text-align:left; padding-left:5px; width:300px;">
              <input type="text" id="txtRegionName" />
              &nbsp;
              <input type="button" value="添 加" id="btnAddRegion" class="easyui-linkbutton"
                        style="width: 65px; height: 26px;"/>
           </td>
           <td>
           其他客户：
               <asp:DropDownList ID="ddlOtherCustomer" runat="server">
                  <asp:ListItem Value="0">--请选择--</asp:ListItem>
               </asp:DropDownList>
            &nbsp;
            <span id="spanCopy">复制</span>&nbsp;&nbsp;<span id="spanCancelCopy">取消复制</span>
           </td>
         </tr>
       </table>
       <br />
       <div id="regionDiv" class="tab"></div>
       <br />
       <div style="text-align: center; height: 35px;">
        
           <input type="button" value="提 交" id="btnSubmit" class="easyui-linkbutton" 
            style="width: 65px; height:26px;"/>
            &nbsp;&nbsp;&nbsp;
            <input type="button" value="返 回" onclick="javascript:window.history.go(-1)" class="easyui-linkbutton" 
            style="width: 65px; height:26px;"/>
    </div>
       <asp:HiddenField ID="hfProvinces" runat="server" />
    </form>
</body>
</html>
<script src="js/addCustomer.js" type="text/javascript"></script>
