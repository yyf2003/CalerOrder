


//var guidanceIds = "";
//var subjectIds = "";
//var oType = "";
//var activeTabIndex = 0;
//var firstLoad = 1;
//var materialCategoryId = "";
//var orderShopNo = "";
//var materialName = "";

$(function () {
    GetGuidacneList();



    $("#guidanceDiv").delegate("input[name='guidanceCB']", "change", function () {
        GetRegion();
    });
    $("#RegionDiv").delegate("input[name='regioncb']", "change", function () {

        GetSubjectCategory();
    });
    $("#subjectCategoryDiv").delegate("input[name='categorycb']", "change", function () {

        GetSubjectList();
    })
    $("#subjectDiv").delegate("input[name='subjectcb']", "change", function () {
        GetProvince();
    })

    $("#ProvinceDiv").delegate("input[name='provincecb']", "change", function () {
        GetCity();
    })

    $("#CityDiv").delegate("input[name='citycb']", "change", function () {
       
        GetMaterialCategory();
    })

    $("#guidanceCBALL").click(function () {
        var checked = this.checked;
        $("input[name='guidanceCB']").each(function () {
            this.checked = checked;
        })
        GetRegion();

    })

    $("#subjectCBALL").click(function () {
        var checked = this.checked;
        $("input[name='subjectcb']").each(function () {
            this.checked = checked;
        })
        GetProvince();

    })

    $("#provinceCBAll").click(function () {
        var checked = this.checked;
        $("input[name='provincecb']").each(function () {
            this.checked = checked;
        })
        GetCity();
     
    })

    $("#cityCBAll").click(function () {
        var checked = this.checked;
        $("input[name='citycb']").each(function () {
            this.checked = checked;
        })
        GetMaterialCategory();
    })
})

function GetGuidacneList() {
    var customerId = $("#ddlCustomer").val();
    var guidanceMonth = $.trim($("#txtMonth").val());
    $("#ImgLoadGuidance").show();
    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getGuidanceList", customerId: customerId, guidanceMonth: guidanceMonth },
        complete: function () { $("#ImgLoadGuidance").hide(); },
        success: function (data) {
            $("#guidanceDiv").html("");
            $("#guidanceCBALL").prop("checked", false);
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

function GetRegion() {
    var guidanceIds = "";
    $("input[name='guidanceCB']:checked").each(function () {
        guidanceIds += $(this).val() + ",";
    })
    $("#ImgLoadRegion").show();
    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getRegion", guidanceIds: guidanceIds },
        complete: function () { $("#ImgLoadRegion").hide(); },
        success: function (data) {
            $("#RegionDiv").html("");
            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='regioncb' value='" + json[i].RegionName + "' /><span>" + json[i].RegionName + "</span>&nbsp;</div>";

                }
                $("#RegionDiv").html(div);

            }
            GetSubjectCategory();
        }
    })
}

function GetSubjectCategory() {
    
    var region = "";
    $("input[name='regioncb']:checked").each(function () {
        region += $(this).val() + ",";
    })
    
    $("#ImgLoadSubjectCategory").show();
    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getSubjectCategory", region: region },
        complete: function () { $("#ImgLoadSubjectCategory").hide(); },
        success: function (data) {
            $("#subjectCategoryDiv").html("");
            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='categorycb' value='" + json[i].CategoryId + "' /><span>" + json[i].CategoryName + "</span>&nbsp;</div>";

                }
                $("#subjectCategoryDiv").html(div);

            }
            GetSubjectList();
        }
    })
}

function GetSubjectList() {

    var region = "";
    $("input[name='regioncb']:checked").each(function () {
        region += $(this).val() + ",";
    })
    var category = "";
    $("input[name='categorycb']:checked").each(function () {
        category += $(this).val() + ",";
    })
    
    $("#ImgLoadSubject").show();
    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getSubjectList", region: region, category: category },
        cache: false,
        complete: function () { $("#ImgLoadSubject").hide(); },
        success: function (data) {
            $("#subjectDiv").html("");
            $("#subjectCBALL").prop("checked", false);
            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='subjectcb' value='" + json[i].SubjectId + "' /><span>" + json[i].SubjectName + "</span>&nbsp;</div>";

                }
                $("#subjectDiv").html(div);

            }
            GetProvince();
        }
    })
}

function GetProvince() {
    var region = "";
    $("input[name='regioncb']:checked").each(function () {
        region += $(this).val() + ",";
    })
    var category = "";
    $("input[name='categorycb']:checked").each(function () {
        category += $(this).val() + ",";
    })
    var subjectId = "";
    $("input[name='subjectcb']:checked").each(function () {
        subjectId += $(this).val() + ",";
    })
    $("#ImgLoadProvince").show();
    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getProvince", region: region, category: category, subjectIds: subjectId },
        cache: false,
        complete: function () { $("#ImgLoadProvince").hide(); },
        success: function (data) {
            $("#ProvinceDiv").html("");
            $("#provinceCBAll").prop("checked", false);
            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='provincecb' value='" + json[i].Province + "' /><span>" + json[i].Province + "</span>&nbsp;</div>";

                }
                $("#ProvinceDiv").html(div);

            }
            GetCity();
        }
    })
}

