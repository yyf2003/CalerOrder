<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddGuidance.aspx.cs" Inherits="WebApp.Subjects.AddGuidance" %>

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
    <style type="text/css">
        #typeContainer
        {
            width: 810px;
        }
        #typeContainer input
        {
            float: left;
            width: 100px;
            margin-bottom: 5px;
        }
        #typeContainer span
        {
            float: left;
            margin-right: 10px;
            margin-left: 0px;
            margin-bottom: 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            <asp:Label ID="labTitel" runat="server" Text="新增活动指导"></asp:Label>
        </p>
    </div>
    <div class="tr">
        >>活动信息</div>
    <div class="tab">
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 120px;">
                    所属客户
                </td>
                <td style="text-align: left; padding-left: 5px; width: 300px;">
                    <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged">
                    </asp:DropDownList>
                    <span style="color: Red;">*</span>
                </td>
                <td style="width: 120px;">
                    类型
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:RadioButtonList ID="rblType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="1" Selected="True">新建&nbsp;</asp:ListItem>
                        <%--<asp:ListItem Value="2">增补&nbsp;</asp:ListItem>--%>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    活动名称
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px; ">
                    <asp:TextBox ID="txtItemName" runat="server" MaxLength="50" Style="width: 200px;"></asp:TextBox>
                    <span style="color: Red;">*</span>
                    <asp:Label ID="labMsg" runat="server" Text="" Style="color: Red;"></asp:Label>
                </td>
                
            </tr>
            <tr class="tr_bai">
                <td>
                    活动月份
                </td>
                <td style="text-align: left; padding-left: 5px; width: 300px;">
                    <asp:TextBox ID="txtGuidanceMonth" runat="server" CssClass="Wdate" onclick="WdatePicker({skin:'whyGreen',dateFmt:'yyyy年MM月'})"
                        MaxLength="20"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
                <td style="width: 120px;">
                    起始时间
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtBeginDate" runat="server" CssClass="Wdate" onclick="WdatePicker()"
                        MaxLength="20"></asp:TextBox>
                    —
                    <asp:TextBox ID="txtEndDate" runat="server" CssClass="Wdate" onclick="WdatePicker()"
                        MaxLength="20"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
             <tr class="tr_bai">
                <td>
                    活动类型
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                  <asp:RadioButtonList ID="rblActivityType" runat="server" RepeatDirection="Horizontal"
                        RepeatLayout="Flow">
                    </asp:RadioButtonList>
                    <span style="color: Red;">*</span>
                    <asp:RadioButtonList ID="rblHasInstallFees" runat="server"  RepeatDirection="Horizontal"
                        RepeatLayout="Flow" style=" display:none; margin-left:20px;">
                        <asp:ListItem Value="1" Selected="True">有安装费 </asp:ListItem>
                        <asp:ListItem Value="0">无安装费 </asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:RadioButtonList ID="rblHasExperssFees" runat="server"  RepeatDirection="Horizontal"
                        RepeatLayout="Flow" style=" display:none; margin-left:20px;">
                        <asp:ListItem Value="1" Selected="True">有快递费 </asp:ListItem>
                        <asp:ListItem Value="0">无快递费 </asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:DropDownList ID="ddlExperssPrice" runat="server"  style=" display:none;">
                      <asp:ListItem Value="0">--请选择发货费--</asp:ListItem>
                    </asp:DropDownList>
                </td>
             </tr>
            <tr class="tr_bai">
                <td>
                    材质价格方案
                </td>
                <td colspan="3" style="text-align: left; padding: 5px;">
                    <asp:DropDownList ID="ddlPriceItemList" runat="server">
                        <asp:ListItem Value="0">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    项目分类
                </td>
                <td colspan="3" style="text-align: left; padding: 5px;">
                    <div id="typeContainer" runat="server">
                    </div>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    项目命名规范
                </td>
                <td colspan="3" style="text-align: left; padding: 5px;">
                    <asp:TextBox ID="txtSubjectNames" runat="server" TextMode="MultiLine" Style="width: 580px;
                        height: 80px;"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    备注
                </td>
                <td colspan="3" style="text-align: left; padding: 5px;">
                    <asp:TextBox ID="txtRemark" runat="server" MaxLength="20" TextMode="MultiLine" Style="width: 580px;
                        height: 80px;"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    <br />
    <div style="text-align: center; height: 35px;">
        <div id="buttonDiv">
            <asp:Button ID="btnSubmit" runat="server" Text="提 交" OnClientClick="return CheckVal()"
                class="easyui-linkbutton" Style="width: 65px; height: 26px;" OnClick="btnSubmit_Click" />
            &nbsp;&nbsp;&nbsp;
            <input type="button" value="返 回" onclick="javascript:window.history.go(-1)" class="easyui-linkbutton"
                style="width: 65px; height: 26px;" />
        </div>
        <div id="waitingDiv" style="display: none;">
            <img src="/image/WaitImg/loading1.gif" />
        </div>
    </div>
    <br />
    <br />
    <asp:HiddenField ID="hfSubjectType" runat="server" />
    </form>
</body>
</html>
<script type="text/javascript">
    function CheckVal() {
        var ItemName = $.trim($("#txtItemName").val());

        var ActivityType = $("input:radio[name='rblActivityType']:checked").val() || -1;

        var month = $.trim($("#txtGuidanceMonth").val());
        var BeginDate = $.trim($("#txtBeginDate").val());
        var EndDate = $.trim($("#txtEndDate").val());
        var SubjectNames = $.trim($("#txtSubjectNames").val());
        var experssPrice = $("#ddlExperssPrice").val()||0;
        if ($("#ddlCustomer").val() == "0") {
            alert("请选择所属客户");
            return false;
        }
        if (ItemName == "") {
            alert("请输入活动名称");
            return false;
        }
        if (ActivityType == -1) {
            alert("请选择活动类型");
            return false;
        }
        if (ActivityType == 2) {
            if (experssPrice == 0) {
                alert("请选择发货费");
                return false;
            }
        }
        if (month == "") {
            alert("请选择活动月份");
            return false;
        }
        if (BeginDate == "") {
            alert("请选择开始时间");
            return false;
        }
        if (EndDate == "") {
            alert("请选择结束时间");
            return false;
        }
        var subjectTypes = "";
        $("input[name='txtTypeName']").each(function () {
            var str = "";
            var text = $.trim($(this).val());
            if (text != "") {
                var id = $(this).data("typeid") || 0;
                str = id + ":" + text;
                subjectTypes += str + "|";
            }
        })
        if (subjectTypes.length > 0) {
            subjectTypes = subjectTypes.substring(0, subjectTypes.length - 1);
            $("#hfSubjectType").val(subjectTypes);
        }
        else {
            alert("至少填写一个项目分类");
            return false;
        }

        if (SubjectNames == "") {
            alert("请输入项目命名规范");
            return false;
        }

        if (confirm("是否提交？")) {
            $("#buttonDiv").hide();
            $("#waitingDiv").show();
        }
        else
            return false;
    }

    $(function () {
        show();
        $("body").keydown(function (event) {

            if (event.keyCode == 13) {
                var tagName = event.target.tagName;
                if (tagName == "TEXTAREA") {
                    return true;
                }
                else
                    return false;
            }
        });

        $("#typeContainer").delegate("span[name='deletetype']", "click", function () {
            var input = $(this).prev("input");
            var span = $(this);
            
            var typeId = input.data("typeid") || 0;
            if (typeId > 0) {
                $.ajax({
                    type: "get",
                    url: "Handler/AddGuidance.ashx?type=deleteType&typeId=" + typeId,
                    cache: false,
                    success: function (data) {
                        if (data == "ok") {
                            input.remove();
                            span.remove();
                        }
                        else
                            alert("删除失败！");
                    }
                })
            }
            else {
                input.remove();
                $(this).remove();
            }
        })

        $("#btnAdd").click(function () {
            var div = "<input type='text' data-typeid='0' name='txtTypeName' value=''/><span name='deletetype' style='color:red;cursor:pointer;'>×</span>";
            $(this).parent().before(div);
        })

        $("input[name='rblActivityType']").change(function () {

            show();
        })
    })

    function show() {
        var val = $("input[name='rblActivityType']:checked").val() || 0;
        $("#rblHasInstallFees").hide();
        $("#rblHasExperssFees").hide();
        $("#ddlExperssPrice").val("0").hide();
        if (val == 1) {
            $("#rblHasInstallFees").show();
        }
        else if (val == 2) {
            $("#ddlExperssPrice").show();

        }
        else if (val == 3) {
            $("#rblHasExperssFees").show();
        }
        
       
    }
</script>
