<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Subjects.InstallPrice.Edit" %>

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
    <script type="text/javascript">
        var guidanceId = '<%=guidanceId %>';
        function FinishEdit() {
            $("#hfIsFinishEdit").val("1");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div style=" min-height:600px;">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" />
        <p class="nav_table_p">
            提交安装费
            <asp:Label ID="labTitle" runat="server" Text="" style=" font-weight:normal;"></asp:Label>
        </p>
    </div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table class="table">
                <tr class="tr_bai">
                    <td style="width: 120px; ">
                        活动名称：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:Label ID="labGuidanceName" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td style="width: 120px; ">
                        是否二次安装：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:CheckBox ID="cbIsSecondInstall" runat="server" />是
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td style="width: 120px;">
                        区域：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:CheckBoxList ID="cblRegion" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"
                            OnSelectedIndexChanged="cblRegion_SelectedIndexChanged" AutoPostBack="true">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        项目类型：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <img name="loadImg" id="imgLoadSubjectType" style=" display:none;" src="../../image/WaitImg/loadingA.gif" />
                        <asp:CheckBoxList ID="cblSubjectType" runat="server" RepeatDirection="Horizontal"
                            RepeatLayout="Flow" OnSelectedIndexChanged="cblSubjectType_SelectedIndexChanged"
                            AutoPostBack="true">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        项目名称：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <div name="loadImg" id="imgLoadSubjects" style=" display:none;">
                          <img src="../../image/WaitImg/loadingA.gif" />
                        </div>
                        
                        <asp:CheckBox ID="cbSubjects" runat="server" />全选
                        <hr style=" width:50px;color:Blue;"/>
                        <asp:CheckBoxList ID="cblSubjectName" runat="server" RepeatDirection="Horizontal"
                            RepeatLayout="Flow" RepeatColumns="5" OnSelectedIndexChanged="cblSubjectName_SelectedIndexChanged"
                            AutoPostBack="true">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <%--<tr class="tr_bai">
                    <td>
                        补单项目名称：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                       <asp:CheckBoxList ID="cblSupplimentSubject" runat="server" 
                            RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="6" 
                            onselectedindexchanged="cblSupplimentSubject_SelectedIndexChanged" AutoPostBack="true">
                        </asp:CheckBoxList>
                    </td>
                </tr>--%>
                <tr class="tr_bai">
                    <td>
                        店铺规模大小：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <div name="loadImg" id="imgLoadPOSScale" style=" display:none;">
                          <img src="../../image/WaitImg/loadingA.gif" />
                        </div>
                       
                        <asp:CheckBox ID="cbPOSScale" runat="server" />全选
                        <hr style=" width:50px;color:Blue;"/>
                        <asp:CheckBoxList ID="cblPOSScale" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"
                            OnSelectedIndexChanged="cblPOSScale_SelectedIndexChanged" AutoPostBack="true">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        选择店铺数量：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <div name="loadImg" id="imgLoadSelectShopCount" style=" display:none;">
                          <img src="../../image/WaitImg/loadingA.gif" />
                        </div>
                        <asp:Label ID="labShopCount" runat="server" Text=""></asp:Label>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        剩余店铺数量：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:Label ID="labTotalShopCount" runat="server" Text="0"></asp:Label>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        归类到：
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:DropDownList ID="ddlSubject" runat="server">
                            <asp:ListItem Value="0">--请选择--</asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button ID="btnSubmit" runat="server" Text="提 交" Style="width: 65px; height: 26px;
                            margin-left: 20px;" OnClick="btnSubmit_Click"/>
                        <img name="loadImg" id="imgWaitting" style=" display:none;" src="../../image/WaitImg/loadingA.gif" />
                    </td>
                </tr>
            </table>
            <div class="tr" style="margin-top: 15px;">
                》信息列表：
            </div>
            <asp:Repeater ID="RepeaterList" runat="server" 
                onitemcommand="RepeaterList_ItemCommand" 
                onitemdatabound="RepeaterList_ItemDataBound">
                <HeaderTemplate>
                    <table class="table">
                        <tr class="tr_hui">
                            <td style="width: 50px;">
                                序号
                            </td>
                            <td>
                                项目名称
                            </td>
                            <td>
                                区域
                            </td>
                            <td>
                                项目分类
                            </td>
                            <td>
                                店铺规模大小
                            </td>
                            <td>
                                店铺数量
                            </td>
                            <td>
                                安装费
                            </td>
                            <td style="width: 70px;">
                                操作
                            </td>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="tr_bai">
                        <td>
                            <%#Container.ItemIndex + 1 + (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize%>
                        </td>
                        <td>
                            <%#Eval("SubjectName") %>
                        </td>
                        <td>
                            <%#Eval("detail.SelectRegion")%>
                        </td>
                        <td>
                            <asp:Label ID="labSubjectTypeName" runat="server" Text=""></asp:Label>
                        </td>
                        <td>
                            <%--<%#Eval("detail.SelectPOSScale")%>--%>
                            <asp:Label ID="labSelectPOSScale" runat="server" Text=""></asp:Label>
                        </td>
                        <td>
                           <%-- <%#Eval("detail.ShopCount")%>--%>
                            <asp:Label ID="labShopCount" runat="server" Text=""></asp:Label>
                        </td>
                        <td>
                            <%----%>
                            <span name="spanCheckShop" data-detailid='<%#Eval("detail.Id") %>' style=" cursor:pointer; text-decoration:underline;color:Blue;">
                            <%--<%#Eval("detail.Price")%>--%>
                             <asp:Label ID="labPrice" runat="server" Text=""></asp:Label>
                            </span>
                        </td>
                        <td>
                            <asp:LinkButton ID="lbDelete" CommandArgument='<%#Eval("detail.Id") %>' CommandName="DeleteItem" runat="server" ForeColor="Red">删除</asp:LinkButton>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%if (RepeaterList.Items.Count == 0)
                      {%>
                    <tr class="tr_bai">
                        <td colspan="8" style="text-align: center;">
                          --暂无数据--
                        </td>
                    </tr>
                    <%} %>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            <div style="text-align: center;">
                <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
                    CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                    NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                    CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
                </webdiyer:AspNetPager>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>


    <div style=" margin-top :20px; text-align:center; margin-bottom :30px;">
        <asp:Button ID="btnBack" runat="server" Text="返 回"  class="easyui-linkbutton" Style="width: 65px; height: 26px;" onclick="btnBack_Click"/>
    </div>
    </div>
    <asp:HiddenField ID="hfIsFinishEdit" runat="server" />
    <asp:HiddenField ID="hfIsCheckEmpty" runat="server" />
    </form>
</body>
</html>
<link href="/layer/skin/default/layer.css" rel="stylesheet" type="text/css" />
<script src="/layer/layer.js" type="text/javascript"></script>

<script src="js/edit.js" type="text/javascript"></script>
<script type="text/javascript">
    
</script>