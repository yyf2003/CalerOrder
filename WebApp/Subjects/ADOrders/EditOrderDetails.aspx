<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditOrderDetails.aspx.cs"
    Inherits="WebApp.Subjects.ADOrders.EditOrderDetails" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="../../easyui1.4/jquery.easyui.min.js"></script>
    <link href="../../bootstrap-3.3.2-dist/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <script src="../../easyui1.4/jquery.min.js" type="text/javascript"></script>
    <script src="../../bootstrap-3.3.2-dist/js/bootstrap.min.js" type="text/javascript"></script>
    <style type="text/css">
        .title
        {
            font-size: 15px;
            font-weight: bold;
        }
        input[type='text']
        {
            height: 23px;
            vertical-align: top;
        }
        ul li:hover
        {
            text-decoration: underline;
            cursor: pointer;
            color: Blue;
        }
    </style>
    <script type="text/javascript">
        function closeLoading() {
            alert("ok");

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            修改订单
        </p>
    </div>
    <div class="tr">>>项目信息
    </div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                所属客户
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labCustomer" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目编号
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <table class="table1" style="width: 100%;">
        <tr class="tr_hui">
            <td style="width: 100px;">
                修改类型：
            </td>
            <td style="padding-left: 5px; text-align: left;">
                <asp:RadioButtonList ID="rblEditType" runat="server" RepeatDirection="Horizontal"
                    RepeatLayout="Flow" AutoPostBack="true" OnSelectedIndexChanged="rblEditType_SelectedIndexChanged">
                    <asp:ListItem Value="1" Selected="True">原始订单&nbsp;</asp:ListItem>
                    <asp:ListItem Value="2">最终订单 </asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
    </table>
    <div class="tab" style="margin-top: 10px;">
        <asp:Panel ID="Panel1" runat="server">
            <span style="font-weight: bold;">原始订单</span>
            <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
            <ul id="myTab" class="nav nav-tabs" style="background-color: #dce0e9;">
                <li class="active"><a href="#list" data-toggle="tab">List订单</a></li>
                <li><a href="#pop" data-toggle="tab">POP订单</a></li>
                <li><a href="#buchong" data-toggle="tab">补充订单</a></li>
                <li><a href="#merge" data-toggle="tab">合并订单</a></li>
            </ul>
            <div id="myTabContent" class="tab-content">
                <div class="tab-pane fade in active" id="list" style="padding: 5px;">
                    <table>
                        <tr>
                            <td style="height: 30px;">
                                店铺编号：<asp:TextBox ID="txtListShopNo" MaxLength="20" runat="server" Style="width: 100px;"></asp:TextBox>&nbsp;&nbsp;
                                位置：
                                <asp:CheckBoxList ID="cblListSheet" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                </asp:CheckBoxList>
                                &nbsp;&nbsp; 区域：
                                <asp:CheckBoxList ID="cblListRegion" runat="server" RepeatDirection="Horizontal"
                                    RepeatLayout="Flow">
                                </asp:CheckBoxList>
                                &nbsp;&nbsp; 城市级别：
                                <asp:CheckBoxList ID="cblListCityTier" runat="server" RepeatDirection="Horizontal"
                                    RepeatLayout="Flow">
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                        <tr>
                            <td style="height: 30px;">
                                <asp:Button ID="btnSreachList" runat="server" Text="搜 索" class="easyui-linkbutton ExportMerge"
                                    Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnSreachList_Click" />
                                &nbsp;&nbsp;
                                <asp:Button ID="btnDeleteList" runat="server" Text="删 除" class="easyui-linkbutton ExportMerge"
                                    Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnDeleteList_Click" />
                                &nbsp;&nbsp;
                                <asp:Button ID="brnReconverList" runat="server" Text="恢 复" class="easyui-linkbutton ExportMerge"
                                    Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="brnReconverList_Click" />
                                &nbsp;&nbsp;
                                <asp:Button ID="btnSaveListEdit" runat="server" Text="提交修改" class="easyui-linkbutton ExportMerge"
                                    Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnSaveListEdit_Click" />
                                &nbsp;&nbsp;
                                <input type="button" value="批量导入" id="btnImportList" class="easyui-linkbutton ExportMerge"
                                    style="width: 65px; height: 26px; margin-bottom: 5px;" />
                            </td>
                        </tr>
                    </table>
                    <div id="divContent" style="overflow: auto;">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <asp:Repeater ID="gvList" runat="server" OnItemDataBound="gvList_ItemDataBound">
                                    <HeaderTemplate>
                                        <table class="table1" id="gvList" style="width: 1500px;">
                                            <tr class="tr_hui">
                                                <td style="width: 30px;">
                                                    <asp:CheckBox ID="cbListAll" runat="server" />
                                                </td>
                                                <td style="width: 30px;">
                                                    序号
                                                </td>
                                                <td style="width: 90px;">
                                                    店铺编号
                                                </td>
                                                <td>
                                                    店铺名称
                                                </td>
                                                <td style="width: 80px;">
                                                    区域
                                                </td>
                                                <td style="width: 80px;">
                                                    省份
                                                </td>
                                                <td style="width: 80px;">
                                                    城市
                                                </td>
                                                <td style="width: 80px;">
                                                    城市级别
                                                </td>
                                                <td style="width: 80px;">
                                                    物料支持
                                                </td>
                                                <td style="width: 80px;">
                                                    店铺规模大小
                                                </td>
                                                <td style="width: 80px;">
                                                    角落类型
                                                </td>
                                                <td style="width: 80px;">
                                                    位置
                                                </td>
                                                <td style="width: 80px;">
                                                    级别
                                                </td>
                                                <td style="width: 60px;">
                                                    性别
                                                </td>
                                                <td style="width: 60px;">
                                                    数量
                                                </td>
                                                <td style="width: 80px;">
                                                    选图
                                                </td>
                                                <td style="width: 120px;">
                                                    备注
                                                </td>
                                                <td style="width: 60px;">
                                                    状态
                                                </td>
                                            </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td style="width: 30px;">
                                                <asp:CheckBox ID="cbListOne" runat="server" />
                                                <asp:HiddenField ID="hfListId" runat="server" Value='<%#Eval("list.Id") %>' />
                                            </td>
                                            <td style="width: 30px;">
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
                                                <asp:TextBox ID="txtListMaterialSupport" runat="server" Text=' <%#Eval("list.MaterialSupport")%>'
                                                    MaxLength="50" Style="width: 95%; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtListPOSScale" runat="server" Text=' <%#Eval("list.POSScale")%>'
                                                    MaxLength="50" Style="width: 95%; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtListCornerType" runat="server" Text=' <%#Eval("list.CornerType")%>'
                                                    MaxLength="30" Style="width: 95%; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtListSheet" runat="server" Text=' <%#Eval("list.Sheet")%>' MaxLength="20"
                                                    Style="width: 95%; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlListLevelNum" runat="server" Style="height: 23px;">
                                                    <asp:ListItem Value="0">请选择</asp:ListItem>
                                                    <asp:ListItem Value="1">1</asp:ListItem>
                                                    <asp:ListItem Value="2">2</asp:ListItem>
                                                    <asp:ListItem Value="3">3</asp:ListItem>
                                                    <asp:ListItem Value="4">4</asp:ListItem>
                                                    <asp:ListItem Value="5">5</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtListGender" runat="server" Text=' <%#Eval("list.Gender")%>' MaxLength="10"
                                                    Style="width: 95%; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtListQuantity" runat="server" Text=' <%#Eval("list.Quantity")%>'
                                                    MaxLength="2" Style="width: 95%; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtListChooseImg" runat="server" Text=' <%#Eval("list.ChooseImg")%>'
                                                    MaxLength="30" Style="width: 95%; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtListRemark" runat="server" Text=' <%#Eval("list.Remark")%>' MaxLength="50"
                                                    Style="width: 95%; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <%#Eval("list.IsDelete") != null && bool.Parse(Eval("list.IsDelete").ToString())==true?"<span style='color:red;'>已删除</span>":"正常"%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <%if (gvList.Items.Count == 0)
                                          { %>
                                        <tr>
                                            <td colspan="18" style="text-align: center; height: 30px;">
                                                --无数据--
                                            </td>
                                        </tr>
                                        <%} %>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                                <div style="text-align: center;">
                                    <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="10" CssClass="paginator"
                                        CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                        NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                                        CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
                                    </webdiyer:AspNetPager>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnSreachList" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="btnDeleteList" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="btnSaveListEdit" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="btnAddList" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="brnReconverList" EventName="click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                    <table class="table1" name="addTb">
                        <tr class="tr_hui">
                            <td style="width: 50px;">
                            </td>
                            <td>
                                店铺编号
                            </td>
                            <td>
                                物料支持
                            </td>
                            <td>
                                店铺规模
                            </td>
                            <td>
                                位置
                            </td>
                            <td>
                                级别
                            </td>
                            <td>
                                角落类型
                            </td>
                            <td>
                                性别
                            </td>
                            <td>
                                数量
                            </td>
                            <td>
                                选图
                            </td>
                            <td>
                                备注
                            </td>
                            <td style="width: 70px;">
                            </td>
                        </tr>
                        <tr class="tr_bai">
                            <td style="height: 35px;">
                                新增：
                            </td>
                            <td>
                                <asp:TextBox ID="txtAddListPOSCode" runat="server" MaxLength="20" Style="width: 90%;
                                    text-align: center;"></asp:TextBox>
                            </td>
                            <td>
                                <div style="position: relative;">
                                    <asp:TextBox ID="txtAddListMaterialSupport" class="showDll" runat="server" MaxLength="30"
                                        Style="width: 90%; text-align: center;"></asp:TextBox>
                                    <div id="divListMaterialSupport" style="display: none; position: absolute; background-color: White;
                                        border: 1px solid #ccc; padding-top: 2px; height: 100px; width: 90%; overflow: auto;">
                                        <ul id="ddlListMaterialSupport" name="MaterialSupport" style="margin-top: 0; width: 90%;
                                            margin-left: 0px; list-style: none;">
                                        </ul>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <div style="position: relative;">
                                    <asp:TextBox ID="txtAddListPOSScale" class="showDll" runat="server" MaxLength="30"
                                        Style="width: 90%; text-align: center;"></asp:TextBox>
                                    <div id="divAddListPOSScale" style="display: none; position: absolute; background-color: White;
                                        border: 1px solid #ccc; padding-top: 2px; height: 100px; width: 90%; overflow: auto;">
                                        <ul id="ddlAddListPOSScale" name="POSScale" style="margin-top: 0; width: 90%; margin-left: 0px;
                                            list-style: none;">
                                        </ul>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <div style="position: relative;">
                                    <asp:TextBox ID="txtAddListSheet" class="showDll" runat="server" MaxLength="20" Style="width: 90%;
                                        text-align: center;"></asp:TextBox>
                                    <div id="divAddListSheet" style="display: none; position: absolute; background-color: White;
                                        border: 1px solid #ccc; padding-top: 2px; height: 100px; width: 90%; overflow: auto;">
                                        <ul id="ddlAddListSheet" name="Sheet" style="margin-top: 0; width: 90%; margin-left: 0px;
                                            list-style: none;">
                                        </ul>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <div style="position: relative;">
                                    <%--<asp:TextBox ID="txtAddListLevelNum" class="showDll" Enabled="false" runat="server" MaxLength="30"
                                    Style="width: 90%; text-align: center;"></asp:TextBox>
                                <div id="divAddListLevelNum" style="display: none; position: absolute; background-color: White;
                                    border: 1px solid #ccc; padding-top: 2px; height: 100px; width: 90%; overflow: auto;">
                                    <ul id="ddlAddListLevelNum" name="LevelNum" style="margin-top: 0; width: 90%; margin-left: 0px;
                                        list-style: none;">
                                    </ul>
                                </div>--%>
                                    <asp:DropDownList ID="ddlAddListLevelNum"  runat="server" Style="height: 23px;">
                                        <asp:ListItem Value="0">请选择</asp:ListItem>
                                        <asp:ListItem Value="1">1</asp:ListItem>
                                        <asp:ListItem Value="2">2</asp:ListItem>
                                        <asp:ListItem Value="3">3</asp:ListItem>
                                        <asp:ListItem Value="4">4</asp:ListItem>
                                        <asp:ListItem Value="5">5</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </td>
                            <td>
                                <asp:TextBox ID="txtAddListCornerType" runat="server" MaxLength="20" Style="width: 90%;
                                    text-align: center;"></asp:TextBox>
                            </td>
                            <td>
                                <div style="position: relative;">
                                    <asp:TextBox ID="txtAddListGender" class="showDll" runat="server" MaxLength="10"
                                        Style="width: 90%; text-align: center;"></asp:TextBox>
                                    <div id="divAddListGender" style="display: none; position: absolute; background-color: White;
                                        border: 1px solid #ccc; padding-top: 2px; height: 100px; width: 90%; overflow: auto;">
                                        <ul id="ddlAddListGender" name="Gender" style="margin-top: 0; width: 90%; margin-left: 0px;
                                            list-style: none;">
                                        </ul>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <asp:TextBox ID="txtAddListQuantity" name="addQuantity" Text="1" runat="server" MaxLength="2"
                                    Style="width: 90%; text-align: center;"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtAddListChooseImg" runat="server" MaxLength="30" Style="width: 90%;
                                    text-align: center;"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtAddListRemarks" runat="server" MaxLength="50" Style="width: 90%;
                                    text-align: center;"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Button ID="btnAddList" runat="server" Text="提交" class="easyui-linkbutton ExportMerge"
                                    Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnAddList_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="tab-pane fade" id="pop" style="padding: 5px;">
                    <table>
                        <tr>
                            <td style="height: 30px;">
                                店铺编号：<asp:TextBox ID="txtPOPShopNo" MaxLength="20" runat="server" Style="width: 100px;"></asp:TextBox>&nbsp;&nbsp;
                                位置：
                                <asp:CheckBoxList ID="cblPOPSheet" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                </asp:CheckBoxList>
                                &nbsp;&nbsp; 区域：
                                <asp:CheckBoxList ID="cblPOPRegion" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                </asp:CheckBoxList>
                                &nbsp;&nbsp; 城市级别：
                                <asp:CheckBoxList ID="cblPOPCityTier" runat="server" RepeatDirection="Horizontal"
                                    RepeatLayout="Flow">
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                        <tr>
                            <td style="height: 30px;">
                                <asp:Button ID="btnSreachPOP" runat="server" Text="搜 索" class="easyui-linkbutton"
                                    Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnSreachPOP_Click" />
                                &nbsp;&nbsp;
                                <asp:Button ID="btnDeletePOP" runat="server" Text="删 除" class="easyui-linkbutton"
                                    Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnDeletePOP_Click" />
                                &nbsp;&nbsp;
                                <asp:Button ID="btnReconverPOP" runat="server" Text="恢 复" class="easyui-linkbutton"
                                    Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnReconverPOP_Click" />
                                &nbsp;&nbsp;
                                <asp:Button ID="btnSavePOPEdit" runat="server" Text="提交修改" class="easyui-linkbutton"
                                    Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnSavePOPEdit_Click" />
                                &nbsp;&nbsp;
                                <input type="button" value="批量导入" id="Button5" class="easyui-linkbutton" style="width: 65px;
                                    height: 26px; margin-bottom: 5px;" />
                            </td>
                        </tr>
                    </table>
                    <div id="div1" style="overflow: auto;">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <asp:Repeater ID="gvPOP" runat="server" OnItemDataBound="gvPOP_ItemDataBound">
                                    <HeaderTemplate>
                                        <table class="table1" id="gvPOP" style="width: 3800px;">
                                            <tr class="tr_hui">
                                                <td style="width: 30px;">
                                                    <asp:CheckBox ID="cbPOPAll" runat="server" />
                                                </td>
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
                                                    物料支持
                                                </td>
                                                <td>
                                                    店铺规模大小
                                                </td>
                                                <td>
                                                    POP编号
                                                </td>
                                                <td>
                                                    POP名称
                                                </td>
                                                <td>
                                                    POP类型
                                                </td>
                                                <td>
                                                    性别
                                                </td>
                                                <td>
                                                    数量
                                                </td>
                                                <td>
                                                    位置
                                                </td>
                                                <td>
                                                    位置描述
                                                </td>
                                                <td>
                                                    位置宽(mm)
                                                </td>
                                                <td>
                                                    位置高(mm)
                                                </td>
                                                <td>
                                                    位置深(mm)
                                                </td>
                                                <td>
                                                    位置规模
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
                                                    样式
                                                </td>
                                                <td>
                                                    角落类型
                                                </td>
                                                <td>
                                                    系列
                                                </td>
                                                <td>
                                                    是否标准规格
                                                </td>
                                                <td>
                                                    是否格栅
                                                </td>
                                                <td>
                                                    是否框架
                                                </td>
                                                <td>
                                                    单/双面
                                                </td>
                                                <td>
                                                    是否有玻璃
                                                </td>
                                                <td>
                                                    背景
                                                </td>
                                                <td>
                                                    格栅横向数量
                                                </td>
                                                <td>
                                                    格栅纵向数量
                                                </td>
                                                <td>
                                                    平台长(mm)
                                                </td>
                                                <td>
                                                    平台宽(mm)
                                                </td>
                                                <td>
                                                    平台高(mm)
                                                </td>
                                                <td>
                                                    设备类别
                                                </td>
                                                <td>
                                                    选图
                                                </td>
                                                <td>
                                                    备注
                                                </td>
                                                <td>
                                                    状态
                                                </td>
                                            </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr class="tr_bai">
                                            <td style="width: 30px;">
                                                <asp:CheckBox ID="cbPOPOne" runat="server" />
                                                <asp:HiddenField ID="hfPOPId" runat="server" Value='<%#Eval("Order.Id") %>' />
                                                <asp:HiddenField ID="hfShopNo" runat="server" Value='<%#Eval("shop.ShopNo") %>' />
                                            </td>
                                            <td style="width: 40px;">
                                                <%#(AspNetPager2.CurrentPageIndex-1)*AspNetPager2.PageSize+ Container.ItemIndex + 1%>
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
                                                <asp:TextBox ID="txtPOPMaterialSupport" runat="server" Text=' <%#Eval("order.MaterialSupport")%>'
                                                    MaxLength="50" Style="width: 95%; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPOPPOSScale" runat="server" Text=' <%#Eval("order.POSScale")%>'
                                                    MaxLength="50" Style="width: 95%; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPOPGraphicNo" runat="server" Text=' <%#Eval("order.GraphicNo")%>'
                                                    MaxLength="50" Style="width: 90%; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <%#Eval("pop.POPName")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.POPType")%>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPOPGender" runat="server" Text=' <%#Eval("order.Gender")%>' MaxLength="50"
                                                    Style="width: 80px; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPOPQuantity" runat="server" Text=' <%#Eval("order.Quantity")%>'
                                                    MaxLength="50" Style="width: 60px; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPOPSheet" runat="server" Text=' <%#Eval("order.Sheet")%>' MaxLength="50"
                                                    Style="width: 90%; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPOPPositionDescription" runat="server" Text=' <%#Eval("pop.PositionDescription")%>'
                                                    MaxLength="50" Style="width: 90%; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <%#Eval("pop.WindowWide")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.WindowHigh")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.WindowDeep")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.WindowSize")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.GraphicWidth")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.GraphicLength")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.Area")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.GraphicMaterial")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.Style")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.CornerType")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.Category")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.StandardDimension")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.Modula")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.Frame")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.DoubleFace")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.Glass")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.Backdrop")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.ModulaQuantityWidth")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.ModulaQuantityHeight")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.PlatformLength")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.PlatformWidth")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.PlatformHeight")%>
                                            </td>
                                            <td>
                                                <%#Eval("pop.FixtureType")%>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPOPChooseImg" runat="server" Text=' <%#Eval("order.ChooseImg")%>'
                                                    MaxLength="50" Style="width: 90%; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPOPRemark" runat="server" Text=' <%#Eval("order.Remark")%>' MaxLength="50"
                                                    Style="width: 90%; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <%#Eval("Order.IsDelete") != null && bool.Parse(Eval("Order.IsDelete").ToString()) == true ? "<span style='color:red;'>已删除</span>" : "正常"%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <%if (gvPOP.Items.Count == 0)
                                          { %>
                                        <tr>
                                            <td colspan="30" style="text-align: center; height: 30px;">
                                                --无数据--
                                            </td>
                                        </tr>
                                        <%} %>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                                <div style="text-align: center;">
                                    <webdiyer:AspNetPager ID="AspNetPager2" runat="server" PageSize="10" CssClass="paginator"
                                        CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                        NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                                        CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager2_PageChanged">
                                    </webdiyer:AspNetPager>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnSreachPOP" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="btnDeletePOP" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="btnSavePOPEdit" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="btnAddPOP" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="btnReconverPOP" EventName="click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                    <table class="table1" name="addTb">
                        <tr class="tr_hui">
                            <td style="width: 50px;">
                            </td>
                            <td>
                                店铺编号
                            </td>
                            <td>
                                物料支持
                            </td>
                            <td>
                                店铺规模
                            </td>
                            <td>
                                POP编号
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
                                选图
                            </td>
                            <td>
                                备注
                            </td>
                            <td style="width: 70px;">
                            </td>
                        </tr>
                        <tr class="tr_bai">
                            <td style="height: 35px;">
                                新增：
                            </td>
                            <td>
                                <asp:TextBox ID="txtAddPOPPOSCode" runat="server" MaxLength="20" Style="width: 90%;
                                    text-align: center; vertical-align: middle;"></asp:TextBox>
                            </td>
                            <td>
                                <div style="position: relative;">
                                    <asp:TextBox ID="txtAddPOPMaterialSupport" class="showDll" runat="server" MaxLength="30"
                                        Style="width: 90%; text-align: center;"></asp:TextBox>
                                    <div id="divPOPMaterialSupport" style="display: none; position: absolute; background-color: White;
                                        border: 1px solid #ccc; padding-top: 2px; height: 100px; width: 90%; overflow: auto;">
                                        <ul id="ddlPOPMaterialSupport" name="MaterialSupport" style="margin-top: 0; width: 90%;
                                            margin-left: 0px; list-style: none;">
                                        </ul>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <div style="position: relative;">
                                    <asp:TextBox ID="txtAddPOPPOSScale" class="showDll" runat="server" MaxLength="30"
                                        Style="width: 90%; text-align: center;"></asp:TextBox>
                                    <div id="divAddPOPPOSScale" style="display: none; position: absolute; background-color: White;
                                        border: 1px solid #ccc; padding-top: 2px; height: 100px; width: 90%; overflow: auto;">
                                        <ul id="ddlAddPOPPOSScale" name="POSScale" style="margin-top: 0; width: 90%; margin-left: 0px;
                                            list-style: none;">
                                        </ul>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <asp:TextBox ID="txtAddPOPGraphicNo" runat="server" MaxLength="20" Style="width: 90%;
                                    text-align: center;"></asp:TextBox>
                            </td>
                            <td>
                                <div style="position: relative;">
                                    <asp:TextBox ID="txtAddPOPSheet" class="showDll" runat="server" MaxLength="20" Style="width: 90%;
                                        text-align: center;"></asp:TextBox>
                                    <div id="divAddPOPSheet" style="display: none; position: absolute; background-color: White;
                                        border: 1px solid #ccc; padding-top: 2px; height: 100px; width: 90%; overflow: auto;">
                                        <ul id="ddlAddPOPSheet" name="Sheet" style="margin-top: 0; width: 90%; margin-left: 0px;
                                            list-style: none;">
                                        </ul>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <div style="position: relative;">
                                    <asp:TextBox ID="txtAddPOPGender" class="showDll" runat="server" MaxLength="20" Style="width: 90%;
                                        text-align: center;"></asp:TextBox>
                                    <div id="divAddPOPGender" style="display: none; position: absolute; background-color: White;
                                        border: 1px solid #ccc; padding-top: 2px; height: 100px; width: 90%; overflow: auto;">
                                        <ul id="ddlAddPOPGender" name="Gender" style="margin-top: 0; width: 90%; margin-left: 0px;
                                            list-style: none;">
                                        </ul>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <asp:TextBox ID="txtAddPOPQuantity" name="addQuantity" runat="server" Text="1" MaxLength="10"
                                    Style="width: 90%; text-align: center;"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtAddPOPChooseImg" runat="server" MaxLength="2" Style="width: 90%;
                                    text-align: center;"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtAddPOPRemarks" runat="server" MaxLength="30" Style="width: 90%;
                                    text-align: center;"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Button ID="btnAddPOP" runat="server" Text="提交" class="easyui-linkbutton" Style="width: 65px;
                                    height: 26px; margin-bottom: 5px;" OnClick="btnAddPOP_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="tab-pane fade" id="buchong" style="padding: 5px; overflow: auto;">
                    <table>
                        <tr>
                            <td style="height: 30px;">
                                店铺编号：<asp:TextBox ID="txtBCShopNo" MaxLength="20" runat="server" Style="width: 100px;"></asp:TextBox>&nbsp;&nbsp;
                                位置：
                                <asp:CheckBoxList ID="cblBCSheet" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                </asp:CheckBoxList>
                                &nbsp;&nbsp; 区域：
                                <asp:CheckBoxList ID="cblBCRegion" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                </asp:CheckBoxList>
                                &nbsp;&nbsp; 城市级别：
                                <asp:CheckBoxList ID="cblBCCityTier" runat="server" RepeatDirection="Horizontal"
                                    RepeatLayout="Flow">
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                        <tr>
                            <td style="height: 30px;">
                                <asp:Button ID="btnSreachBC" runat="server" Text="搜 索" class="easyui-linkbutton"
                                    Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnSreachBC_Click" />
                                &nbsp;&nbsp;
                                <asp:Button ID="btnDeleteBC" runat="server" Text="删 除" class="easyui-linkbutton"
                                    Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnDeleteBC_Click" />
                                &nbsp;&nbsp;
                                <asp:Button ID="btnReconverBC" runat="server" Text="恢 复" class="easyui-linkbutton"
                                    Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnReconverBC_Click" />
                                &nbsp;&nbsp;
                                <asp:Button ID="btnSaveBCEdit" runat="server" Text="提交修改" class="easyui-linkbutton"
                                    Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnSaveBCEdit_Click" />
                                &nbsp;&nbsp;
                                <input type="button" value="批量导入" id="Button6" class="easyui-linkbutton" style="width: 65px;
                                    height: 26px; margin-bottom: 5px;" />
                            </td>
                        </tr>
                    </table>
                    <div id="div3" class="tab-content" style="overflow: auto;">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <asp:Repeater ID="gvSupplement" runat="server">
                                    <HeaderTemplate>
                                        <table class="table1" id="gvBC" style="width: 100%;">
                                            <tr class="tr_hui">
                                                <td style="width: 30px;">
                                                    <asp:CheckBox ID="cbBCAll" runat="server" />
                                                </td>
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
                                                    选图
                                                </td>
                                                <td>
                                                    备注
                                                </td>
                                                <td>
                                                    状态
                                                </td>
                                            </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td style="width: 30px;">
                                                <asp:CheckBox ID="cbBCOne" runat="server" />
                                                <asp:HiddenField ID="hfBCId" runat="server" Value='<%#Eval("list.Id") %>' />
                                            </td>
                                            <td style="width: 40px;">
                                                <%#(AspNetPager3.CurrentPageIndex-1)*AspNetPager3.PageSize+ Container.ItemIndex + 1%>
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
                                                <asp:TextBox ID="txtBCSheet" runat="server" Text=' <%#Eval("list.Sheet")%>' MaxLength="50"
                                                    Style="width: 95%; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtBCGender" runat="server" Text=' <%#Eval("list.Gender")%>' MaxLength="50"
                                                    Style="width: 90px; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtBCQuantity" runat="server" Text=' <%#Eval("list.Quantity")%>'
                                                    MaxLength="50" Style="width: 90px; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtBCChooseImg" runat="server" Text=' <%#Eval("list.ChooseImg")%>'
                                                    MaxLength="50" Style="width: 90%; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtBCRemark" runat="server" Text=' <%#Eval("list.Remark")%>' MaxLength="50"
                                                    Style="width: 90%; text-align: center;"></asp:TextBox>
                                            </td>
                                            <td>
                                                <%#Eval("list.IsDelete") != null && bool.Parse(Eval("list.IsDelete").ToString()) == true ? "<span style='color:red;'>已删除</span>" : "正常"%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <%if (gvSupplement.Items.Count == 0)
                                          { %>
                                        <tr>
                                            <td colspan="30" style="text-align: center; height: 30px;">
                                                --无数据--
                                            </td>
                                        </tr>
                                        <%} %>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                                <div style="text-align: center;">
                                    <webdiyer:AspNetPager ID="AspNetPager3" runat="server" PageSize="10" CssClass="paginator"
                                        CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                        NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                                        CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager3_PageChanged">
                                    </webdiyer:AspNetPager>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnSreachBC" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="btnDeleteBC" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="btnSaveBCEdit" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="btnAddBC" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="btnReconverBC" EventName="click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                    <table class="table1" name="addTb">
                        <tr class="tr_hui">
                            <td style="width: 50px;">
                            </td>
                            <td>
                                店铺编号
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
                                选图
                            </td>
                            <td>
                                备注
                            </td>
                            <td style="width: 70px;">
                            </td>
                        </tr>
                        <tr class="tr_bai">
                            <td style="height: 35px;">
                                新增：
                            </td>
                            <td>
                                <asp:TextBox ID="txtAddBCPOSCode" runat="server" MaxLength="20" Style="width: 90%;
                                    text-align: center;"></asp:TextBox>
                            </td>
                            <td>
                                <div style="position: relative;">
                                    <asp:TextBox ID="txtAddBCSheet" class="showDll" runat="server" MaxLength="30" Style="width: 90%;
                                        text-align: center;"></asp:TextBox>
                                    <div id="divAddBCSheet" style="display: none; position: absolute; background-color: White;
                                        border: 1px solid #ccc; padding-top: 2px; height: 100px; width: 90%; overflow: auto;">
                                        <ul id="ddlAddBCSheet" name="Sheet" style="margin-top: 0; width: 90%; margin-left: 0px;
                                            list-style: none;">
                                        </ul>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <div style="position: relative;">
                                    <asp:TextBox ID="txtAddBCGender" class="showDll" runat="server" MaxLength="30" Style="width: 90%;
                                        text-align: center;"></asp:TextBox>
                                    <div id="divAddBCGender" style="display: none; position: absolute; background-color: White;
                                        border: 1px solid #ccc; padding-top: 2px; height: 100px; width: 90%; overflow: auto;">
                                        <ul id="ddlAddBCGender" name="Gender" style="margin-top: 0; width: 90%; margin-left: 0px;
                                            list-style: none;">
                                        </ul>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <asp:TextBox ID="txtAddBCQuantity" name="addQuantity" Text="1" runat="server" MaxLength="20"
                                    Style="width: 90%; text-align: center;"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtAddBCChooseImg" runat="server" MaxLength="20" Style="width: 90%;
                                    text-align: center;"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtAddBCRemarks" runat="server" MaxLength="20" Style="width: 90%;
                                    text-align: center;"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Button ID="btnAddBC" runat="server" Text="提交" class="easyui-linkbutton" Style="width: 65px;
                                    height: 26px; margin-bottom: 5px;" OnClick="btnAddBC_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="tab-pane fade" id="merge" style="padding: 5px;">
                    <table>
                        <tr>
                            <td style="height: 30px;">
                                店铺编号：<asp:TextBox ID="txtMergeShopNo" MaxLength="20" runat="server" Style="width: 100px;"></asp:TextBox>&nbsp;&nbsp;
                                位置：
                                <asp:CheckBoxList ID="cblMergeSheet" runat="server" RepeatDirection="Horizontal"
                                    RepeatLayout="Flow">
                                </asp:CheckBoxList>
                                &nbsp;&nbsp; 区域：
                                <asp:CheckBoxList ID="cblMergeRegion" runat="server" RepeatDirection="Horizontal"
                                    RepeatLayout="Flow">
                                </asp:CheckBoxList>
                                &nbsp;&nbsp; 城市级别：
                                <asp:CheckBoxList ID="cblMergeCityTier" runat="server" RepeatDirection="Horizontal"
                                    RepeatLayout="Flow">
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                        <tr>
                            <td style="height: 30px;">
                                <asp:Button ID="btnSreachMerge" runat="server" Text="搜 索" class="easyui-linkbutton"
                                    Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnSreachMerge_Click" />
                                &nbsp;&nbsp;
                                <asp:Button ID="btnDeleteMerge" runat="server" Text="删 除" class="easyui-linkbutton"
                                    Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnDeleteMerge_Click" />
                                &nbsp;&nbsp;
                                <asp:Button ID="btnReconverMerge" runat="server" Text="恢 复" class="easyui-linkbutton"
                                    Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnReconverMerge_Click" />
                                &nbsp;&nbsp;
                                <asp:Button ID="btnReMerge" runat="server" Text="重新生成合并订单" class="easyui-linkbutton opBtn"
                                    Style="width: 120px; height: 26px; margin-bottom: 5px;" OnClick="btnReMerge_Click" />
                            </td>
                        </tr>
                    </table>
                    <div id="div2" class="tab-content" style="overflow: auto;">
                        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                            <ContentTemplate>
                                <asp:Repeater ID="gvMerge" runat="server" OnItemDataBound="gvMerge_ItemDataBound">
                                    <HeaderTemplate>
                                        <table class="table1" id="gvMerge" style="width: 3000px;">
                                            <tr class="tr_hui">
                                                <td style="width: 30px;">
                                                    <asp:CheckBox ID="cbMergeAll" runat="server" />
                                                </td>
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
                                                    物料支持
                                                </td>
                                                <td>
                                                    店铺规模大小
                                                </td>
                                                <td>
                                                    POP编号
                                                </td>
                                                <td>
                                                    POP名称
                                                </td>
                                                <td>
                                                    POP类型
                                                </td>
                                                <td>
                                                    性别
                                                </td>
                                                <td>
                                                    数量
                                                </td>
                                                <td>
                                                    位置
                                                </td>
                                                <td>
                                                    级别
                                                </td>
                                                <td>
                                                    位置描述
                                                </td>
                                                <td>
                                                    位置宽(mm)
                                                </td>
                                                <td>
                                                    位置高(mm)
                                                </td>
                                                <td>
                                                    位置深(mm)
                                                </td>
                                                <td>
                                                    位置规模
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
                                                    样式
                                                </td>
                                                <td>
                                                    角落类型
                                                </td>
                                                <td>
                                                    系列
                                                </td>
                                                <td>
                                                    是否标准规格
                                                </td>
                                                <td>
                                                    是否格栅
                                                </td>
                                                <td>
                                                    是否框架
                                                </td>
                                                <td>
                                                    单/双面
                                                </td>
                                                <td>
                                                    是否有玻璃
                                                </td>
                                                <td>
                                                    背景
                                                </td>
                                                <td>
                                                    格栅横向数量
                                                </td>
                                                <td>
                                                    格栅纵向数量
                                                </td>
                                                <td>
                                                    平台长
                                                </td>
                                                <td>
                                                    平台宽
                                                </td>
                                                <td>
                                                    平台高
                                                </td>
                                                <td>
                                                    设备类别
                                                </td>
                                                <td>
                                                    选图
                                                </td>
                                                <td>
                                                    备注
                                                </td>
                                                <td>
                                                    状态
                                                </td>
                                            </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr class="tr_bai">
                                            <td>
                                                <asp:CheckBox ID="cbMergeOne" runat="server" />
                                                <asp:HiddenField ID="hfMergeId" runat="server" Value='<%#Eval("merge.Id") %>' />
                                            </td>
                                            <td style="width: 40px;">
                                                <%#(AspNetPager4.CurrentPageIndex-1)*AspNetPager4.PageSize+ Container.ItemIndex + 1%>
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
                                                <%#Eval("merge.MaterialSupport")%>
                                            </td>
                                            <td>
                                                <%#Eval("merge.POSScale")%>
                                            </td>
                                            <td>
                                                <%#Eval("GraphicNo")%>
                                            </td>
                                            <td>
                                                <%#Eval("POPName")%>
                                            </td>
                                            <td>
                                                <%#Eval("POPType")%>
                                            </td>
                                            <td>
                                                <%#Eval("merge.Gender")%>
                                            </td>
                                            <td>
                                                <%#Eval("merge.Quantity")%>
                                            </td>
                                            <td>
                                                <%#Eval("merge.Sheet")%>
                                            </td>
                                            <td>
                                               <%-- <asp:Label ID="labLevelNum" runat="server" Text=""></asp:Label>--%>
                                                <%#Eval("merge.LevelNum")%>
                                            </td>
                                            <td>
                                                <%#Eval("PositionDescription")%>
                                            </td>
                                            <td>
                                                <%#Eval("WindowWide")%>
                                            </td>
                                            <td>
                                                <%#Eval("WindowHigh")%>
                                            </td>
                                            <td>
                                                <%#Eval("WindowDeep")%>
                                            </td>
                                            <td>
                                                <%#Eval("WindowSize")%>
                                            </td>
                                            <td>
                                                <%#Eval("GraphicWidth")%>
                                            </td>
                                            <td>
                                                <%#Eval("GraphicLength")%>
                                            </td>
                                            <td>
                                                <%#Eval("Area")%>
                                            </td>
                                            <td>
                                                <%#Eval("GraphicMaterial")%>
                                            </td>
                                            <td>
                                                <%#Eval("Style")%>
                                            </td>
                                            <td>
                                                <%#Eval("CornerType")%>
                                            </td>
                                            <td>
                                                <%#Eval("Category")%>
                                            </td>
                                            <td>
                                                <%#Eval("StandardDimension")%>
                                            </td>
                                            <td>
                                                <%#Eval("Modula")%>
                                            </td>
                                            <td>
                                                <%#Eval("Frame")%>
                                            </td>
                                            <td>
                                                <%#Eval("DoubleFace")%>
                                            </td>
                                            <td>
                                                <%#Eval("Glass")%>
                                            </td>
                                            <td>
                                                <%#Eval("Backdrop")%>
                                            </td>
                                            <td>
                                                <%#Eval("ModulaQuantityWidth")%>
                                            </td>
                                            <td>
                                                <%#Eval("ModulaQuantityHeight")%>
                                            </td>
                                            <td>
                                                <%#Eval("PlatformLength")%>
                                            </td>
                                            <td>
                                                <%#Eval("PlatformWidth")%>
                                            </td>
                                            <td>
                                                <%#Eval("PlatformHeight")%>
                                            </td>
                                            <td>
                                                <%#Eval("FixtureType")%>
                                            </td>
                                            <td>
                                                <%#Eval("merge.ChooseImg")%>
                                            </td>
                                            <td>
                                                <%#Eval("merge.Remark")%>
                                            </td>
                                            <td>
                                                <%#Eval("merge.IsDelete") != null && bool.Parse(Eval("merge.IsDelete").ToString()) == true ? "<span style='color:red;'>已删除</span>" : "正常"%>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <%if (gvMerge.Items.Count == 0)
                                          { %>
                                        <tr>
                                            <td colspan="50" style="text-align: center; height: 30px;">
                                                --无数据--
                                            </td>
                                        </tr>
                                        <%} %>
                                        </table>
                                    </FooterTemplate>
                                </asp:Repeater>
                                <div style="text-align: center;">
                                    <webdiyer:AspNetPager ID="AspNetPager4" runat="server" PageSize="10" CssClass="paginator"
                                        CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                                        NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                                        CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager4_PageChanged">
                                    </webdiyer:AspNetPager>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnSreachMerge" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="btnDeleteMerge" EventName="click" />
                                <%--<asp:AsyncPostBackTrigger ControlID="btnSaveBCEdit" EventName="click" />--%>
                                <asp:AsyncPostBackTrigger ControlID="btnReconverMerge" EventName="click" />
                                <asp:AsyncPostBackTrigger ControlID="btnReMerge" EventName="click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="Panel2" runat="server">
            <span style="font-weight: bold;">最终订单</span>
            <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
            <table>
                <tr>
                    <td style="height: 30px;">
                        店铺编号：<asp:TextBox ID="txtShopNoF" MaxLength="20" runat="server" Style="width: 100px;"></asp:TextBox>&nbsp;&nbsp;
                    </td>
                </tr>
                <tr>
                    <td style="height: 30px;">
                        <asp:Button ID="btnSreachF" runat="server" Text="搜 索" class="easyui-linkbutton ExportMerge"
                            Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnSreachF_Click" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnDeleteF" runat="server" Text="删 除" class="easyui-linkbutton ExportMerge"
                            Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnDeleteF_Click" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnReconverF" runat="server" Text="恢 复" class="easyui-linkbutton ExportMerge"
                            Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnReconverF_Click" />
                        &nbsp;&nbsp;
                        <asp:Button ID="btnSaveEditF" runat="server" Text="提交修改" class="easyui-linkbutton ExportMerge"
                            Style="width: 65px; height: 26px; margin-bottom: 5px;" OnClick="btnSaveEditF_Click" />
                    </td>
                </tr>
            </table>
            <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                <ContentTemplate>
                    <div class="tab" style="overflow: auto;">
                        <asp:Repeater ID="gvFinalOrder" runat="server" 
                            onitemdatabound="gvFinalOrder_ItemDataBound">
                            <HeaderTemplate>
                                <%if (gvFinalOrder.Items.Count == 0)
                                  { %>
                                <table class="table" id="gvFinalOrder">
                                    <%}
                                  else
                                  {%>
                                    <table class="table1" id="gvFinalOrder" style="width: 1800px;">
                                        <tr class="tr_hui">
                                            <td style="width: 30px;">
                                                <asp:CheckBox ID="cbAllF" runat="server" />
                                            </td>
                                            <td style="width: 40px;">
                                                序号
                                            </td>
                                            <td style="width: 50px;">
                                                订单类型
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
                                                店铺类型
                                            </td>
                                           
                                            <td>
                                                位置
                                            </td>
                                            <td>
                                                级别
                                            </td>
                                            <td>
                                                位置描述
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
                                                选图
                                            </td>
                                            <td>
                                                备注
                                            </td>
                                            <td>
                                                状态
                                            </td>
                                        </tr>
                                        <%} %>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr runat="server" id="trId" class="tr_bai">
                                    <td style="width: 30px;">
                                        <asp:CheckBox ID="cbOneF" runat="server" />
                                        <asp:HiddenField ID="hfFinalOrderId" runat="server" Value='<%#Eval("order.Id") %>' />
                                    </td>
                                    <td style="width: 40px;">
                                        <%#(AspNetPager5.CurrentPageIndex-1)*AspNetPager5.PageSize+ Container.ItemIndex + 1%>
                                    </td>
                                    <td style="width: 50px;">
                                        <%#Eval("order.OrderType") != null ? Eval("order.OrderType").ToString() == "1" ? "pop" : "道具" : "POP"%>
                                    </td>
                                    <td>
                                        <%--店铺编号--%>
                                        <%#Eval("shop.ShopNo")%>
                                    </td>
                                    <td>
                                        <%--店铺名称--%>
                                        <%#Eval("shop.ShopName")%>
                                    </td>
                                    <td>
                                        <%--区域--%>
                                        <%#Eval("shop.RegionName")%>
                                    </td>
                                    <td>
                                        <%--省份--%>
                                        <%#Eval("shop.ProvinceName")%>
                                    </td>
                                    <td>
                                        <%--城市--%>
                                        <%#Eval("shop.CityName")%>
                                    </td>
                                    <td>
                                        <%--城市级别--%>
                                        <%#Eval("shop.CityTier")%>
                                    </td>
                                    <td>
                                        <%#Eval("shop.Format")%>
                                    </td>
                                    
                                    <td>
                                        <%--位置--%>
                                        <asp:TextBox ID="txtSheetF" runat="server" Text='<%#Eval("order.Sheet")%>' MaxLength="20"
                                                    Style="width: 90px; text-align: center;"></asp:TextBox>
                                       
                                    </td>
                                    <td>
                                        <%--级别--%>
                                       <asp:TextBox ID="txtLevelNumF" runat="server" Text='<%#Eval("order.LevelNum")%>' MaxLength="1"
                                                    Style="width: 90px; text-align: center;"></asp:TextBox>
                                    </td>
                                    <td>
                                        <%--位置描述--%>
                                        <asp:TextBox ID="txtPositionDescriptionF" runat="server" Text='<%#Eval("order.PositionDescription")%>' MaxLength="50"
                                                    Style="width: 90px; text-align: center;"></asp:TextBox>
                                        
                                    </td>
                                    
                                    <td>
                                        <%--性别--%>
                                        <asp:TextBox ID="txtGenderF" runat="server" Text='<%#Eval("order.Gender")%>' MaxLength="10"
                                                    Style="width: 90px; text-align: center;"></asp:TextBox>
                                       
                                    </td>
                                    <td>
                                        <%--数量--%>
                                        <asp:TextBox ID="txtQuantityF" runat="server" Text='<%#Eval("order.Quantity")%>' MaxLength="10"
                                                    Style="width: 90px; text-align: center;"></asp:TextBox>
                                        
                                    </td>
                                    <td>
                                        <%--POP宽(mm)--%>
                                        <asp:TextBox ID="txtGraphicWidthF" runat="server" Text='<%#Eval("order.GraphicWidth")%>' MaxLength="10"
                                                    Style="width: 90px; text-align: center;"></asp:TextBox>
                                        
                                    </td>
                                    <td>
                                        <%-- POP高(mm)--%>
                                        <asp:TextBox ID="txtGraphicLengthF" runat="server" Text='<%#Eval("order.GraphicLength")%>' MaxLength="10"
                                                    Style="width: 90px; text-align: center;"></asp:TextBox>
                                        
                                    </td>
                                    <td>
                                        <%--面积(M2)--%>
                                        <%#Eval("order.Area")%>
                                    </td>
                                    <td>
                                        <%--POP材质--%>
                                        <%#Eval("order.GraphicMaterial")%>
                                    </td>
                                    <td>
                                        <%--选图--%>
                                        <asp:TextBox ID="txtChooseImgF" runat="server" Text='<%#Eval("order.ChooseImg")%>' MaxLength="50"
                                                    Style="width: 90px; text-align: center;"></asp:TextBox>
                                       
                                    </td>
                                    <td>
                                        <%--备注--%>
                                         <asp:TextBox ID="txtRemarkF" runat="server" Text='<%#Eval("order.Remark")%>' MaxLength="50"
                                                    Style="width: 90px; text-align: center;"></asp:TextBox>
                                       
                                    </td>
                                    <td>
                                       <%#Eval("order.IsDelete") != null ? bool.Parse(Eval("order.IsDelete").ToString()) ? "已删除" : "正常" : "正常"%>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                <%if (gvFinalOrder.Items.Count == 0)
                                  { %>
                                <tr>
                                    <td>
                                        --无数据--
                                    </td>
                                </tr>
                                <%} %>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                    <div class="tab" style="text-align: center;">
                        <webdiyer:AspNetPager ID="AspNetPager5" runat="server" PageSize="20" CssClass="paginator"
                            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                            CustomInfoTextAlign="Left" LayoutType="Table">
                        </webdiyer:AspNetPager>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="AspNetPager5" EventName="pagechanged" />
                    <asp:AsyncPostBackTrigger ControlID="btnSreachF" EventName="click" />
                    <asp:AsyncPostBackTrigger ControlID="btnDeleteF" EventName="click" />
                    <asp:AsyncPostBackTrigger ControlID="btnSaveEditF" EventName="click" />
                    <asp:AsyncPostBackTrigger ControlID="btnReconverF" EventName="click" />
                </Triggers>
            </asp:UpdatePanel>
        </asp:Panel>
    </div>
    <div style="height: 200px;">
    </div>
    </form>
