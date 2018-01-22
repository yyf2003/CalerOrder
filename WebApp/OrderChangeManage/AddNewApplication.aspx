<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddNewApplication.aspx.cs"
    Inherits="WebApp.OrderChangeManage.js.AddNewApplication" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <script src="../../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            新增项目变更申请
        </p>
    </div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table class="layui-table" style="margin-top: 10px;">
                <tr>
                    <td>
                        客户
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true" style=" height:23px;" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged">
                        </asp:DropDownList>
                        <span style="color: Red; cursor: pointer;">*</span>
                    </td>
                </tr>
                <tr>
                    <td style="width: 80px;">
                        活动月份：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtGuidanceMonth" runat="server" AutoPostBack="true" CssClass="Wdate"
                            onclick="WdatePicker({skin:'whyGreen',dateFmt:'yyyy-MM',isShowClear:false,readOnly:true,onpicked:getMonth})" MaxLength="20" style=" width:80px;" OnTextChanged="txtGuidanceMonth_TextChanged"></asp:TextBox>
                        <asp:LinkButton ID="lbUp" runat="server" Style="margin-left: 20px; cursor: pointer;
                            color: Blue;" OnClick="lbUp_Click">上一个月</asp:LinkButton>
                        <asp:LinkButton ID="lbDown" runat="server" Style="margin-left: 20px; cursor: pointer;
                            color: Blue;" OnClick="lbDown_Click">下一个月</asp:LinkButton>
                    </td>
                </tr>
                <tr>
                    <td>
                        活动名称：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <%-- <asp:DropDownList ID="ddlGuidance" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlGuidance_SelectedIndexChanged">
                            <asp:ListItem Value="-1">--请选择活动指引--</asp:ListItem>
                        </asp:DropDownList>--%>
                        <asp:CheckBoxList ID="cblGuidanceList" runat="server" CellSpacing="25"
                            RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="5" AutoPostBack="true"
                            OnSelectedIndexChanged="cblGuidanceList_SelectedIndexChanged">
                        </asp:CheckBoxList>
                        <asp:Panel ID="Panel_EmptyGuidance" runat="server" Visible="false">
                            <span style="color: Red;">无活动信息！</span>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
            <blockquote class="layui-elem-quote" style="height: 15px; padding-top: 7px; font-weight: bold;">
                项目信息
            </blockquote>
            <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
                <HeaderTemplate>
                    <table class="layui-table" style="margin-top: -10px;">
                        <thead>
                            <tr>
                                <th style="width: 20px;">
                                </th>
                                <th style="width: 100px;">
                                    订单类型
                                </th>
                                <th style="width: 150px;">
                                    项目编号
                                </th>
                                <th style="width: 350px;">
                                    项目名称
                                </th>
                                <th>
                                    变更类型
                                </th>
                            </tr>
                        </thead>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td rowspan="2">
                            <asp:CheckBox ID="cbOne" runat="server" />
                            <asp:HiddenField ID="hfSubjectId" runat="server" Value='<%#Eval("Id") %>' />
                        </td>
                        <td style="font-weight: bold;">
                            <asp:Label ID="labSubjectType" runat="server" Text=""></asp:Label>
                        </td>
                        <td style="font-weight: bold;">
                            <%#Eval("SubjectNo") %>
                        </td>
                        <td style="font-weight: bold;">
                            <%#Eval("SubjectName") %>
                        </td>
                        <td>
                            <asp:RadioButtonList ID="rblChangeType" runat="server" RepeatDirection="Horizontal"
                                RepeatLayout="Flow">
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <%-- <td></td>--%>
                        <td colspan="4" style="text-align: left; padding-left: 15px;">
                            变更说明：
                            <asp:TextBox ID="txtRemark" runat="server" MaxLength="60" Style="width: 300px; height:21px;"></asp:TextBox>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%if (Repeater1.Items.Count == 0)
                      {%>
                    <tr>
                        <td colspan="5" style="text-align: center;">
                            --无数据信息--
                        </td>
                    </tr>
                    <%} %>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            <div style="text-align: center; margin-top: 20px; margin-bottom: 20px;">
                <asp:Button ID="btnSubmit" runat="server" Text="提 交" OnClick="btnSubmit_Click" OnClientClick="return Check();"
                    CssClass="layui-btn layui-btn-normal" />
                <img id="loadingImg" src="../image/WaitImg/loadingA.gif" style="display: none;" />
                <asp:Button ID="btnBack" runat="server" Text="返 回" CssClass="layui-btn layui-btn-primary"
                    Style="margin-left: 30px;" OnClick="btnBack_Click" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
<script type="text/javascript">
    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
        $("input[name^='cblGuidanceList']").change(function () {
            var checked = this.checked;
            if (checked) {
                $("input[name^='cblGuidanceList']").each(function () {
                    $(this).attr("checked", false);
                })
                $(this).attr("checked", true);

            }
        })
    })
    
    function Check() {
        var len = $("input[name$='cbOne']:checked").length;
        if (len == 0) {
            layer.msg("请选择要变更的项目");
            return false;
        }
        $("#btnBack").prop("disabled", true);
        $("#loadingImg").show();
        return true;
    }

    function ShowError(msg) {
        layer.msg(msg);
        $("#btnBack").prop("disabled", false);
    }
    function Success() {
        layer.msg("提交成功！");
        setTimeout(function () {
            window.location = "ApplicationList.aspx";
        }, 1000);

    }

    function getMonth() {
        $("#txtGuidanceMonth").blur();
    }
</script>
