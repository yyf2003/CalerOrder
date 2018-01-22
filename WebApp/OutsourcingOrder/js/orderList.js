
var currOutsourceId = "";
var currOutsourceName = "";
var guidanceIds = "";
var subjectIds = "";
var oType = "";
var activeTabIndex = 0;
var firstLoad = 1;
var materialCategoryId = "";
var orderShopNo = "";
var exportType = "";
var materialName = "";

$(function () {
    GetOutsourceList();
    //Order.getOrderList(orderPageIndex, orderPageSize);

    $("#guidanceDiv").delegate("input[name='guidanceCB']", "change", function () {
        GetSubjectList();
        GetMaterialCategory();
    })

    $("#guidanceCBALL").click(function () {
        var checked = this.checked;
        $("input[name='guidanceCB']").each(function () {
            this.checked = checked;
        })
        GetSubjectList();
        GetMaterialCategory();
    })

    $("#subjectCBALL").click(function () {
        var checked = this.checked;
        $("input[name='subjectcb']").each(function () {
            this.checked = checked;
        })
       
        GetMaterialCategory();
    })

    $("#subjectDiv").delegate("input[name='subjectcb']", "change", function () {

        GetMaterialCategory();
    })

    $("#btnSearch").click(function () {
        GetCondition();
        Order.getOrderList(orderPageIndex, orderPageSize);
    })
    layui.use('element', function () {
        var element = layui.element();

        //tab事件监听
        element.on('tab(order)', function (data) {
            activeTabIndex = data.index;

            if (activeTabIndex == 1 && firstLoad == 1) {
                //firstLoad = 0;
                //$("#Button1").click();
            }
        });

    });

    $("#txtSearchOrderShopNo").searchbox({
        width: 300,
        searcher: doSearchOrder,
        prompt: '请输入店铺编号,可以输入多个,逗号分隔'
    })

    $("#btnRefresh").click(function () {
        Order.getOrderList(orderPageIndex, orderPageSize);
    })

    //导出喷绘王模板-北京
    $("#btnExportBJPHW").click(function () {

        GetCondition();
        if (currOutsourceId == 0) {
            alert("请选择外协");
            return false;
        }
        if (guidanceIds == "") {
            alert("请选择活动");
            return false;
        }

        Export("exportbjphw");

    })
    //导出喷绘王模板-外协
    $("#btnExportOtherPHW").click(function () {
        GetCondition();
        if (currOutsourceId == 0) {
            alert("请选择外协");
            return false;
        }
        if (guidanceIds == "") {
            alert("请选择活动");
            return false;
        }

        Export("exportotherphw");

    })

    //导出350
    $("#btnExport350").click(function () {
        GetCondition();
        if (currOutsourceId == 0) {
            alert("请选择外协");
            return false;
        }
        if (guidanceIds == "") {
            alert("请选择活动");
            return false;
        }

        //$(this).attr("disabled", true).next("img").show();
        //checkExportBJPHW($(this));
        //var url = "Handler/ExportOrders.ashx?type=exportbjphw&subjectids=" + subjectId;
        Export("export350");

    })

    $("input[name^='cblExportType']").change(function () {

        if (this.checked) {

            $(this).siblings("input").each(function () {
                this.checked = false;
            });
        }
    })
    $("input[name^='cblMaterial']").change(function () {

        if (this.checked) {
            $(this).siblings("input").each(function () {
                this.checked = false;
            });
        }
    })
})


