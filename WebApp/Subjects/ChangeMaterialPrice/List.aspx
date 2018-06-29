<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="List.aspx.cs"
    Inherits="WebApp.Subjects.ChangeMaterialPrice.List" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            layui.use('element', function () {
                var element = layui.element;
                element.init();
            });
        })
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <a href="/index.aspx">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /></a><p
                class="nav_table_p">
                修改订单材质单价
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
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged">
                            </asp:DropDownList>
                            <span style="color: Red; cursor: pointer;">*</span>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            活动时间
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:TextBox ID="txtGuidanceMonth" runat="server" CssClass="Wdate" onclick="WdatePicker({skin:'whyGreen',dateFmt:'yyyy-MM',isShowClear:false,readOnly:true,onpicked:getMonth})"
                                Style="width: 80px;" OnTextChanged="txtGuidanceMonth_TextChanged" AutoPostBack="true"></asp:TextBox>
                            <asp:LinkButton ID="lbUp" runat="server" Style="margin-left: 20px; cursor: pointer;
                                color: Blue;" OnClick="lbUp_Click">上一个月</asp:LinkButton>
                            <asp:LinkButton ID="lbDown" runat="server" Style="margin-left: 20px; cursor: pointer;
                                color: Blue;" OnClick="lbDown_Click">下一个月</asp:LinkButton>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            活动名称
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <div id="loadGuidance" style="display: none;">
                                <img src="../../image/WaitImg/loadingA.gif" />
                            </div>
                            <div>
                                <asp:CheckBox ID="cbCheckAllGuidance" runat="server" /><span style="color: Blue;">全选</span>
                                <asp:Button ID="btnCheckAllGuidance" runat="server" Text="Button" OnClick="btnCheckAllGuidance_Click"
                                    Style="display: none;" />
                            </div>
                            <asp:CheckBoxList ID="cblGuidanceList" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="5" AutoPostBack="true"
                                OnSelectedIndexChanged="cblGuidanceList_SelectedIndexChanged">
                            </asp:CheckBoxList>
                            <asp:Panel ID="Panel_EmptyGuidance" runat="server" Visible="false">
                                <span style="color: Red;">无活动信息！</span>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td class="style1">
                            材质信息
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <div id="loadMaterial" style="display: none;">
                                <img src="../../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblMaterial" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="5">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                        </td>
                        <td style="text-align: left; padding-left: 5px; height: 40px;">
                            <asp:Button ID="btnSearch" runat="server" Text="查 询" class="layui-btn layui-btn-small"
                                OnClick="btnSearch_Click" OnClientClick="return check()" />
                            <img src="../../image/WaitImg/loadingA.gif" id="loadSearch" style="display: none;" />
                        </td>
                    </tr>
                </table>
                <table class="table" style="margin-top: 20px;">
                    <tr>
                        <td style="width: 120px; height: 40px;">
                            请选择新价格：
                        </td>
                        <td style="width: 200px; text-align: left; padding-left: 5px;">
                            <asp:DropDownList ID="ddlPriceItem" runat="server">
                                <asp:ListItem Value="0">--请选择价格条目--</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:Button ID="btnUpdatePrice" runat="server" Text="更 新" class="layui-btn layui-btn-small"
                                OnClientClick="return checkUpdate()" />
                            <img src="../../image/WaitImg/loadingA.gif" id="loadUpdatePrice" style="display: none;" />
                            <%--<asp:Label ID="labUpdateMsg" runat="server" Text="" style="color:Blue;"></asp:Label>--%>
                            <span id="labUpdateMsg" style="color:Blue;"></span>
                        </td>
                    </tr>
                </table>
                <div class="tr">
                    》订单信息
                </div>
                <asp:Repeater ID="Repeater1" runat="server">
                    <HeaderTemplate>
                        <table class="table">
                            <tr class="tr_hui">
                                <td style="width: 40px;">
                                    序号
                                </td>
                                <td>
                                    活动
                                </td>
                                <td>
                                    店铺编号
                                </td>
                                <td>
                                    店铺名称
                                </td>
                                <td>
                                    Sheet
                                </td>
                                <td>
                                    材质
                                </td>
                                <td>
                                    单价
                                </td>
                                <td>
                                    单位
                                </td>
                                <td>
                                    数量
                                </td>
                                <td>
                                    画面宽
                                </td>
                                <td>
                                    画面高
                                </td>
                                <td>
                                    面积
                                </td>
                                <td>
                                    合计金额
                                </td>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="tr_bai">
                            <td>
                                <%#Container.ItemIndex+1+((AspNetPager1.CurrentPageIndex-1)*AspNetPager1.PageSize) %>
                            </td>
                            <td>
                                <%#Eval("ItemName") %>
                            </td>
                            <td>
                                <%#Eval("order.ShopNo") %>
                            </td>
                            <td>
                                <%#Eval("order.ShopName") %>
                            </td>
                            <td>
                                <%#Eval("order.Sheet") %>
                            </td>
                            <td>
                                <%#Eval("order.GraphicMaterial")%>
                            </td>
                            <td>
                                <%#Eval("order.UnitPrice")%>
                            </td>
                            <td>
                                <%#Eval("order.UnitName")%>
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
                                <%#Eval("order.Area") != null ? Math.Round(decimal.Parse(Eval("order.Area").ToString()), 2).ToString() : Eval("order.Area")%>
                            </td>
                            <td>
                                <%#Eval("order.TotalPrice") != null ? Math.Round(decimal.Parse(Eval("order.TotalPrice").ToString()), 2).ToString() : Eval("order.TotalPrice")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <%if (Repeater1.Items.Count == 0)
                          {%>
                        <tr class="tr_bai">
                            <td colspan="13" style="text-align: center;">
                                --无数据--
                            </td>
                        </tr>
                        <%} %>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <div style="text-align: center;">
                    <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
                        CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                        NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" showinputbox="Never"
                        CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
                    </webdiyer:AspNetPager>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    function getMonth() {
        $("#txtGuidanceMonth").blur();
    }



    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function (sender, e) {
        var eleId = e.get_postBackElement().id;
        if (eleId.indexOf("lbUp") != -1 || eleId.indexOf("lbDown") != -1) {
            $("#loadGuidance").show();
        }
        if (eleId.indexOf("btnCheckAllGuidance") != -1 || eleId.indexOf("cblGuidanceList") != -1) {
            $("#loadMaterial").show();
        }
    });

    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
        $("#loadGuidance").hide();
        $("#loadMaterial").hide();
        $("#loadSearch").hide();
        $("#loadUpdatePrice").hide();
        $("#cbCheckAllGuidance").change(function () {
            var checked = this.checked;
            $("input[name^='cblGuidanceList']").each(function () {
                this.checked = checked;
            });
            $("#btnCheckAllGuidance").click();
        });

    });

    function check() {
        var guidanceId = "";
        $("input[name^='cblGuidanceList']:checked").each(function () {
            guidanceId += $(this).val() + ",";
        })
        if (guidanceId == "") {
            layer.msg("请选择活动");
            return false;
        }
        $("#loadSearch").show();

        return true;

    }

    function checkUpdate() {
        $("#labUpdateMsg").html("更新中...");
        var guidanceId = "";
        $("input[name^='cblGuidanceList']:checked").each(function () {
            guidanceId += $(this).val() + ",";
        })
        if (guidanceId == "") {
            layer.msg("请选择活动");
            return false;
        }
        var material = "";
        $("input[name^='cblMaterial']:checked").each(function () {
            material += $(this).val() + ",";
        })

        var priceItemId = $("#ddlPriceItem").val() || 0;
        if (priceItemId == 0) {
            layer.msg("请选择价格条目");
            return false;
        }
        $("#loadUpdatePrice").show();
        //$.ajaxSettings.async = true;
        $.ajax({
            type: "post",
            url: "Handler1.ashx",
            data: { type: "updateUnitPrice", priceItemId: priceItemId, guidanceId: guidanceId, material: material },
            beforeSend:function(){
                
            },
            complete: function () {
                $("#loadUpdatePrice").hide();
            },
            success: function (data) {

                if (data == "ok") {
                    $("#labUpdateMsg").html("");
                    layer.msg("更新成功");
                    //clearInterval(timer);
                    $("#btnSearch").click();
                }
                else {
                    //clearInterval(timer);
                    $("#labUpdateMsg").html("更新失败：" + data);
                }
            }
        });
        
        //checkUpdateState();
        return false;
    }


    var timer;
    function checkUpdateState() {
        $("#labUpdateMsg").html("更新中。。。！");
        timer = setInterval(function () {
            
            $.ajax({
                type: "get",
                url: "CheckUpdateState.ashx?time="+Math.random()*1000,
                cache: false,
                success: function (data) {
                    if (data != "") {
                        var json = eval(data);
                        var total = json[0].Total;
                        var finish = json[0].Finish;
                       
                        var progress = parseInt(finish / total);
                        $("#labUpdateMsg").html("已完成：" + (progress * 100) + "%");
                        
                    }
                }
            });

        }, 1000);
    }


   
</script>
