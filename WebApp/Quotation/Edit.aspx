<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Quotation.Edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../easyui1.4/jquery.min.js"></script>
    <script src="../easyui1.4/jquery.easyui.min.js"></script>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var subjectId = '<%=subjectId %>';
        function Finish() {
            alert("提交成功！");
            window.parent.FinishImport();
        }
        function Fail(msg) {
            alert("提交失败：" + msg);

        }
    </script>
    <style type="text/css">
       
      #editTb li
       {
         margin-bottom:5px;
         height:20px;
         font-size:14px;
         cursor:pointer;    
         padding-left:5px;
       }
       #editTb li:hover{background-color:#f0f1f2;}
       #BingPriceNameList li:hover{ text-decoration:underline; color:Blue; cursor:pointer;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            编辑报价信息
        </p>
    </div>
    <div>
        <div class="tr">
            >>费用信息
        </div>
        <table class="table" id="editTb">
            <tr class="tr_bai">
                <td style="width: 120px;">
                    类别
                </td>
                <td style="width: 300px; text-align: left; padding-left: 5px;">
                    <div style="position: relative;">
                        <asp:TextBox ID="txtCategory" class="showDll" runat="server" MaxLength="50"></asp:TextBox>
                        <div id="divCategory" style="display: none; position: absolute; background-color: White;
                            border: 1px solid #ccc; padding-top: 2px; height: 130px; width:150px; overflow: auto;">
                            <ul id="ddlCategory" style="margin-top: 0; width: 130px; margin-left: 0px; list-style: none;">
                            </ul>
                        </div>
                        <span style="color: Red;">*</span>
                    </div>
                </td>
                <td style="width: 120px;">
                    AD费用归属
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <div style="position: relative;">
                        <asp:TextBox ID="txtBelongs" class="showDll" runat="server" MaxLength="50"></asp:TextBox>
                        <div id="divBelongs" style="display: none; position: absolute; background-color: White;
                            border: 1px solid #ccc; padding-top: 2px; height: 130px; width:150px; overflow: auto;">
                            <ul id="ddlBelongs" style="margin-top: 0; width: 120px; margin-left: 0px; list-style: none;">
                            </ul>
                        </div>
                        <span style="color: Red;">*</span>
                    </div>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    分类
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <div style="position: relative;">
                        <asp:TextBox ID="txtClassification" class="showDll" runat="server" MaxLength="50"></asp:TextBox>
                        <div id="divClassification" style="display: none; position: absolute; background-color: White;
                            border: 1px solid #ccc; padding-top: 2px; height: 180px;width:200px; overflow: auto;">
                            <ul id="ddlClassification" style="margin-top: 0; width: 180px; margin-left: 0px;
                                list-style: none;">
                            </ul>
                        </div>
                        <span style="color: Red;">*</span>
                    </div>
                </td>
                <td>
                    Adidas负责人
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtAdidasContact" runat="server" MaxLength="50" style=" width:200px;"></asp:TextBox>
                    （多个人的时候请用逗号(，)分隔）
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    税率
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <div style="position: relative;">
                        <asp:TextBox ID="txtTaxRate" class="showDll" runat="server" MaxLength="50"></asp:TextBox>
                        <div id="divTaxRate" style="display: none; position: absolute; background-color: White;
                            border: 1px solid #ccc; padding-top: 2px; height: 120px;width: 150px; overflow: auto;">
                            <ul id="ddlTaxRate" style="margin-top: 0; width: 130px; margin-left: 0px; list-style: none;">
                            </ul>
                        </div>
                        <span style="color: Red;">*</span>
                    </div>
                </td>
                <td>
                    报价账户</td>
                <td style="text-align: left; padding-left: 5px;">
                    <div style="position: relative;">
                        <asp:TextBox ID="txtAccount" class="showDll" runat="server" MaxLength="10"></asp:TextBox>
                        <div id="divAccount" style="display: none; position: absolute; background-color: White;
                            border: 1px solid #ccc; padding-top: 2px; height: 120px;width: 150px; overflow: auto;">
                            <ul id="ddlAccount" style="margin-top: 0; width: 130px; margin-left: 0px; list-style: none;">
                            </ul>
                        </div>
                        <span style="color: Red;">*</span>
                    </div>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    报价金额</td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtOfferPrice" runat="server" MaxLength="10"></asp:TextBox>
                    <span style="color: Red;">*</span>（不含挂账）
                </td>
                <td>
                    挂账金额
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtOtherPrice" runat="server" MaxLength="10"></asp:TextBox>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    挂账说明</td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtOtherPriceRemark" runat="server" MaxLength="50" Style="width: 300px;"></asp:TextBox>
                   
                </td>
               
            </tr>
            <tr class="tr_bai">
                <td>
                    备注</td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtRemark" runat="server" MaxLength="50" Style="width: 300px;"></asp:TextBox>
                   
                </td>
               
            </tr>
        </table>
        <br />
        <div class="tr">
            >>分摊金额
        </div>
        <table class="table" style="width: 425px;">
            <thead>
                <tr class="tr_hui">
                    <td style="width: 120px;">
                        费用名称
                    </td>
                    <td style="width: 200px;">
                        费用金额
                    </td>
                    <td>
                        删除
                    </td>
                </tr>
            </thead>
            <tbody id="SharePrice">
                <tr class="tr_bai">
                    <td>
                        <input type="text" name="txtSharePriceName" value="北京" maxlength="20" style="text-align: center;
                            width: 90%;" />
                    </td>
                    <td>
                        <input type="text" name="txtSharePrice" value="" maxlength="10" style="text-align: center;
                            width: 80%;" />
                    </td>
                    <td>
                        <span name="deleSharePrice" style="color: Red; cursor: pointer;">删除</span>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        <input type="text" name="txtSharePriceName" value="上海" maxlength="20" style="text-align: center;
                            width: 90%;" />
                    </td>
                    <td>
                        <input type="text" name="txtSharePrice" value="" maxlength="10" style="text-align: center;
                            width: 80%;" />
                    </td>
                    <td>
                        <span name="deleSharePrice" style="color: Red; cursor: pointer;">删除</span>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        <input type="text" name="txtSharePriceName" value="成都" maxlength="20" style="text-align: center;
                            width: 90%;" />
                    </td>
                    <td>
                        <input type="text" name="txtSharePrice" value="" maxlength="10" style="text-align: center;
                            width: 80%;" />
                    </td>
                    <td>
                        <span name="deleSharePrice" style="color: Red; cursor: pointer;">删除</span>
                    </td>
                </tr>
            </tbody>
            <tfoot>
                <tr class="tr_bai">
                    <td style="height: 35px;">
                        <input type="text" id="txtSharePriceName" maxlength="20" value="" style="text-align: center;
                            width: 90%;" />
                    </td>
                    <td>
                        <input type="text" id="txtSharePrice" value="" maxlength="10" style="text-align: center;
                            width: 80%;" />
                    </td>
                    <td>
                        <input type="button" id="btnAddSharePrice" value="添加" class="easyui-linkbutton" style="width: 65px;
                            height: 26px;" />
                    </td>
                </tr>
            </tfoot>
        </table>
        <br />
        <div class="tr">
            >>并入金额
        </div>
        <table class="table" style="width: 425px;">
            <thead>
                <tr class="tr_hui">
                    <td style="width: 200px;">
                        名称
                    </td>
                    <td style="width: 160px;">
                        金额
                    </td>
                    <td>
                        删除
                    </td>
                </tr>
            </thead>
            <tbody id="BingPrice">
            </tbody>
            <tfoot id="AddBingPrice">
                <tr class="tr_bai">
                    <td style="height: 35px;">
                       
                        <%--<asp:DropDownList ID="ddlSubjects" runat="server">
                           <asp:ListItem Value="">--请选择--</asp:ListItem>
                        </asp:DropDownList>--%>
                        <div style=" position:relative;">
                           <input type="text" id="txtBingPriceName" maxlength="20" value="" style="text-align: center;
                            width: 150px;" /><span id="showBingPriceName" style="color:Blue; cursor:pointer; text-decoration:underline;">选择</span>
                            <div id="divBingPriceName" style="display:none; position:absolute; width:1px; height:1px; border:1px solid #ccc; top:1px; left:190px; background-color:White;">
                               <div class="tr" style=" text-align:left;">>>选择项目名称
                                  <span id="closeBingPriceName" style=" float:right; cursor:pointer;">
                                      <img src="../image/close.gif" />
                                   </span>
                               </div>
                               <div class="tr_hui" style=" text-align:left; height:35px; margin-top:-6px; line-height:35px;">
                                  <span style="float:left; margin-left:5px;">选择时间：<input type="text" id="txtDate" onclick="WdatePicker({'dateFmt':'yyyy年MM月'})" Contenteditable="false" style=" width:100px; "/></span>
                                  <input type="button" id="btnSearchSubject"  value="查询" class="easyui-linkbutton" style="width: 65px;
                            height: 26px; float:right; margin-left:8px;  float:left;"/>
                                  
                               </div>
                               <div id="DivBingPriceNameList" style=" height:200px; overflow:auto;">
                               
                                  <ul id="BingPriceNameList"></ul>
                               </div>
                            </div>
                        </div>
                    </td>
                    <td>
                        <input type="text" id="txtBingPrice" value="" maxlength="10" style="text-align: center;
                            width: 80%;" />
                    </td>
                    <td>
                        <input type="button" id="btnAddBingPrice" value="添加" class="easyui-linkbutton" style="width: 65px;
                            height: 26px;" />
                    </td>
                </tr>
            </tfoot>
        </table>
    </div>
    <br />
    <asp:HiddenField ID="hfSharePrice" runat="server" />
    <asp:HiddenField ID="hfBingPrice" runat="server" />
    <div style="text-align: center;">
        <asp:Button ID="btnSubmit" runat="server" Text="提 交" OnClientClick="return CheckPriceVal()"
            class="easyui-linkbutton" Style="width: 65px; height: 26px;" OnClick="btnSubmit_Click" />
    </div>
    <br />
    </form>
</body>
</html>
<script src="js/Edit.js" type="text/javascript"></script>