var orderPageIndex = 1;
var orderPageSize = 20;
var Order = {
    getOrderList: function (pageIndex, pageSize) {
        $("#divContent1").css({ height: "600px" });
        orderShopNo = $.trim($("#txtSearchOrderShopNo").val());
        $("#tbOrderList").datagrid({
            queryParams: { type: "getOrderList", outsourceId: currOutsourceId, guidanceIds: guidanceIds, subjectIds: subjectIds, materialCategoryId: materialCategoryId, outsourceType: oType, shopNo: orderShopNo, currpage: pageIndex, pagesize: pageSize, exportType: exportType, materialName: materialName },
            method: 'get',
            url: 'handler/OrderList.ashx',
            columns: [[
                        { field: 'rowIndex', title: '序号' },
                        { field: 'Id', title: '序号', hidden: true },
                        { field: 'OrderType', title: '类型' },
                        { field: 'ShopNo', title: '店铺编号' },
                        { field: 'ShopName', title: '店铺名称' },
                        { field: 'Region', title: '区域' },
                        { field: 'Province', title: '省份' },
                        { field: 'City', title: '城市' },
                        { field: 'CityTier', title: '城市级别' },
                        { field: 'Format', title: '店铺类型' },
                        { field: 'OrderPrice', title: '费用金额' },
                        { field: 'Sheet', title: 'POP位置' },
                        { field: 'Gender', title: '男/女' },
                        { field: 'Quantity', title: '数量' },
                        { field: 'GraphicWidth', title: 'POP宽' },
                        { field: 'GraphicLength', title: 'POP高' },
                        { field: 'GraphicMaterial', title: '材质' },
                        { field: 'ChooseImg', title: '选图' },
                        { field: 'PositionDescription', title: '位置描述' }



            ]],
            height: "100%",
            toolbar: "#toolbar",
            pageList: [20],
            striped: true,
            border: false,
            singleSelect: true,
            pagination: true,
            pageNumber: pageIndex,
            pageSize: pageSize,
            fitColumns: true,
            iconCls: 'icon-save',
            //fit: false,
            emptyMsg: '没有相关记录'
            //onLoadSuccess: function (data) {
                //var outHeight = $("#conditionTB").height();
                //var tabContentHeight = outHeight - 255;
                

            //},
//            onBeforeLoad: function (param) {

//            }


        });
        var p = $("#tbOrderList").datagrid('getPager');
        $(p).pagination({
            showRefresh: false,
            onSelectPage: function (curIndex, curSize) {
                Order.getOrderList(curIndex, curSize);
            }
        });


    },
    getShopList: function () { }
};

function GetOutsourceList() {

    $("#tbOutsource").datagrid({
        method: 'get',
        url: 'handler/OrderList.ashx?type=getOutsource',
        columns: [[
                    { field: 'Id', hidden: true },
                    { field: 'CompanyName', title: '外协名称' }
                ]],
        height: '100%',
        border: false,
        fitColumns: true,
        singleSelect: false,
        rownumbers: true,
        emptyMsg: '没有相关记录',
        onClickRow: function (rowIndex, data) {
            //selectOutsource(data.Id, data.CompanyName);
            selectOutsource();
        }
    })
}

//function selectOutsource(oId, oName) {
//    currOutsourceId = oId;
//    currOutsourceName = oName;
//    $("#orderTitle").panel({
//        title: ">>外协名称：" + oName
//    });
//    GetGuidacneList();
//    ClearDiv();
//}

function selectOutsource() {
    var rows = $("#tbOutsource").datagrid("getSelections");
    var oName = "";
    currOutsourceName = "";
    currOutsourceId = "";
    if (rows && rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            currOutsourceId += (rows[i].Id + ",");
            oName += (rows[i].CompanyName + ",");
        }
    }
    if (oName.length > 0) {
        oName = oName.substring(0, oName.length - 1);
    }
    if (currOutsourceId.length > 0) {
        currOutsourceId = currOutsourceId.substring(0, currOutsourceId.length - 1);
    }
    currOutsourceName = oName;
    $("#orderTitle").panel({
        title: ">>外协名称：" + oName
    });
    GetGuidacneList();
    ClearDiv();
}

function GetGuidacneList() {
    var customerId = $("#ddlCustomer").val();
    var guidanceMonth = $.trim($("#txtMonth").val());

    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getGuidanceList", outsourceId: currOutsourceId, customerId: customerId, guidanceMonth: guidanceMonth },
        success: function (data) {
            $("#guidanceDiv").html("");
            if (data != "") {
                var json = eval(data);
                for (var i = 0; i < json.length; i++) {

                    var div = "<div style='float:left;'><input type='checkbox' name='guidanceCB' value='" + json[i].GuidanceId + "' />" + json[i].GuidanceName + "&nbsp;</div>";
                    $("#guidanceDiv").append(div);
                }
            }
            else {
                $("#guidanceDiv").html("<span style='color:red;'>无活动信息！</span>");
            }
        }
    });
}

function GetSubjectList() {
    GetCondition();
    $("#ImgLoadSubject").show();
    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getSubjectList", outsourceId: currOutsourceId, guidanceIds: guidanceIds },
        complete: function () { $("#ImgLoadSubject").hide(); },
        success: function (data) {
            $("#subjectDiv").html("");
            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='subjectcb' value='" + json[i].SubjectId + "' /><span>" + json[i].SubjectName + "</span>&nbsp;</div>";

                }
                $("#subjectDiv").html(div);

            }
        }
    })
}

