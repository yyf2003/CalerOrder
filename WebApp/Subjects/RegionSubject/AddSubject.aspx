<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddSubject.aspx.cs" Inherits="WebApp.Subjects.RegionSubject.AddSubject" %>

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
            分区订单—新增项目
        </p>
    </div>
    <div class="tr">
        >>项目信息</div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table class="table">
                <tr class="tr_bai">
                    <td style="width: 120px;">
                        所属客户：
                    </td>
                    <td style="text-align: left; padding-left: 5px; width: 250px;">
                        <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged">
                        </asp:DropDownList>
                        <span style="color: Red;">*</span>
                    </td>
                    <td style="width: 100px;">
                        订单类型：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:RadioButtonList ID="rblSubjectType" runat="server" RepeatDirection="Horizontal"
                            RepeatLayout="Flow" AutoPostBack="true" OnSelectedIndexChanged="rblSubjectType_SelectedIndexChanged">
                        </asp:RadioButtonList>
                        <span style="color: Red;">*</span>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        活动名称：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:DropDownList ID="ddlGuidance" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlGuidance_SelectedIndexChanged">
                            <asp:ListItem Value="-1">请选择</asp:ListItem>
                        </asp:DropDownList>
                        <span style="color: Red;">*</span>
                    </td>
                    <td>
                        项目名称：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtSubjectName" runat="server" MaxLength="50" Style="width: 250px;"></asp:TextBox>
                        <asp:DropDownList ID="ddlSubjectName" runat="server" Visible="false">
                            <asp:ListItem Value="0">--请选择项目--</asp:ListItem>
                        </asp:DropDownList>
                        <span style="color: Red;">*</span>
                        <asp:Label ID="labMsg" runat="server" Text="" Style="color: Red;"></asp:Label>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        开始时间：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtBeginDate" runat="server" onclick="WdatePicker()" MaxLength="20"></asp:TextBox>
                        <span style="color: Red;">*</span>
                    </td>
                    <td>
                        结束时间：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtEndDate" runat="server" onclick="WdatePicker()" MaxLength="20"></asp:TextBox>
                        <span style="color: Red;">*</span>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        区域：
                    </td>
                    <td colspan="3"  style="text-align: left; padding-left: 5px;">
                        <asp:RadioButtonList ID="rblRegion" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        </asp:RadioButtonList>
                        <span style="color: Red;">*</span>
                    </td>
                   
                </tr>
                <tr class="tr_bai">
                    <td>
                        是否二次安装：
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <asp:CheckBox ID="cbIsSecondInstall" runat="server" />是（安装费单独算）
                         &nbsp;&nbsp;&nbsp;&nbsp;
                        基础安装费类型：
                        <asp:RadioButtonList ID="rblSecondInstallType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                         
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        备注：
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtRemark" runat="server" Style="width: 280px;"></asp:TextBox>
                    </td>
                </tr>
            </table>

            <br />
            <div class="tr">>>外协选择
              <span style="color:Red; font-weight:bolder;">*</span>
            </div>
            <table class="table">
               <tr class="tr_bai">
                  <td style="width: 120px;">生产外协：</td>
                  <td style=" text-align:left; padding-left:5px;">
                      <asp:DropDownList ID="ddlOutsourceRegion" runat="server" 
                          onselectedindexchanged="ddlOutsourceRegion_SelectedIndexChanged" AutoPostBack="true">
                        
                      </asp:DropDownList>
                      <asp:DropDownList ID="ddlOutsource" runat="server">
                         <asp:ListItem Value="0">--请选择外协--</asp:ListItem>
                      </asp:DropDownList>
                      
                  </td>
               </tr>
               <tr class="tr_bai">
                  <td></td>
                  <td style=" text-align:left; padding-left:5px;">
                     <span style=" color:Blue;">提示：如果此项目中所有订单都是非店铺默认外协生产，就请选择生产外协；如果订单中需要多个生产外协，请在订单模板中填写！</span>
                  </td>
               </tr>
            </table>
            <br />
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnNext" EventName="click" />
        </Triggers>
    </asp:UpdatePanel>
    <div style="text-align: center; height: 35px;">
        <asp:Button ID="btnNext" runat="server" Text="下一步" OnClientClick="return CheckVal()"
            class="easyui-linkbutton" Style="width: 65px; height: 26px;" OnClick="btnNext_Click" />
        <img id="loadingImg" src="/image/WaitImg/loadingA.gif" style="display: none;" />
        &nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnReturn" runat="server" Text="返 回" class="easyui-linkbutton" Style="width: 65px;
            height: 26px;" OnClick="btnReturn_Click" />
    </div>
    </form>
</body>
</html>
<script src="js/addSubject.js" type="text/javascript"></script>
