<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddSubject.aspx.cs" Inherits="WebApp.Subjects.AddSubject" %>

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
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" /> 
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script> 
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            <asp:Label ID="labTitel" runat="server" Text="新增项目"></asp:Label>
        </p>
    </div>
    <div class="tr">
        >>项目信息</div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table class="table">
                <tr class="tr_bai">
                  <td>
                    新增类型：
                  </td>
                  <td colspan="3" style="text-align: left; padding-left: 5px;">
                      <asp:RadioButtonList ID="rblSubjectItemType" runat="server" 
                          RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" 
                          onselectedindexchanged="rblSubjectItemType_SelectedIndexChanged">
                      </asp:RadioButtonList>
                      <span style="color:Blue;">(提示：如果是特殊活动要选‘上海增补’)</span>
                  </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        所属客户：
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged">
                        </asp:DropDownList>
                        <span style="color: Red;">*</span>
                    </td>
                    
                </tr>
                <tr class="tr_bai">
                 <td>
                    订单类型：
                  </td>
                  <td colspan="3" style="text-align: left; padding-left: 5px;">
                     <asp:RadioButtonList ID="rblSubjectType" runat="server" 
                            RepeatDirection="Horizontal" AutoPostBack="true"
                            RepeatLayout="Flow" 
                            onselectedindexchanged="rblSubjectType_SelectedIndexChanged">
                        </asp:RadioButtonList>
                  </td>
                </tr>
                <tr class="tr_bai">
                    <td style="width: 120px;">
                        活动名称：
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px; width: 300px;">
                        <asp:DropDownList ID="ddlGuidance" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlGuidance_SelectedIndexChanged">
                            <asp:ListItem Value="0">请选择</asp:ListItem>
                        </asp:DropDownList>
                        <span style="color: Red;">*</span>
                    </td>
                    <%--<td style="width: 120px;">
                        ：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                       
                    </td>--%>
                </tr>
                <tr class="tr_bai" id="supplementSubjectListTr" runat="server">
                  <td>
                    增补项目：
                  </td>
                  <td colspan="3" style="text-align: left; padding-left: 5px;">
                     <asp:DropDownList ID="ddlSupplementSubjectList" runat="server" 
                          AutoPostBack="true" 
                          onselectedindexchanged="ddlSupplementSubjectList_SelectedIndexChanged">
                            <asp:ListItem Value="0">--请选择项目--</asp:ListItem>
                        </asp:DropDownList>
                  </td>
                </tr>
                <tr class="tr_bai">
                 <td>
                    项目名称：
                  </td>
                  <td colspan="3" style="text-align: left; padding-left: 5px;">
                     <asp:TextBox ID="txtSubjectName" runat="server" MaxLength="50" Style="width: 350px;"></asp:TextBox>
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
                        <asp:TextBox ID="txtBeginDate" runat="server" autocomplete="off" onclick="WdatePicker()" MaxLength="20"></asp:TextBox>
                        <span style="color: Red;">*</span>
                    </td>
                    <td style="width: 120px;">
                        结束时间：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtEndDate" runat="server" autocomplete="off" onclick="WdatePicker()" MaxLength="20"></asp:TextBox>
                        <span style="color: Red;">*</span>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        项目类型：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:DropDownList ID="ddlSubjectType" runat="server">
                            <asp:ListItem Value="0">请选择</asp:ListItem>
                        </asp:DropDownList>
                        (POP订单必选)
                    </td>
                    <td style="width: 120px;">
                        项目分类：
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
                        实施区域：
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <asp:RadioButtonList ID="rblPriceBlong" runat="server" RepeatDirection="Horizontal"
                            RepeatLayout="Flow">
                          
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr class="tr_bai" id="subjectTypeTr">
                    <td>
                        类型：
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;" class="style1">
                        <asp:CheckBox ID="cbNoOrderList" runat="server" AutoPostBack="true" OnCheckedChanged="cbNoOrderList_CheckedChanged" />无POP订单
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 补单区域：
                        <asp:RadioButtonList ID="rblRegion" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        </asp:RadioButtonList>
                        <asp:Label ID="labTipsMsg" runat="server" Text="" style=" color:Red; margin-left:10px;"></asp:Label>
                    </td>
                </tr>
                <tr class="tr_bai" runat="server" id="trSecondInstall">
                    <td>
                        是否二次安装：
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <asp:CheckBox ID="cbIsSecondInstall" runat="server" />是（安装费单独算）
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        基础安装费类型：
                        <asp:RadioButtonList ID="rblSecondInstallType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                           <%--<asp:ListItem Value="1">全部150</asp:ListItem>
                           <asp:ListItem Value="2">T1-T3级别150,T4-T7按实际算</asp:ListItem>--%>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        备注：
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtRemark" runat="server" Style="width: 350px;"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <br />
            <div style="text-align: center; height: 35px;">
                <div id="div0" style="display: none;">
                </div>
                <asp:Panel ID="Panel1" runat="server" Visible="false">
                    <asp:Button ID="btnSubmit" runat="server" Text="提 交" class="easyui-linkbutton" Style="width: 65px;
                        height: 26px;" OnClick="btnSubmit_Click" OnClientClick="return CheckVal0()" />
                    <img id="Img0" src="../image/WaitImg/loadingA.gif" style="display: none;" />
                </asp:Panel>
                <asp:Panel ID="Panel2" runat="server">
                    <asp:Button ID="btnNext" runat="server" Text="下一步" OnClientClick="return CheckVal()"
                        class="easyui-linkbutton" Style="width: 65px; height: 26px;" OnClick="btnNext_Click" />
                    <img id="loadingImg" src="../image/WaitImg/loadingA.gif" style="display: none;" />
                    &nbsp;&nbsp;&nbsp;
                    <%--<input type="button" value="返 回" onclick="javascript:window.history.go(-1)" class="easyui-linkbutton" 
            style="width: 65px; height:26px;"/>--%>
                    <asp:Button ID="btnBack" runat="server" Text="返 回" class="easyui-linkbutton" Style="width: 65px;
                        height: 26px;" OnClick="btnBack_Click" />
                </asp:Panel>
                <div id="div1">
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
<script src="js/addSubject.js" type="text/javascript"></script>