</body>
</html>
<script src="../js/EditOrderDetails.js" type="text/javascript"></script>
<script type="text/javascript">
    var requestEleId;
    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (sender, e) {

        $("#gvList").find("input[name$='cbListAll']").on("change", function () {
            var checked = this.checked;
            $("#gvList").find("input[name$='cbListOne']").each(function () {
                this.checked = checked;
            })
        })
        $("#gvPOP").find("input[name$='cbPOPAll']").on("change", function () {
            var checked = this.checked;
            $("#gvPOP").find("input[name$='cbPOPOne']").each(function () {
                this.checked = checked;
            })
        })
        $("#gvBC").find("input[name$='cbBCAll']").on("change", function () {
            var checked = this.checked;
            $("#gvBC").find("input[name$='cbBCOne']").each(function () {
                this.checked = checked;
            })
        })

        $("#gvMerge").find("input[name$='cbMergeAll']").on("change", function () {
            var checked = this.checked;
            $("#gvMerge").find("input[name$='cbMergeOne']").each(function () {
                this.checked = checked;
            })
        })

        $("#gvFinalOrder").find("input[name$='cbAllF']").on("change", function () {

            var checked = this.checked;

            $("#gvFinalOrder").find("input[name$='cbOneF']").each(function () {
                this.checked = checked;
            })
        })


        //var id = e.get_postBackElement().id;
        requestEleId = requestEleId || "";
        if (requestEleId != "" && requestEleId.indexOf("btnAdd") != -1) {
            $("table[name='addTb']").find("input[type='text']").val("");
            $("input[name='addQuantity']").val("1");
        }
        //重新合并订单完成后
        if (requestEleId != "" && requestEleId.indexOf("btnReMerge") != -1) {

            var img = $(".imgLoading");
            img.css("color", "blue").html("操作成功");
            $(".opBtn").removeAttr("disabled");
            setInterval(function () {
                img.remove();
            }, 3000);
        }

    })
    Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(function (sender, e) {
        var id = requestEleId = e.get_postBackElement().id;
        if (id.indexOf("btnAddList") != -1) {
            if (!CheckAddList()) {
                e.set_cancel(true);
            }
        }
        if (id.indexOf("btnAddPOP") != -1) {
            if (!CheckAddPOP()) {
                e.set_cancel(true);
            }
        }
        if (id.indexOf("btnAddBC") != -1) {
            if (!CheckAddSupplement()) {
                e.set_cancel(true);
            }
        }
        //list提示
        if (id.indexOf("btnDeleteList") != -1 || id.indexOf("brnReconverList") != -1 || id.indexOf("btnSaveListEdit") != -1) {
            var len = $("input[name$='cbListOne']:checked").length;
            if (len == 0) {
                alert("请选择订单");
                e.set_cancel(true);
            }
            else {
                if (id.indexOf("btnDeleteList") != -1) {
                    if (!confirm("确定要删除嘛？")) {
                        e.set_cancel(true);
                    }
                }
                else if (id.indexOf("brnReconverList") != -1) {
                    if (!confirm("确定要撤销删除嘛？")) {
                        e.set_cancel(true);
                    }
                }
            }

        }

        //POP提示
        if (id.indexOf("btnDeletePOP") != -1 || id.indexOf("btnReconverPOP") != -1 || id.indexOf("btnSavePOPEdit") != -1) {
            var len = $("input[name$='cbPOPOne']:checked").length;
            if (len == 0) {
                alert("请选择订单");
                e.set_cancel(true);
            }
            else {
                if (id.indexOf("btnDeletePOP") != -1) {
                    if (!confirm("确定要删除嘛？")) {
                        e.set_cancel(true);
                    }
                }
                else if (id.indexOf("btnReconverPOP") != -1) {
                    if (!confirm("确定要撤销删除嘛？")) {
                        e.set_cancel(true);
                    }
                }
            }
        }

        //补充订单提示
        if (id.indexOf("btnDeleteBC") != -1 || id.indexOf("btnReconverBC") != -1 || id.indexOf("btnSaveBCEdit") != -1) {
            var len = $("input[name$='cbBCOne']:checked").length;
            if (len == 0) {
                alert("请选择订单");
                e.set_cancel(true);
            }
            else {
                if (id.indexOf("btnDeleteBC") != -1) {
                    if (!confirm("确定要删除嘛？")) {
                        e.set_cancel(true);
                    }
                }
                else if (id.indexOf("btnReconverBC") != -1) {
                    if (!confirm("确定要撤销删除嘛？")) {
                        e.set_cancel(true);
                    }
                }
            }
        }

        //合并订单提示
        if (id.indexOf("btnDeleteMerge") != -1 || id.indexOf("btnReconverMerge") != -1) {
            var len = $("input[name$='cbMergeOne']:checked").length;
            if (len == 0) {
                alert("请选择订单");
                e.set_cancel(true);
            }
            else {
                if (id.indexOf("btnDeleteMerge") != -1) {
                    if (!confirm("确定要删除嘛？")) {
                        e.set_cancel(true);
                    }
                }
                else if (id.indexOf("btnReconverMerge") != -1) {
                    if (!confirm("确定要撤销删除嘛？")) {
                        e.set_cancel(true);
                    }
                }
            }
        }

        //最终订单提示
        if (id.indexOf("btnDeleteF") != -1 || id.indexOf("btnReconverF") != -1) {
            var len = $("input[name$='cbOneF']:checked").length;
            if (len == 0) {
                alert("请选择订单");
                e.set_cancel(true);
            }
            else {
                if (id.indexOf("btnDeleteF") != -1) {
                    if (!confirm("确定要删除嘛？")) {
                        e.set_cancel(true);
                    }
                }
                else if (id.indexOf("btnReconverF") != -1) {
                    if (!confirm("确定要撤销删除嘛？")) {
                        e.set_cancel(true);
                    }
                }
            }
        }


        //重新合并订单
        if (id.indexOf("btnReMerge") != -1) {
            var div = "<span class='imgLoading'><img  src='/image/WaitImg/loadingA.gif'/></span>";
            //$(obj)
            var btn = e.get_postBackElement();
            $(btn).attr("disabled", "disabled").after(div);
        }
    })
</script>
