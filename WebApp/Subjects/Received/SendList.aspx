﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendList.aspx.cs" Inherits="WebApp.Subjects.Received.SendList" %>

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
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            发货/收货信息管理
        </p>
    </div>
    <div class="tr">
        >>搜索</div>
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table class="table">
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            客户
                        </td>
                        <td style="text-align: left; padding-left: 5px; width: 250px;">
                            <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged">
                            </asp:DropDownList>
                            <span style="color: Red;">*</span>
                        </td>
                        <td style="width: 120px;">
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            活动时间
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px; width: 250px;">
                            <asp:TextBox ID="txtBeginDate" runat="server" MaxLength="15" onclick="WdatePicker()"
                                ContentEditable="false"></asp:TextBox>
                            —
                            <asp:TextBox ID="txtEndDate" runat="server" MaxLength="15" onclick="WdatePicker()"
                                ContentEditable="false"></asp:TextBox>
                            &nbsp;
                            <asp:Button ID="btnSearchSubject" runat="server" Text="获取活动" class="easyui-linkbutton"
                                Style="width: 65px; height: 26px;" OnClick="btnSearchSubject_Click" />
                            <img id="imgLoading1" style="display: none;" src='/image/WaitImg/loadingA.gif' />
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            活动名称
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px; width: 250px;">
                           
                            <div id="loadGuidance" style="display: none;">
                                <img src='/image/WaitImg/loadingA.gif' />
                            </div>
                            <%--<div id="guidanceDiv">
                            <asp:CheckBoxList ID="cblGuidanceList" runat="server" AutoPostBack="true" RepeatDirection="Horizontal"
                               RepeatLayout="Flow" RepeatColumns="5" OnSelectedIndexChanged="cblGuidance_SelectedIndexChanged">
                           </asp:CheckBoxList>
                           </div>
                            <asp:CheckBoxList ID="cbla" runat="server" RepeatDirection="Horizontal"
                               RepeatLayout="Flow">
                               <asp:ListItem>aaaa </asp:ListItem>
                               <asp:ListItem>bbbb </asp:ListItem>
                               <asp:ListItem>cccc </asp:ListItem>
                            </asp:CheckBoxList>--%>



                            <asp:DropDownList ID="ddlGuidance" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlGuidance_SelectedIndexChanged">
                                <asp:ListItem Value="-1">--请选择--</asp:ListItem>
                            </asp:DropDownList>
                            <span style="color: Red;">*</span>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td class="style1">
                            区域
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;" class="style2">
                            <div id="loadRegion" style="display: none;">
                                <img src="/image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblRegion" runat="server" CssClass="cbl" CellSpacing="20" RepeatDirection="Horizontal"
                                RepeatLayout="Flow" RepeatColumns="5" OnSelectedIndexChanged="cblRegion_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            省份
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
                            <div id="loadProvince" style="display: none;">
                                <img src="/image/WaitImg/loadingA.gif" />
                            </div>
                            <div id="cbAllProvinceDiv" runat="server" style="display: none;text-align:left;">
                                <asp:CheckBox ID="cbAllProvince" runat="server" />全选
                                <hr align="left" style="width: 100px; margin-bottom: 5px;"/>
                            </div>
                            <asp:CheckBoxList ID="cblProvince" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="8" OnSelectedIndexChanged="cblProvince_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            城市
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
                            <div id="loadCity" style="display: none;">
                                <img src="/image/WaitImg/loadingA.gif" />
                            </div>
                            <div id="cbAllCityDiv" runat="server" style="display: none;text-align:left;">
                                <asp:CheckBox ID="cbAllCity" runat="server" />全选
                                <hr align="left" style="width: 100px; margin-bottom: 5px;"/>
                            </div>
                            <asp:CheckBoxList ID="cblCity" runat="server" CssClass="cbl" CellSpacing="20" RepeatDirection="Horizontal"
                                RepeatLayout="Flow" RepeatColumns="8">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            店铺编号
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:TextBox ID="txtShopNo" runat="server" MaxLength="30"></asp:TextBox>
                        </td>
                        <td>
                            店铺名称
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:TextBox ID="txtShopName" runat="server" MaxLength="100" Style="width: 200px;"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hfGuidanceId" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <div style="text-align: right; padding-right: 25px; margin-top: 8px;">
            <asp:Button ID="Button1" runat="server" Text="查 询" OnClientClick="return check()" class="easyui-linkbutton" Style="width: 65px;
                height: 26px;" OnClick="Button1_Click" />
        </div>
    </div>
    <br />
    <div class="tr">
        >>店铺信息列表
    </div>
    <div>
        <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" CellPadding="0"
            CssClass="table" BorderWidth="0" HeaderStyle-BorderStyle="None" 
            EmptyDataText="--无符合条件的信息--" onrowcommand="gv_RowCommand" 
            onrowdatabound="gv_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="序号" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                    </ItemTemplate>
                    <HeaderStyle Width="50px"></HeaderStyle>
                </asp:TemplateField>
                <asp:BoundField DataField="ShopNo" HeaderText="店铺编号" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="ShopName" HeaderText="店铺名称" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="RegionName" HeaderText="区域" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="ProvinceName" HeaderText="省份" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="CityName" HeaderText="城市" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="CityTier" HeaderText="城市级别" HeaderStyle-BorderColor="#dce0e9" />
                <asp:TemplateField HeaderText="POP信息" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <span name="checkPOPSpan" data-shopid='<%#Eval("Id") %>' style="color: Blue; cursor: pointer;">
                            查看</span>
                    </ItemTemplate>
                    <HeaderStyle Width="60px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="发货状态" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:Label ID="labSendState" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="60px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="收货状态" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:Label ID="labReceiveState" runat="server" Text="--"></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="60px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="操作" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:LinkButton CommandArgument='<%#Eval("Id") %>' CommandName="Send" ID="lbSendConfirm"
                            runat="server" Style="color: Blue;">发货确认</asp:LinkButton>
                        |
                        <asp:LinkButton CommandArgument='<%#Eval("Id") %>' CommandName="Receive" ID="lbReceiveConfirm"
                            runat="server" Style="color: Blue;">收货确认</asp:LinkButton>
                    </ItemTemplate>
                    <HeaderStyle Width="130px"></HeaderStyle>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="查看" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:LinkButton CommandArgument='<%#Eval("Id") %>' CommandName="Check" ID="lbCheck"
                            runat="server" Style="color: Blue;">查看</asp:LinkButton>
                    </ItemTemplate>
                    <HeaderStyle Width="50px"></HeaderStyle>
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
    <br />
    
    </form>
