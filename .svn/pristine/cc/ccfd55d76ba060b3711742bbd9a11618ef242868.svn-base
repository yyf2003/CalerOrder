<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Approve.aspx.cs" Inherits="WebApp.Subjects.Supplement.Approve" %>

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
    <style type="text/css">
    .table1
    {
        border-collapse: collapse;
        text-align: center;
        font-size: 12px;
        width:100%;
    }
    .table1 tr
    {
        height: 30px;
    }
    .table1 td
    {
        border: #dce0e9 solid 1px;
    }
    .center1
    {
        text-align: center;
    }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            补单审批
        </p>
    </div>
    <div class="tr">
        >>补单项目信息
    </div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                项目编号
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td class="style1">
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style1">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td class="style1">
                补单金额
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style1">
                <asp:Label ID="labPrice" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        
        <tr class="tr_bai">
            <td class="style1">
                提交人
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style1">
                <asp:Label ID="labUserName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td class="style1">
                提交时间
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style1">
                <asp:Label ID="labAddDate" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <div class="tr">
        >>订单列表
    </div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
      <ContentTemplate>
         
      
    <div class="tab-content" style="height: 360px; overflow: auto;">
        <div>
            <asp:Repeater ID="gvPOP" runat="server">
                <HeaderTemplate>
                    <table class="table1">
                        <tr class="tr_hui">
                            <td style="width: 40px;">
                                序号
                            </td>
                            <td>
                                店铺编号
                            </td>
                            <td>
                                店铺名称
                            </td>
                            <td>
                                区域
                            </td>
                            <td>
                                省份
                            </td>
                            <td>
                                城市
                            </td>
                            <td>
                                城市级别
                            </td>
                            <td>
                                位置
                            </td>
                            <td>
                                性别
                            </td>
                            <td>
                                数量
                            </td>
                            <td>
                                POP宽(mm)
                            </td>
                            <td>
                                POP高(mm)
                            </td>
                            <td>
                                面积(M2)
                            </td>
                            <td>
                                POP材质
                            </td>
                            <td>
                                单价
                            </td>
                            <td>
                                选图
                            </td>
                            <td>
                                位置描述
                            </td>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="tr_bai">
                        <td style="width: 40px;">
                            <%#(AspNetPager1.CurrentPageIndex-1)*AspNetPager1.PageSize+ Container.ItemIndex + 1%>
                        </td>
                        <td>
                            <%#Eval("shop.ShopNo")%>
                        </td>
                        <td>
                            <%#Eval("shop.ShopName")%>
                        </td>
                        <td>
                            <%#Eval("shop.RegionName")%>
                        </td>
                        <td>
                            <%#Eval("shop.ProvinceName")%>
                        </td>
                        <td>
                            <%#Eval("shop.CityName")%>
                        </td>
                        <td>
                            <%#Eval("shop.CityTier")%>
                        </td>
                        <td>
                            <%#Eval("order.Sheet")%>
                        </td>
                        <td>
                            <%#Eval("order.Gender")%>
                        </td>
                        <td>
                            <%#Eval("order.Quantity")%>
                        </td>
                        <td>
                            <%#Eval("order.GraphicWidth")%>
                        </td>
                        <td>
                            <%#Eval("order.GraphicLength")%>
                        </td>
                        <td>
                            <%#Eval("order.Area")%>
                        </td>
                        <td>
                            <%#Eval("order.GraphicMaterial")%>
                        </td>
                        <td>
                            <%#Eval("order.UnitPrice")%>
                        </td>
                        <td>
                            <%#Eval("order.ChooseImg")%>
                        </td>
                        <td>
                            <%#Eval("order.PositionDescription")%>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
    <div style="text-align: center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" pagesize="10" cssclass="paginator"
            currentpagebuttonclass="cpb" alwaysshow="True" firstpagetext="首页" lastpagetext="尾页"
            nextpagetext="下一页" prevpagetext="上一页" showcustominfosection="Left" showinputbox="Never"
            custominfotextalign="Left" layouttype="Table" onpagechanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    </ContentTemplate>
    </asp:UpdatePanel>
    <br />
     <div class="tr">
        >>审批信息</div>
    <table class="table">
      <tr class="tr_bai">
        <td style="width:100px;">审批结果</td>
        <td style=" text-align:left; padding-left:5px;">
            <asp:RadioButtonList ID="rblApproveResult" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
              <asp:ListItem Value="1">通过 </asp:ListItem>
              <asp:ListItem Value="2">不通过 </asp:ListItem>
            </asp:RadioButtonList>
        </td>
      </tr>
      <tr class="tr_bai">
        <td>审批意见</td>
        <td style=" text-align:left; padding-left:5px; height:80px;">
            <asp:TextBox ID="txtRemark" runat="server" Columns="60" Rows="5" TextMode="MultiLine" MaxLength="100"></asp:TextBox>
            (100字以内)
        </td>
      </tr>
    </table>
    <br />
    <div style="text-align: center;">
        <asp:Button ID="btnSubmit" runat="server" Text="提 交" OnClientClick="return Check()" class="easyui-linkbutton" style="width: 65px;
            height: 26px; font-size:13px;" onclick="btnSubmit_Click"/>
       
         &nbsp;&nbsp;
      <asp:Button ID="btnGoBack" runat="server" Text="返 回"  class="easyui-linkbutton" style="width: 65px;
            height: 26px; font-size:13px;" onclick="btnGoBack_Click"/>
         
    </div>
    <br />
    <br />
    </form>
</body>
</html>
<script type="text/javascript">
    $(function () {


        $("#txtRemark").on("keyup", function () {
            var val = $(this).val();
            if (val.length > 100) {
                $(this).val(val.substring(0, 100));
            }
        })
    })


    function Check() {
        var result = $("input:radio[name='rblApproveResult']:checked").val() || 0;
        if (result == 0) {
            alert("请选择审批结果");
            return false;
        }
        if (result == 2) {
            if ($.trim($("#txtRemark").val()) == "") {
                alert("请填写审批意见");
                return false;
            }
        }
        return confirm("确定提交吗？");
    }
</script>
