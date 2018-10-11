<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditPOSScale.aspx.cs" Inherits="WebApp.Subjects.InstallPrice.EditPOSScale" %>

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
        function Finish() {
           
            window.parent.FinishEdit();
        }
    </script>
    <style type="text/css">
      #ddlPOSScaleMenu li
      {
         list-style:none;
         margin-bottom:0px;
         text-decoration:underline;
         color:Blue;
         cursor:pointer;	
         margin-left:5px;
      }
    </style>
</head>
<body>
    <form id="form1" runat="server">
     <div>
        <table class="table">
            <tr class="tr_hui">
                <td style="text-align: center; width: 100px;">
                    活动名称
                </td>
                <td style="text-align: left; padding-left: 5px;width: 300px;">
                    <asp:Label ID="labGuidanceName" runat="server" Text=""></asp:Label>
                </td>
                <td style="text-align: center;width: 90px;">
                    店铺数量
                </td>
                <td style="text-align: left; padding-left: 5px;" colspan="2">
                    <asp:Label ID="labShopCount" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
            
        </table>
    </div>
    <div class="tr" style=" height:28px; margin-top:10px;">》搜索</div>
    <div>
      <table class="table">
         <tr class="tr_bai">
            <td style="width:100px;">店铺编号</td>
            <td style=" text-align:left; padding-left:5px; width:300px;">
                <asp:TextBox ID="txtShopNo" runat="server" MaxLength="20"></asp:TextBox>
             </td>
            <td style="width:90px;">店铺名称</td>
            <td style=" text-align:left; padding-left:5px;">
              <asp:TextBox ID="txtShopName" runat="server" MaxLength="30" style=" width:180px;"></asp:TextBox>
            </td>
         </tr>
         <tr class="tr_bai">
            <td>省份</td>
            <td style=" text-align:left; padding-left:5px; ">
                <asp:DropDownList ID="ddlProvince" runat="server" AutoPostBack="true" 
                    onselectedindexchanged="ddlProvince_SelectedIndexChanged">
                  <asp:ListItem Value="">--请选择--</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>城市</td>
            <td style=" text-align:left; padding-left:5px;">
                <asp:DropDownList ID="ddlCity" runat="server">
                  <asp:ListItem Value="">--请选择--</asp:ListItem>
                </asp:DropDownList>
            </td>
         </tr>
         <tr class="tr_bai">
            <td>城市级别</td>
            <td  style=" text-align:left; padding-left:5px;">
                <asp:CheckBoxList ID="cblCityTier" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                </asp:CheckBoxList>
            </td>
            <td colspan="2" style=" text-align:left; padding-left:5px;">
               <asp:Button ID="btnSearch" runat="server" Text="查 询" class="easyui-linkbutton" Style="width: 65px; height: 26px;
                           " onclick="btnSearch_Click" OnClientClick="return checkSearch();"/>
                <img id="searchLoadingImg" style="display:none;" src="../../image/WaitImg/loadingA.gif" />
            </td>
         </tr>
         
      </table>
      <table class="table" style=" margin-top:10px;">
        <tr class="tr_bai">
            <td style="width:100px;">选择店铺规模大小</td>
            <td style=" text-align:left; padding-left:5px; width:300px;">
                <%--<asp:DropDownList ID="ddlPOSScale0" runat="server">
                   <asp:ListItem Value="">--请选择--</asp:ListItem>
                </asp:DropDownList>--%>
                <div style="position: relative;">
                <asp:TextBox ID="txtPOSScale" runat="server" MaxLength="50" style=" width:150px;"></asp:TextBox>
                <div id="divPOSScaleList" style="display: none; position: absolute; width: 150px;  background-color: White;
                            border: 1px solid #ccc; padding-top: 1px; z-index: 100;">
                    <ul id="ddlPOSScaleMenu" style="margin-top: 0; width: 150px;min-height:50px;max-height:150px; overflow:auto;  margin-left: 0px; list-style: none;">
                       
                    </ul>
                 </div>
                </div>
            </td>
            <td style=" text-align:left; padding-left:5px;">
                <asp:Button ID="btnUpdateAll" runat="server" Text="按条件更新" 
                    class="easyui-linkbutton" Style="width: 75px; height: 26px;
                            " onclick="btnUpdateAll_Click" OnClientClick="return check()"/>
                <img id="imgUpdateAll" style=" display:none;" src="../../image/WaitImg/loadingA.gif" />
                <asp:Button ID="btnUpdate" runat="server" Text="更新" class="easyui-linkbutton" Style="width: 65px; height: 26px;
                            margin-left: 20px;" onclick="btnUpdate_Click"/>
                <img  id="imgUpdate" style=" display:none;" src="../../image/WaitImg/loadingA.gif" />

            </td>
         </tr>
      </table>
    </div>
    <div class="tr" style=" height:25px;margin-top:8px;">
        》店铺信息
       </div>
    <div class="containerDiv">
        <asp:Repeater ID="gvShop" runat="server" DataMember= onitemdatabound="gvShop_ItemDataBound">
            <HeaderTemplate>
                <table class="table" style="width: 1200px;">
                    <tr class="tr_hui">
                        <td style="width: 40px;">
                            序号
                        </td>
                        <td>
                            店铺编号
                        </td>
                        <td>
                            店铺规模大小
                        </td>
                        <td>
                            物料支持级别
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
                            安装级别
                        </td>
                        <td>
                            店铺地址
                        </td>
                        
                        
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="tr_bai">
                    <td style="width: 40px;">
                        <%#(AspNetPager1.CurrentPageIndex-1)*AspNetPager1.PageSize+ Container.ItemIndex + 1%>
                    </td>
                    
                    <td>
                        <%#Eval("ShopNo") %>

                    </td>
                    <td>
                        <%--<asp:TextBox ID="txtPOSScale" runat="server" Text='<%#Eval("POSScale")%>'></asp:TextBox>--%>
                        <%--<asp:DropDownList ID="ddlPOSScale" runat="server" Visible="false">
                           <asp:ListItem Value="">空</asp:ListItem>
                        </asp:DropDownList>
                        <asp:Label ID="labPOSScale" runat="server" Text='<%#Eval("POSScale")%>'></asp:Label>
                        <asp:HiddenField ID="hfShopId" runat="server" Value='<%#Eval("shop.Id") %>'/>--%>
                    </td>
                    <td>
                        <%--<%#Eval("MaterialSupport")%>--%>
                    </td>
                    <td>
                        <%#Eval("ShopName") %>
                    </td>
                    <td>
                        <%#Eval("RegionName") %>
                    </td>
                    <td>
                        <%#Eval("ProvinceName") %>
                    </td>
                    <td>
                        <%#Eval("CityName") %>
                    </td>
                    <td>
                        <%#Eval("CityTier")%>
                    </td>
                    <td>
                        <%#Eval("IsInstall")%>
                    </td>
                    <td>
                        <%#Eval("POPAddress")%>
                    </td>
                    
                   
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <%if (gvShop.Items.Count == 0)
                  { %>
                <tr class="tr_bai">
                    <td colspan="15" style="text-align: center;">
                        --无数据--
                    </td>
                </tr>
                <%} %>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    <div style="text-align: center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    function check() {
        if ($("#ddlPOSScale0").val() == "") {
            alert("请选择店铺规模大小");
            return false;
        }
        return true;
    }

    function checkSearch() {
        $("#searchLoadingImg").show();
    }


    $(function () {
        $("#txtPOSScale").on("click", function () {
            showPOSScaleList();
        })

        $("#ddlPOSScaleMenu").delegate("li", "click", function () {

            $("#txtPOSScale").val($(this).html());

        })
    })
    function showPOSScaleList() {
        var li0 = "<li style=' text-align:center; '><img src='../../image/WaitImg/loading1.gif' style=' margin-top:10px;'/></li>";
        $("#ddlPOSScaleMenu").html("").append(li0);
        
        $("#divPOSScaleList").css({ left: 0 + "px", top: 20 + "px" }).slideDown("fast");
        $("body").bind("mousedown", onBodyDown);
        //$("#ddlPOSScaleMenu")
        $.ajax({
            type: "get",
            url: "handler/Handler1.ashx?type=getPOSScale",
            cache:true,
            success: function (data) {
                $("#ddlPOSScaleMenu").find("li").eq(0).hide();
                var li = "";
                if (data != "") {
                    var json = eval(data);
                    
                    for (var i = 0; i < json.length; i++) {
                        li += "<li>" + json[i].POSScale + "</li>";
                    }
                    
                }
                else {
                    li = "<li style='text-align:center; '>无数据</li>";
                }
                $("#ddlPOSScaleMenu").append(li);
            }
        });
    }
    function hideMenu() {
        $("#divPOSScaleList").fadeOut("fast");
        $("body").unbind("mousedown", onBodyDown);
    }

    function onBodyDown(event) {
        if (!(event.target.id == "txtPOSScale" || event.target.id == "divPOSScaleList" || $(event.target).parents("#divPOSScaleList").length > 0)) {
            hideMenu();
        }

    }
</script>