function GetMaterialCategory() {
    GetCondition();

    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getMaterialCategory", outsourceId: currOutsourceId, guidanceIds: guidanceIds, subjectIds: subjectIds },
        beforeSend: function () { $("#ImgLoadMC").show(); },
        complete: function () { $("#ImgLoadMC").hide(); },
        success: function (data) {
            $("#MaterialCategoryDiv").html("");

            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='mcategorycb' value='" + json[i].CategoryId + "' /><span>" + json[i].CategoryName + "</span>&nbsp;</div>";

                }
                $("#MaterialCategoryDiv").html(div);

            }
        }
    })
}

function getMonth() {
    GetGuidacneList();
}

function GetCondition() {
    guidanceIds = "";
    subjectIds = "";
    materialCategoryId = "";
    oType = "";
    orderShopNo = "";
    $("input[name='guidanceCB']:checked").each(function () {
        guidanceIds += $(this).val() + ",";
    })
    $("input[name='subjectcb']:checked").each(function () {
        subjectIds += $(this).val() + ",";
    })
    $("input[name='mcategorycb']:checked").each(function () {
        materialCategoryId += $(this).val() + ",";
    })
    $("input[name^='cblOutsourceType']:checked").each(function () {
        oType += $(this).val() + ",";
    })
    exportType = "";
    exportType = $("input[name^='cblExportType']:checked").val() || "";
    materialName = "";
    materialName = $("input[name^='cblMaterial']:checked").val() || "";
    orderShopNo = $.trim($("#txtSearchOrderShopNo").val());
    if (guidanceIds != "") {
        guidanceIds = guidanceIds.substring(0, guidanceIds.length - 1);
    }
    if (subjectIds != "") {
        subjectIds = subjectIds.substring(0, subjectIds.length - 1);
    }
    if (materialCategoryId != "") {
        materialCategoryId = materialCategoryId.substring(0, materialCategoryId.length - 1);
    }
    if (oType != "") {
        oType = oType.substring(0, oType.length - 1);
    }
}

function ClearDiv() {
    $("#subjectDiv").html("");
    $("#MaterialCategoryDiv").html("");
    guidanceIds = "";
    subjectIds = "";
    oType = "";
    materialCategoryId = "";
    orderShopNo = "";
    //$("#tbOrderList").datagrid('loadData',{ total: 0, rows: [] });
    var item = $('#tbOrderList').datagrid('getRows');
    for (var i = item.length - 1; i >= 0; i--) {
        var index = $('#tbOrderList').datagrid('getRowIndex', item[i]);
        $('#tbOrderList').datagrid('deleteRow', index);
    }    
}

//查询上个月活动
$("#spanUp").click(function () {
    var month1 = $.trim($("#txtMonth").val());
    if (month1 != "") {
        month1 = month1.replace(/-/g, "/");
        var date = new Date(month1);
        date.setMonth(date.getMonth() - 1);
        $("#txtMonth").val(date.Format("yyyy-MM"));
        GetGuidacneList();
    }

})

//查询下个月活动
$("#spanDown").click(function () {
    var month1 = $.trim($("#txtMonth").val());
    if (month1 != "") {
        month1 = month1.replace(/-/g, "/");
        var date = new Date(month1);
        date.setMonth(date.getMonth() + 1);
        $("#txtMonth").val(date.Format("yyyy-MM"));
        GetGuidacneList();
    }
})


function doSearchOrder() {
    Order.getOrderList(orderPageIndex, orderPageSize);
}

function Export(type) {
   
    var typeName = "";
    $("input[name^='cblOutsourceType']:checked").each(function () {
        typeName += $(this).next().text() + ",";
    })
    var fileName = currOutsourceName;
    if (typeName != "") {
        typeName = typeName.substring(0, typeName.length - 1);
        fileName = fileName + "(" + typeName + ")";
    }
    //exportType: exportType, materialName: materialName
    var url = "ExportHelper.aspx?type=" + type + "&outsourceId=" + currOutsourceId + "&guidanceIds=" + guidanceIds + "&subjectIds=" + subjectIds + "&outsourceType=" + oType + "&shopNo=" + orderShopNo + "&materialCategoryId=" + materialCategoryId + "&fileName=" + fileName + "&exportType=" + exportType + "&materialName=" + materialName;

    $("#exportFrame").attr("src", url);
}