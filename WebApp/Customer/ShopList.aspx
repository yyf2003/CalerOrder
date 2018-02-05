<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShopList.aspx.cs" Inherits="WebApp.Customer.ShopList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="/easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="/fancyBox/source/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <link href="/easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="/easyui1.4/jquery.min.js"></script>
    <script src="/easyui1.4/jquery.easyui.min.js"></script>
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var url = '<%=url %>';
        function FinishImport() {
            $("#hfIsFinishImport").val("1");
        }
        
    </script>
    <style type="text/css">
        #editDiv li
        {
            margin-bottom: 2px;
            height: 20px;
            font-size: 14px;
            cursor: pointer;
            padding-left: 5px;
            color: Blue;
        }
        #editDiv li:hover
        {
            background-color: #f0f1f2;
            text-decoration: underline;
        }
        .conditionTr
        {
            display: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <a href="/index.aspx">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /></a><p
                class="nav_table_p">
                店铺基础数据
            </p>
    </div>
    <div class="tr">
        >>搜索
    </div>
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table class="table">
                    <tr class="tr_bai conditionTr">
                        <td style="width: 150px;">
                            客户名称
                        </td>
                        <td style="width: 200px; text-align: left; padding-left: 5px;">
                            <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td style="width: 150px;">
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                        </td>
                    </tr>
                    <tr class="tr_bai conditionTr">
                        <td>
                            区域
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
                            <div id="loadRegion" style="display: none;">
                                <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblRegion" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"
                                OnSelectedIndexChanged="cblRegion_SelectedIndexChanged" AutoPostBack="true">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai conditionTr">
                        <td>
                            省份
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
                            <div id="loadProvince" style="display: none;">
                                <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblProvince" runat="server" RepeatDirection="Horizontal" RepeatColumns="8"
                                RepeatLayout="Flow" OnSelectedIndexChanged="cblProvince_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai conditionTr">
                        <td>
                            城市级别
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
                            <div id="loadCityTier" style="display: none;">
                                <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblCityTier" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"
                                OnSelectedIndexChanged="cblCityTier_SelectedIndexChanged" AutoPostBack="true">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai conditionTr">
                        <td>
                            Channel
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
                            <div id="loadChannel" style="display: none;">
                                <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblChannel" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"
                                OnSelectedIndexChanged="cblChannel_SelectedIndexChanged" AutoPostBack="true">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai conditionTr">
                        <td>
                            Format
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
                            <div id="loadFormat" style="display: none;">
                                <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblFormat" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"
                                RepeatColumns="8" OnSelectedIndexChanged="cblFormat_SelectedIndexChanged" AutoPostBack="true">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai conditionTr">
                        <td>
                            是否安装店铺
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
                            <div id="loadIsInstall" style="display: none;">
                                <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblIsInstall" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"
                                OnSelectedIndexChanged="cblIsInstall_SelectedIndexChanged">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai conditionTr">
                        <td>
                            渠道
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
                            <div id="loadShopType" style="display: none;">
                                <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblShopType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai conditionTr">
                        <td>
                            客服
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
                            <div id="loadCS" style="display: none;">
                                <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblCS" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            店铺编号
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:TextBox ID="txtShopNo" runat="server"></asp:TextBox>
                        </td>
                        <td>
                            店铺名称
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:TextBox ID="txtShopName" runat="server" Style="width: 200px;"></asp:TextBox>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            有无外协
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:CheckBoxList ID="cblOutsourceState" runat="server" RepeatDirection="Horizontal"
                                RepeatLayout="Flow">
                                <asp:ListItem Value="1">有分配外协&nbsp;&nbsp;</asp:ListItem>
                                <asp:ListItem Value="2">没分配外协</asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                        <td>
                            外协名称
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:DropDownList ID="ddlOutsource" runat="server">
                                <asp:ListItem Value="0">--请选择外协--</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td colspan="4" style="padding-right: 10px; text-align: center; height: 30px;">
                            <span id="spanShowConditions" style="color: #3399FF; cursor: pointer; font-size: 13px;">
                                展开选项</span>
                            <asp:HiddenField ID="hfIsShowConditions" runat="server" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div style="text-align: right; padding-right: 25px; margin-top: 8px;">
            <asp:Button ID="btnSearch" runat="server" Text="查 询" OnClick="btnSearch_Click" OnClientClick="return loadSearch()"
                class="easyui-linkbutton" Style="width: 65px; height: 26px;" />
            <img id="loadingSearch" style="display: none;" src="../image/WaitImg/loadingA.gif" />
            &nbsp;&nbsp;
            <asp:Button ID="btnAdd" runat="server" Text="批量导入" OnClientClick="return false" Visible="false"
                class="easyui-linkbutton" Style="width: 65px; height: 26px;" />
            &nbsp;&nbsp;
            <asp:Button ID="btnExportShop" runat="server" Text="导出店铺信息" Visible="false" class="easyui-linkbutton"
                Style="width: 90px; height: 26px;" OnClick="btnExportShop_Click" OnClientClick="return loading1()" />
            <img id="loadingImg1" style="display: none;" src="../image/WaitImg/loadingA.gif" />
            &nbsp;&nbsp;
            <asp:Button ID="btnExportShopAndPOP" runat="server" Text="导出店铺+POP" Visible="false"
                class="easyui-linkbutton" Style="width: 100px; height: 26px;" OnClick="btnExportShopAndPOP_Click"
                OnClientClick="return loading2()" />
            <img id="loadingImg2" style="display: none;" src="../image/WaitImg/loadingA.gif" />
            &nbsp;&nbsp;
            <asp:Button ID="btnExportShopAndFrame" runat="server" Text="导出店铺+器架" Visible="false"
                class="easyui-linkbutton" Style="width: 100px; height: 26px;" OnClick="btnExportShopAndFrame_Click"
                OnClientClick="return loading3()" />
            <img id="loadingImg3" style="display: none;" src="../image/WaitImg/loadingA.gif" />
        </div>
    </div>
    <br />
    <div class="tr">
        >>信息列表
    </div>
    <div>
        <div id="toolbar">
            <a id="btnAddNewShop" onclick="AddNewShop()" style="float: left; display: none;"
                class="easyui-linkbutton" plain="true" icon="icon-add">添加</a> <a id="btnCheckEditLog"
                    onclick="ShowEditLog()" style="float: left; display: none;" class="easyui-linkbutton"
                    plain="true" icon="icon-tip">修改记录</a>
        </div>
        <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" CellPadding="0"
            CssClass="table" BorderWidth="0" HeaderStyle-BorderStyle="None" EmptyDataText="--无符合条件的信息--"
            OnRowCommand="gv_RowCommand" OnRowDataBound="gv_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="序号" HeaderStyle-Width="40px" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                    </ItemTemplate>
                    <HeaderStyle Width="40px"></HeaderStyle>
                </asp:TemplateField>
                <asp:BoundField DataField="CustomerName" HeaderText="所属客户" HeaderStyle-BorderColor="#dce0e9" />
                <asp:TemplateField HeaderText="店铺编码" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <span name="checkShop" data-shopid='<%#Eval("Id") %>' style="color: Blue; cursor: pointer;
                            text-decoration: underline;">
                            <%#Eval("ShopNo")%>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <%--<asp:BoundField DataField="ShopNo" HeaderText="店铺编码" HeaderStyle-BorderColor="#dce0e9" />--%>
                <asp:BoundField DataField="ShopName" HeaderText="店铺名称" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="RegionName" HeaderText="区域" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="ProvinceName" HeaderText="省份" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="CityName" HeaderText="城市" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="AreaName" HeaderText="区/县" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="CityTier" HeaderText="城市级别" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="IsInstall" HeaderText="是否安装" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="Channel" HeaderText="Channel" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="Format" HeaderText="Format" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="ShopType" HeaderText="渠道" HeaderStyle-BorderColor="#dce0e9" />
                <asp:TemplateField HeaderText="店铺状态" Visible="false" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <%#Eval("Status")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="CSUserName" HeaderText="客服" HeaderStyle-BorderColor="#dce0e9" />
                <asp:TemplateField HeaderText="POP信息" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <span name="checkPOP" data-shopid='<%#Eval("Id") %>' style="color: Blue; cursor: pointer;">
                            查看</span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="器架信息" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <span name="checkMachineFrame" data-shopid='<%#Eval("Id") %>' style="color: Blue;
                            cursor: pointer;">查看</span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="状态" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:Label ID="labState" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="查看" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <span name="checkShop" data-shopid='<%#Eval("Id") %>' style="color: Blue; cursor: pointer;">
                            查看</span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="编辑" Visible="false" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <span name="editShop" data-shopid='<%#Eval("Id") %>' style="color: Blue; cursor: pointer;">
                            编辑</span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="操作" Visible="false" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbDelete" CommandArgument='<%#Eval("Id") %>' CommandName="DeleteItem"
                            runat="server">删除</asp:LinkButton>
                        <%--<asp:LinkButton ID="lbRealDelete" CommandArgument='<%#Eval("Id") %>' CommandName="RealDelete"
                            runat="server" ForeColor="Red" OnClientClick="return confirm('警告：删除后将不能恢复！继续删除吗？')">彻底删除</asp:LinkButton>--%>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <AlternatingRowStyle CssClass="tr_bai" />
            <HeaderStyle CssClass="tr_hui" />
            <RowStyle CssClass="tr_bai" />
            <SelectedRowStyle CssClass="tr_hui" />
            <EmptyDataRowStyle CssClass="tr_bai" />
        </asp:GridView>
    </div>
    <br />
    <div style="text-align: center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    <asp:HiddenField ID="hfIsFinishImport" runat="server" />
    <div id="checkDiv" title="查看店铺信息" style="display: none;">
        <table class="table" style="width: 806px; text-align: center;">
            <tr>
                <td style="width: 100px; height: 30px;">
                    所属客户
                </td>
                <td style="text-align: left; width: 180px; padding-left: 5px;">
                    <asp:Label ID="labClientName" runat="server" Text=""></asp:Label>
                </td>
                <td style="width: 120px; height: 30px;">
                    区域
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labRegion" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    省份
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labProvince" runat="server" Text=""></asp:Label>
                </td>
                <td style="height: 30px;">
                    城市
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labCity" runat="server" Text=""></asp:Label>
                    &nbsp;&nbsp;&nbsp;&nbsp; 区/县：
                    <asp:Label ID="labArea" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    店铺编码
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labPOSCode" runat="server" Text=""></asp:Label>
                </td>
                <td style="height: 30px;">
                    店铺名称
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labPOSName" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    经销商编号
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labCustomerCode" runat="server" Text=""></asp:Label>
                </td>
                <td style="height: 30px;">
                    经销商名称
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labCustomerName" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    城市级别
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labCityTier" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    大货安装级别
                </td>
                <td style="text-align: left; padding-left: 5px; width: 100px;">
                    <asp:Label ID="labInstall" runat="server" Text=""></asp:Label>
                </td>
                <td style="width: 120px;">
                    三叶草安装级别
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labBCSInstall" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    联系人1
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labContact1" runat="server" Text=""></asp:Label>
                </td>
                <td style="height: 30px;">
                    联系人电话1
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labTel1" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    联系人2
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labContact2" runat="server" Text=""></asp:Label>
                </td>
                <td style="height: 30px;">
                    联系人电话2
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labTel2" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    Channel
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labChannel" runat="server" Text=""></asp:Label>
                </td>
                <td style="height: 30px;">
                    Format
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labFormat" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    LocationType
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labLocationType" runat="server" Text=""></asp:Label>
                </td>
                <td style="height: 30px;">
                    BusinessModel
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labBusinessModel" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    开店日期
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labOpenDate" runat="server" Text=""></asp:Label>
                </td>
                <td style="height: 30px;">
                    店铺状态
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labStatus" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    客服
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labCSUserName" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    渠道
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labShopType" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    店铺地址
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labPOSAddress" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    SP特殊安装费
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labBasicInstallPrice" runat="server" Text=""></asp:Label>
                </td>
                <td style="height: 30px;">
                    三叶草特殊安装费
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labBCSInstallPrice" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    SP外协安装费
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labOutsourceInstallPrice" runat="server" Text=""></asp:Label>
                </td>
                <td style="height: 30px;">
                    三叶草外协安装费
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labOutsourceBCSInstallPrice" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    主外协名称
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labOutsourceName" runat="server" Text=""></asp:Label>
                </td>
                <td style="height: 30px;">
                    OOH安装外协
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labOOHOutsourceName" runat="server" Text=""></asp:Label>
                </td>
                <td style="height: 30px;">
                    三叶草外协
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   <asp:Label ID="labBCSOutsourceName" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    备注
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labRemark" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px;">
                    <asp:Button ID="btnAddCheckChangeLog" runat="server" Text="查看修改记录" Visible="false"
                        class="easyui-linkbutton" Style="width: 90px; height: 26px;" />
                </td>
            </tr>
        </table>
    </div>
    <div id="editDiv" title="编辑店铺信息" style="display: none;">
        <table class="table" style="width: 856px; text-align: center;">
            <tr>
                <td style="width: 100px; height: 30px;">
                    所属客户
                </td>
                <td style="text-align: left; width: 200px; padding-left: 5px;">
                    <select id="selCustomer">
                        <option value="0">--请选择--</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
                <td style="width: 120px; height: 30px;">
                    区域
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <select id="seleRegion" style="min-width: 120px;">
                        <option value="0">--请选择--</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    省份
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="seleProvince" style="min-width: 120px;">
                        <option value="0">--请选择--</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
                <td style="height: 30px;">
                    城市
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <select id="seleCity" style="min-width: 120px;">
                        <option value="0">--请选择--</option>
                    </select>
                    <span style="color: Red;">*</span> &nbsp;区/县：
                    <select id="seleArea" style="min-width: 120px;">
                        <option value="0">--请选择--</option>
                    </select>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    店铺编码
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPOSCode" runat="server" MaxLength="50"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
                <td style="height: 30px;">
                    店铺名称
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPOSName" runat="server" MaxLength="50" Style="width: 220px;"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    经销商编号
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtAgentNo" runat="server" MaxLength="20"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    经销商名称
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtAgentName" runat="server" MaxLength="50" Style="width: 220px;"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    城市级别
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <%--<asp:TextBox ID="txtCityTier" runat="server" MaxLength="10"></asp:TextBox>--%>
                    <select id="seleCityTier" style="min-width: 120px;">
                        <option value="">--请选择--</option>
                        <option value="T1">T1</option>
                        <option value="T2">T2</option>
                        <option value="T3">T3</option>
                        <option value="T4">T4</option>
                        <option value="T5">T5</option>
                        <option value="T6">T6</option>
                        <option value="T7">T7</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
                <td style="height: 30px;">
                    大货安装级别
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <%-- <asp:TextBox ID="txtInstall" runat="server" MaxLength="10"></asp:TextBox>--%>
                    <select id="seleIsInstall" style="min-width: 120px;">
                        <option value="">--请选择--</option>
                        <option value="Y">Y</option>
                        <option value="N">N</option>
                    </select>
                </td>
                <td>
                    三叶草安装级别
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="seleBCSIsInstall" style="min-width: 120px;">
                        <option value="">--请选择--</option>
                        <option value="Y">Y</option>
                        <option value="N">N</option>
                    </select>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    联系人1
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtContact1" runat="server" MaxLength="50"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    联系人电话1
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtTel1" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    联系人2
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtContact2" runat="server" MaxLength="50"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    联系人电话2
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtTel2" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    Channel
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <div style="position: relative;">
                        <asp:TextBox ID="txtChannel" runat="server" MaxLength="50"></asp:TextBox>
                        <div id="divChannelEditMenu" style="display: none; position: absolute; width: 150px;
                            background-color: White; border: 1px solid #ccc; padding-top: 2px; z-index: 100;">
                            <ul id="ddlChannelEditMenu" style="margin-top: 0; width: 150px; margin-left: 0px;
                                list-style: none; overflow: scroll; max-height: 200px;">
                            </ul>
                        </div>
                    </div>
                </td>
                <td style="height: 30px;">
                    Format
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <div style="position: relative;">
                        <asp:TextBox ID="txtFormat" runat="server" MaxLength="50"></asp:TextBox>
                        <div id="divFormatEditMenu" style="display: none; position: absolute; width: 150px; background-color: White;
                            border: 1px solid #ccc; padding-top: 2px; z-index: 100;">
                            <ul id="ddlFormatEditMenu" style="margin-top: 0; width: 150px; margin-left: 0px; list-style: none;
                                overflow: scroll; max-height: 200px;">
                            </ul>
                        </div>
                        <span style="color: Red;">*</span>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    LocationType
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtLocationType" runat="server" MaxLength="50"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    BusinessModel
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtBusinessModel" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    开店日期
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtOpenDate" runat="server" onclick="WdatePicker()" MaxLength="20"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    店铺状态
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:RadioButtonList ID="rblStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    客服
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selCSUser">
                        <option value="0">--请选择客服--</option>
                    </select>
                </td>
                <td style="height: 30px;">
                    渠道
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtShopType" runat="server" MaxLength="30"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    店铺地址
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtAddress" runat="server" MaxLength="100" Style="width: 465px;"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    SP特殊安装费
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtBasicInstallPrice" runat="server" MaxLength="10"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    三叶草特殊安装费
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtBCSInstallPrice" runat="server" MaxLength="10"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    SP外协安装费
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtOutsourceInstallPrice" runat="server" MaxLength="10"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    三叶草外协安装费
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtOutsourceBCSInstallPrice" runat="server" MaxLength="10"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    主外协名称
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="seleOutsource">
                        <option value="0">--请选择客服--</option>
                    </select>
                </td>
                <td style="height: 30px;">
                    OOH安装外协
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="seleOOHInstallOutsource">
                        <option value="0">--请选择客服--</option>
                    </select>
                </td>
                 <td style="height: 30px;">
                    三叶草外协
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="seleBCSOutsource">
                        <option value="0">--请选择客服--</option>
                    </select>
                </td>
            </tr>
            <tr>
                <td style="height: 30px;">
                    备注
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtRemark" runat="server" MaxLength="50" Style="width: 250px;"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
<script src="../Scripts/common.js" type="text/javascript"></script>
<script src="js/shoplist.js" type="text/javascript"></script>
<script src="../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
<script src="../fancyBox/source/jquery.fancybox.js" type="text/javascript"></script>
<script src="../fancyBox/source/jquery.fancybox.pack.js" type="text/javascript"></script>
<link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
<script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
<script type="text/javascript">

    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function (sender, e) {
        var eleId = e.get_postBackElement().id;

        if (eleId.indexOf("ddlCustomer") != -1) {
            $("#loadRegion").show();

        }
        if (eleId.indexOf("cblRegion") != -1) {
            $("#loadProvince").show();
            //$("#loadCity").show();
            $("#loadCityTier").show();
            $("#loadChannel").show();
            $("#loadFormat").show();
            $("#loadShopLevel").show();
            $("#loadIsInstall").show();
            $("#loadCS").show();
        }
        if (eleId.indexOf("cblProvince") != -1) {
            //$("#loadCity").show();
            $("#loadCityTier").show();
            $("#loadChannel").show();
            $("#loadFormat").show();
            $("#loadShopLevel").show();
            $("#loadIsInstall").show();
            $("#loadCS").show();
        }

        if (eleId.indexOf("cblCity") != -1) {
            $("#loadCityTier").show();
            $("#loadChannel").show();
            $("#loadFormat").show();
            $("#loadShopLevel").show();
            $("#loadIsInstall").show();
            $("#loadCS").show();
        }

        if (eleId.indexOf("cblCityTier") != -1) {

            $("#loadChannel").show();
            $("#loadFormat").show();
            $("#loadShopLevel").show();
            $("#loadIsInstall").show();
        }

        if (eleId.indexOf("cblChannel") != -1) {
            $("#loadFormat").show();
            $("#loadShopLevel").show();
            $("#loadIsInstall").show();
        }
        if (eleId.indexOf("cblFormat") != -1) {
            $("#loadShopLevel").show();
            $("#loadIsInstall").show();
        }
        if (eleId.indexOf("cblShopLevel") != -1) {
            $("#loadIsInstall").show();
        }
        if (eleId.indexOf("cblIsInstall") != -1) {
            $("#loadShopType").show();
        }
    })

    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
        var isShow = $("#hfIsShowConditions").val() || 0;
        if (isShow == 1) {

            $(".conditionTr").show();
            $("#spanShowConditions").html("收起选项");
        }
        $("#loadRegion").hide();
        $("#loadProvince").hide();
        //$("#loadCity").hide();
        $("#loadCityTier").hide();
        $("#loadChannel").hide();
        $("#loadFormat").hide();
        $("#loadShopLevel").hide();
        $("#loadIsInstall").hide();
        $("#loadShopType").hide();
        $("#loadCS").hide();
        $("#spanShowConditions").click(function () {
            var isShow = $("#hfIsShowConditions").val() || 0;
            if (isShow == 0) {
                $("#hfIsShowConditions").val("1");
                $(".conditionTr").show();
                $("#spanShowConditions").html("收起选项");
            }
            else {
                $("#hfIsShowConditions").val("0");
                $(".conditionTr").hide();
                $("#spanShowConditions").html("展开选项");
            }
        })

    })



    //解决jquery库的冲突，很重要！
    var $j = jQuery.noConflict();
    $j(function () {

        $j("#btnAdd").on("click", function () {

            $j("#hfIsFinishImport").val("");
            //var url = "ImportShops.aspx?customerId=" + customerId;
            var url = "ImportShops.aspx";
            $j.fancybox.open({
                href: url,
                type: 'iframe',
                padding: 5,
                width: "90%",
                afterClose: function () {
                    if ($j("#hfIsFinishImport").val() == "1") {
                        $j("#btnSearch").click();
                        $j("#hfIsFinishImport").val("");
                    }
                }
            });
        })

        $("span[name='checkPOP']").click(function () {
            var shopId = $(this).data("shopid");
            var url = "OneShopPOPList.aspx?shopId=" + shopId;
            $j.fancybox.open({
                href: url,
                type: 'iframe',
                padding: 5,
                width: "90%"

            });
        })

        $("span[name='checkMachineFrame']").click(function () {
            var shopId = $(this).data("shopid");
            var url = "OneShopMachineFrameList.aspx?shopId=" + shopId;
            $j.fancybox.open({
                href: url,
                type: 'iframe',
                padding: 5,
                width: "95%"

            });
        })

        $j("#btnAddCheckChangeLog").click(function () {
            ShowEditLog();
            return false;

        })
    })

    function ShowEditLog() {
        layer.open({
            type: 2,
            time: 0,
            title: '查看店铺修改记录',
            skin: 'layui-layer-rim', //加上边框
            area: ['90%', '90%'],
            content: 'EditLog/ShopLogList.aspx?shopId=' + currShopId,
            id: 'popLayer',
            cancel: function (index) {

            }

        });
    }
    
    
   
</script>