function GetCity() {
    var region = "";
    $("input[name='regioncb']:checked").each(function () {
        region += $(this).val() + ",";
    })
    var category = "";
    $("input[name='categorycb']:checked").each(function () {
        category += $(this).val() + ",";
    })
    var subjectId = "";
    $("input[name='subjectcb']:checked").each(function () {
        subjectId += $(this).val() + ",";
    })
    var province = "";
    $("input[name='provincecb']:checked").each(function () {
        province += $(this).val() + ",";
    })
    $("#ImgLoadCity").show();
    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getCity", region: region, category: category, subjectIds: subjectId, province: province },
        cache: false,
        complete: function () { $("#ImgLoadCity").hide(); },
        success: function (data) {
            $("#CityDiv").html("");
            $("#cityCBAll").prop("checked", false);
            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='citycb' value='" + json[i].City + "' /><span>" + json[i].City + "</span>&nbsp;</div>";

                }
                $("#CityDiv").html(div);

            }
            GetMaterialCategory();
        }
    })
}



function GetMaterialCategory() {
    //GetCondition();
    var region = "";
    $("input[name='regioncb']:checked").each(function () {
        region += $(this).val() + ",";
    })
    var category = "";
    $("input[name='categorycb']:checked").each(function () {
        category += $(this).val() + ",";
    })
    var subjectId = "";
    $("input[name='subjectcb']:checked").each(function () {
        subjectId += $(this).val() + ",";
    })
    var province = "";
    $("input[name='provincecb']:checked").each(function () {
        province += $(this).val() + ",";
    })
    var city = "";
    $("input[name='citycb']:checked").each(function () {
        city += $(this).val() + ",";
    })
    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getMaterialCategory", subjectIds: subjectId, region: region, category: category, province: province, city: city },
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

//查询上个月活动
$("#spanUp").click(function () {
    var month1 = $.trim($("#txtMonth").val());
    if (month1 != "") {
        month1 = month1.replace(/-/g, "/");
        var date = new Date(month1);
        date.setMonth(date.getMonth() - 1);
        $("#txtMonth").val(date.getFullYear()+"-"+(date.getMonth()+1));
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
        $("#txtMonth").val(date.getFullYear() + "-" + (date.getMonth() + 1));
        GetGuidacneList();
    }
})

var pageIndex=1, pageSize = 15;
var Order = {
    getOrderList: function (pageIndex, pageSize) {
        //$("#divContent1").css({ height: "600px" });
       
        var region = "";
        $("input[name='regioncb']:checked").each(function () {
            region += $(this).val() + ",";
        })
        var category = "";
        $("input[name='categorycb']:checked").each(function () {
            category += $(this).val() + ",";
        })
        var subjectId = "";
        $("input[name='subjectcb']:checked").each(function () {
            subjectId += $(this).val() + ",";
        })
        var province = "";
        $("input[name='provincecb']:checked").each(function () {
            province += $(this).val() + ",";
        })
        var city = "";
        $("input[name='citycb']:checked").each(function () {
            city += $(this).val() + ",";
        })
        var materialCategoryId = "";
        $("input[name='mcategorycb']:checked").each(function () {
            materialCategoryId += $(this).val() + ",";
        })
        var oType = "";
        $("input[name^='cblOutsourceType']:checked").each(function () {
            oType += $(this).val() + ",";
        })
        var exportType = $("input[name^='cblExportType']:checked").val() || "";
        
        var materialName = $("input[name^='cblMaterial']:checked").val() || "";

        var shopNo = $.trim($("#txtSearchOrderShopNo").val());

        $("#tbOrderList").datagrid({
            queryParams: { type: "getOrderList", region: region, subjectCategoryIds: category, subjectIds: subjectId, province: province, city: city, materialCategoryId: materialCategoryId, outsourceType: oType, shopNo: shopNo, exportType: exportType, materialName: materialName, currpage: pageIndex, pagesize: pageSize },
            method: 'get',
            url: 'handler/OrderList.ashx',
            columns: [[
                        { field: 'checked', checkbox: true },
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
            singleSelect: false,
            pagination: true,
            pageNumber: pageIndex,
            pageSize: pageSize,
            fitColumns: true,
            iconCls: 'icon-save',

            emptyMsg: '没有相关记录',
            rowStyler: function (index, row) {
                if (row.IsDelete == 1) {
                    return 'color:red';
                }
            },
            onClickRow: function (rowIndex, rowData) {
                $(this).datagrid("unselectRow", rowIndex);

            }
        });
        var p = $("#tbOrderList").datagrid('getPager');
        $(p).pagination({
            showRefresh: false,
            onSelectPage: function (curIndex, curSize) {
                Order.getOrderList(curIndex, curSize);
            }
        });

    }
};

