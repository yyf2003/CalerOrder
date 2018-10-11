<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GuidanceDetail.aspx.cs"
    Inherits="WebApp.Subjects.GuidanceDetail" %>

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
   
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    


    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            活动指引明细
        </p>
    </div>
    <div>
        <table class="table">
            <tr class="tr_bai">
                <td style="text-align: right; width: 120px;">
                    客户名称：
                </td>
                <td style="text-align: left; padding-left: 5px; width: 300px;">
                    <asp:Label ID="labCustomerName" runat="server" Text=""></asp:Label>
                </td>
                <td style="width: 120px;">
                    类型：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                  <asp:Label ID="labAddType" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align: right; width: 120px;">
                    活动名称：
                </td>
                <td style="text-align: left; padding-left: 5px; width: 300px;">
                    <asp:Label ID="labItemName" runat="server" Text=""></asp:Label>
                </td>
                <td style="width: 120px;">
                   活动类型：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   <asp:Label ID="labActivityType" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align: right;">
                    开始时间：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labBeginDate" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    结束时间：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labEndDate" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align: right;">
                    活动月份：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labGuidanceMonth" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    材质价格方案：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labMaterialPriceItem" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align: right;">
                    项目分类：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labSubjectType" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align: right;">
                    项目命名规范：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labSubjectNames" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align: right;">
                    备注：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labRemark" runat="server" Text=""></asp:Label>
                </td>
            </tr>
           <%-- <tr class="tr_bai">
                <td style="text-align: right;">
                    原始订单：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:Panel ID="Panel1" runat="server">
                    </asp:Panel>
                </td>
            </tr>--%>
            <tr class="tr_bai">
                <td style="text-align: right;">
                    附件信息：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                   
                    <asp:Panel ID="Panel2" runat="server">
                    </asp:Panel>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align: right;">
                    状态：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px; height:35px;">
                    <div class="layui-inline">
                    <asp:Label ID="labState" runat="server" Text=""></asp:Label>
                    <%--<span id="spanState" runat="server"></span>--%>
                    <span runat="server" id="spanChangeState" class="layui-btn layui-btn-small" style=" margin-left:30px; display:none;"><i class="layui-icon" style=" margin-right:5px;">&#xe610;</i>设置完成</span>
                    <span runat="server" id="spanOpenState" class="layui-btn layui-btn-danger layui-btn-small" style=" margin-left:30px; display:none;"><i class="layui-icon" style=" margin-right:5px;">&#xe628;</i>打 开</span>
                    </div>
                    <asp:Button ID="btnChangeState" runat="server" Text="Button" 
                        onclick="btnChangeState_Click" style="display:none;"/>
                    <asp:Button ID="btnOpenState" runat="server" Text="Button" 
                         style="display:none;" onclick="btnOpenState_Click"/>
                </td>
            </tr>
        </table>
        <div style="margin-top: 20px; height:30px;">
            <asp:HiddenField ID="hfAddType" runat="server" />
            <asp:Button ID="btnAddSubject" runat="server" Visible="false" Text="新增项目" class="layui-btn layui-btn-small"
            style="width: 65px; height:26px; float:right; margin-right:30px;" 
                onclick="btnAddSubject_Click"/>
                <asp:Button ID="btnRefresh" runat="server"  Text="刷 新" class="layui-btn layui-btn-small"
            style="width: 65px; height:26px; float:right; margin-right:30px;" 
                onclick="btnRefresh_Click" />
                 <asp:Button ID="btnToExport" runat="server"  Text="导出订单" class="layui-btn layui-btn-small"
            style="width: 65px; height:26px; float:right; margin-right:30px;" onclick="btnToExport_Click" 
                 />
        </div>
        
        <div class="tr" >
            >>项目信息
        </div>
        <div>
            <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" CellPadding="0"
                CssClass="table" BorderWidth="0" HeaderStyle-BorderStyle="None" EmptyDataText="--无符合条件的信息--"
                OnRowCommand="gv_RowCommand" OnRowDataBound="gv_RowDataBound">
                <Columns>
                    <asp:TemplateField HeaderText="序号" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                        <ItemTemplate>
                            <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                        </ItemTemplate>
                        <HeaderStyle Width="50px"></HeaderStyle>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SubjectNo" HeaderText="项目编号" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="SubjectName" HeaderText="项目名称" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="Contact" HeaderText="项目负责人" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="CustomerName" HeaderText="所属客户" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="RealName" HeaderText="创建人" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:BoundField DataField="AddDate" HeaderText="创建时间" HeaderStyle-BorderColor="#dce0e9" />
                    <asp:TemplateField HeaderText="项目状态" HeaderStyle-BorderColor="#dce0e9">
                        <ItemTemplate>
                            <asp:Label ID="labStatus" runat="server" Text=""></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="60px"></HeaderStyle>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="审批状态" HeaderStyle-BorderColor="#dce0e9">
                        <ItemTemplate>
                            <asp:Label ID="labApprove" runat="server" Text=""></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="60px"></HeaderStyle>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="编辑" Visible="false" HeaderStyle-BorderColor="#dce0e9">
                        <ItemTemplate>
                            <asp:LinkButton CommandArgument='<%#Eval("Id") %>' ID="lbEdit" CommandName="EditOrder"
                                runat="server" Style="color: Blue;">编辑</asp:LinkButton>
                        </ItemTemplate>
                        <HeaderStyle Width="60px"></HeaderStyle>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="查看" HeaderStyle-BorderColor="#dce0e9">
                        <ItemTemplate>
                            <asp:LinkButton CommandArgument='<%#Eval("Id") %>' CommandName="Check" ID="lbCheck"
                                runat="server" Style="color: Blue;">查看</asp:LinkButton>
                        </ItemTemplate>
                        <HeaderStyle Width="50px"></HeaderStyle>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="修改订单" Visible="false" HeaderStyle-BorderColor="#dce0e9">
                        <ItemTemplate>
                            <asp:LinkButton CommandArgument='<%#Eval("Id") %>' ID="lbEditOrder" runat="server"
                                Style="color: Blue;">修改</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="导出" HeaderStyle-BorderColor="#dce0e9">
                        <ItemTemplate>
                            <asp:LinkButton CommandArgument='<%#Eval("Id") %>' ID="lbExport" runat="server" Style="color: Blue;">导出</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="操作" Visible="false" HeaderStyle-BorderColor="#dce0e9">
                        <ItemTemplate>
                            <asp:LinkButton CommandArgument='<%#Eval("Id") %>' CommandName="DeleteItem" ID="lbDelete"
                                runat="server" Style="color: red;">删除</asp:LinkButton>
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
                CustomInfoTextAlign="Left" LayoutType="Table" onpagechanged="AspNetPager1_PageChanged">
            </webdiyer:AspNetPager>
        </div>
        <br />
    </div>
    <br />
    <div style=" text-align:center; margin-bottom:20px;">
    <%--<input type="button" value="返 回" onclick="javascript:window.history.go(-1)" class="easyui-linkbutton" 
            style="width: 65px; height:26px;"/>--%>
        <asp:Button ID="btnBack" runat="server" Text="返 回"  CssClass="easyui-linkbutton" 
            style="width: 65px; height:26px;" onclick="btnBack_Click"/>
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    $(function () {
        
        $("#spanChangeState").click(function () {
            layer.confirm('确定完成吗？', {
                btn: ['是的', '取消'] //按钮
            }, function () {
                $("#btnChangeState").click();
            });
        })
        $("#spanOpenState").click(function () {
            layer.confirm('确定打开吗？', {
                btn: ['是的', '取消'] //按钮
            }, function () {
                $("#btnOpenState").click();
            });
        })
    })
</script>