</body>
</html>
<script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
<script type="text/javascript">

    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function (sender, e) {
        var eleId = e.get_postBackElement().id;
        if (eleId.indexOf("ddlGuidance") != -1) {
            $("#loadRegion").show();
        }
        if (eleId.indexOf("cblRegion") != -1) {
            $("#loadProvince").show();
           
        }
        if (eleId.indexOf("cblProvince") != -1) {
            $("#loadCity").show();
            
        }
        
    })

    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
        $("#loadProvince").hide();
        $("#loadCity").hide();
        $("#loadRegion").hide();
        $("#cbAllProvince").change(function () {
            var checked = this.checked;
            $("input[name^='cblProvince']").each(function () {
                this.checked = checked;
            });
        })
        $("#cbAllCity").change(function () {
            var checked = this.checked;
            $("input[name^='cblCity']").each(function () {
                this.checked = checked;
            });
        })
    })


    function check() {
        var customerId = $("#ddlCustomer").val();
        if (customerId == 0) {
            alert("请选择客户");
            return false;
        }
        var guidanceId = $("#ddlGuidance").val();
        if (guidanceId == -1) {
            alert("请选择活动");
            return false;
        }
        return true;
    }

    $(function () {
//        $("#guidanceDiv").delegate("input[name^='cblGuidanceList']", "change", function () {
//            $(this).siblings("input").each(function () {
//                this.checked = false;
//            })
//        })

//        $("input[name^='cblGuidanceList']").change(function () {
//            $(this).siblings("input").each(function () {
//                this.checked = false;
//            })
//        })


    })
</script>
