<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddOrderDetail.aspx.cs"
    Inherits="WebApp.Subjects.ExtraOrder.AddOrderDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <style type="text/css">
        input:
        {
            height: 30px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            零单—添加订单信息
        </p>
    </div>
    <div class="tr">
        》订单信息</div>
    <div>
        <table class="table" style="margin-top: 0px;">
            <tr class="tr_bai">
                <td style="width: 110px;">
                    店铺编号：
                </td>
                <td style="width: 320px; text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtShopNo" runat="server" Style="height: 18px;"></asp:TextBox>
                </td>
                <td style="width: 120px;">
                    店铺名称：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="TextBox3" runat="server" Style="height: 18px; width: 300px;"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    区域：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlRegion" runat="server" Style="height: 22px;">
                        <asp:ListItem>--请选择区域--</asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="DropDownList2" runat="server" Style="height: 22px;">
                        <asp:ListItem>--请选择省份--</asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="DropDownList3" runat="server" Style="height: 22px;">
                        <asp:ListItem>--请选择城市--</asp:ListItem>
                    </asp:DropDownList>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    地址：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="TextBox4" runat="server" MaxLength="50" Style="height: 18px; width: 500px;"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    POP位置：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="TextBox1" runat="server" MaxLength="20" Style="height: 18px;"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
                <td>
                    位置描述：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="TextBox2" runat="server" MaxLength="30" Style="height: 18px;"></asp:TextBox>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    POP宽：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="TextBox5" runat="server" MaxLength="20" Style="height: 18px;"></asp:TextBox>
                    mm
                    <span style="color: Red;">*</span>
                </td>
                <td>
                    POP高：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="TextBox6" runat="server" MaxLength="20" Style="height: 18px;"></asp:TextBox>
                    mm
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    材质：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlCategory" runat="server" Style="height: 22px;">
                        <asp:ListItem>--材质类型--</asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlMaterial" runat="server" Style="height: 22px;">
                        <asp:ListItem>--材质名称--</asp:ListItem>
                    </asp:DropDownList>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    数量：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="TextBox7" runat="server" MaxLength="20" Style="height: 18px;"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
                <td>
                    备注：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="TextBox8" runat="server" MaxLength="20" Style="height: 18px; width: 300px;"></asp:TextBox>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    费用信息：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <table>
                        <tr class="tr_bai">
                            <td style="width: 70px;border:0px;">
                                费用类型：
                            </td>
                            <td style="width: 120px; text-align: left;border:0px;">
                                <select style="height: 22px;">
                                    <option value="">--请选择--</option>
                                    <option value="安装费">安装费</option>
                                    <option value="发货费">发货费</option>
                                    <option value="其他费用">其他费用</option>
                                </select>
                                <span style="color: Red;">*</span>
                            </td>
                            <td style="width: 80px;border:0px;">
                                应收金额：
                            </td>
                            <td style="width: 180px; text-align: left;border:0px;">
                                <asp:TextBox ID="TextBox9" runat="server" MaxLength="8" Style="height: 18px; width: 90px;"></asp:TextBox>
                                <span style="color: Red;">*</span>
                            </td>
                            <td style="width: 80px;border:0px;">
                                应付金额：
                            </td>
                            <td style="width: 120px; text-align: left;border:0px;">
                                <asp:TextBox ID="TextBox10" runat="server" MaxLength="8" Style="height: 18px; width: 90px;"></asp:TextBox>
                                <span style="color: Red;">*</span>
                            </td>
                            
                        </tr>
                        <tr>
                           <td style="border:0px;">备注：</td>
                           <td colspan="3" style="text-align: left;border:0px;">
                              <asp:TextBox ID="TextBox11" runat="server" MaxLength="50" Style="height: 18px; width: 300px;"></asp:TextBox>
                           </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                </td>
                <td colspan="3" style="height: 35px; text-align: left; padding-left: 5px;">
                    <input type="button" id="btnAddPOP" value="添 加" class="layui-btn layui-btn-small" />
                </td>
            </tr>
        </table>
    </div>
   
    <div class="tr" style="margin-top: 10px;">
        》订单信息列表</div>
    </form>
</body>
</html>
