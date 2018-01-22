

var PriceJsonStr = "";
var CurrPriceId = 0; //选中的材料报价ID
var CurrCustomerId = 0; //选中的报价的客户ID
var CurrRegionId = 0; //
var CurrBigTypeId = 0; //选中的材料小类ID
var CurrSmallTypeId = 0; //选中的材料小类ID

var Price = {
    bindCustomer: function () {
        $.ajax({
            type: "get",
            url: "./Handler/SetPrice.ashx?type=getCustomer",
            cache: false,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var select = "";
                        if (parseInt(json[i].Id) == parseInt(CurrCustomerId)) {
                            select = "selected='selected'";
                        }
                        var option = "<option value='" + json[i].Id + "' " + select + ">" + json[i].CustomerName + "</option>";
                        $("#selCustomer").append(option);

                    }
                    Price.bindRegion();
                }

            }
        })
    },
    bindRegion: function () {
        var pid = $("#selCustomer").val();
        document.getElementById("selRegion").length = 1;
        $.ajax({
            type: "get",
            url: "./Handler/SetPrice.ashx?type=getRegion&customerId=" + pid,
            cache: false,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var select1 = "";
                        if (parseInt(json[i].Id) == parseInt(CurrRegionId)) {
                            select1 = "selected='selected'";

                        }
                        var option1 = "<option value='" + json[i].Id + "' " + select1 + ">" + json[i].RegionName + "</option>";
                        $("#selRegion").append(option1);
                    }
                }

            }
        })
    },
    bindBigType: function () {
//        $.ajax({
//            type: "get",
//            url: "./Handler/Material.ashx?type=getBigType",
//            cache: false,
//            success: function (data) {
//                if (data != "") {
//                    var json = eval(data);
//                    for (var i = 0; i < json.length; i++) {
//                        var select = "";
//                        if (parseInt(json[i].Id) == parseInt(CurrBigTypeId)) {
//                            select = "selected='selected'";
//                        }
//                        var option = "<option value='" + json[i].Id + "' " + select + ">" + json[i].TypeName + "</option>";
//                        $("#selBigType").append(option);

//                    }
//                    Price.bindSmallType();
//                }

//            }
//        })
    },
    bindSmallType: function () {
//        var pid = $("#selBigType").val();
//        document.getElementById("selSmallType").length = 1;
//        pid = pid == 0 ? -1 : pid;
//        $.ajax({
//            type: "get",
//            url: "./Handler/Material.ashx?type=getSmallType&parentId=" + pid,
//            cache: false,
//            success: function (data) {
//                if (data != "") {
//                    var json = eval(data);
//                    for (var i = 0; i < json.length; i++) {
//                        var select1 = "";
//                        if (parseInt(json[i].Id) == parseInt(CurrSmallTypeId)) {
//                            select1 = "selected='selected'";

//                        }
//                        var option1 = "<option value='" + json[i].Id + "' " + select1 + ">" + json[i].TypeName + "</option>";
//                        $("#selSmallType").append(option1);
//                    }
//                }
//                InitData(currPage, pageSize);
//            }
//        })
    },
    addPrice: function () {
        $("#editPriceDiv").show().dialog({
            modal: true,
            width: 820,
            height: 450,
            iconCls: 'icon-add',
            title: '添加报价',
            resizable: false,
            buttons: [

                    {
                        text: '添加',
                        iconCls: 'icon-add',
                        visible: false,
                        handler: function () {
                            
                            if (CheckVal()) {
                                $.ajax({
                                    type: "get",
                                    url: "./Handler/SetPrice.ashx?type=edit&optype=add&jsonString=" + urlCodeStr(PriceJsonStr),
                                    cache: false,
                                    success: function (data) {
                                        if (data == "exist") {
                                            alert("该材料的报价已存在！");

                                        }
                                        else if (data == "ok") {
                                            //alert("提交成功！");
                                            var str = $("#searchTab input,select").serialize();

                                            window.location.href = "SetPrice.aspx?urlStr=" + escape(str);
                                        }
                                        else
                                            alert(data);
                                    }
                                })
                            }
                        }
                    },

                    {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#editPriceDiv").dialog('close');
                        }
                    }
                ]
        });
    },
    editPrice: function () {
        $("#editPriceDiv").show().dialog({
            modal: true,
            width: 820,
            height: 450,
            iconCls: 'icon-edit',
            title: '编辑报价',
            resizable: false,
            buttons: [

                    {
                        text: '更新',
                        iconCls: 'icon-edit',
                        visible: false,
                        handler: function () {
                            if (CheckVal()) {
                                $.ajax({
                                    type: "get",
                                    url: "./Handler/SetPrice.ashx?type=edit&optype=update&jsonString=" + urlCodeStr(PriceJsonStr),
                                    cache: false,
                                    success: function (data) {
                                        if (data == "exist") {
                                            alert("该材料的报价已存在！");

                                        }
                                        else if (data == "ok") {
                                            //alert("提交成功！");
                                            var str = $("#searchTab input,select").serialize();
                                            window.location.href = "SetPrice.aspx?urlStr=" + escape(str);
                                        }
                                        else
                                            alert(data);
                                    }
                                })
                            }
                        }
                    },
                    {
                        text: '添加',
                        iconCls: 'icon-add',
                        visible: false,
                        handler: function () {
                            if (CheckVal()) {
                                $.ajax({
                                    type: "get",
                                    url: "./Handler/SetPrice.ashx?type=edit&optype=add&jsonString=" + escape(PriceJsonStr),
                                    cache: false,
                                    success: function (data) {
                                        if (data == "exist") {
                                            alert("该材料的报价已存在！");

                                        }
                                        else if (data == "ok") {
                                            //alert("提交成功！");
                                            var str = $("#searchTab input,select").serialize();
                                            window.location.href = "SetPrice.aspx?urlStr=" + escape(str);
                                        }
                                        else
                                            alert(data);
                                    }
                                })
                            }
                        }
                    },
                    {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#editPriceDiv").dialog('close');
                        }
                    }
                ]
        });
    }
}


$(function () {
    $("#btnAdd").click(function () {
        ClearVal();
        CurrCustomerId = $("#ddlCustomer").val();
        CurrRegionId = $("#ddlRegion").val();
        //CurrBigTypeId = $("#ddlBigType").val();
        //CurrSmallTypeId = $("#ddlSmallType").val();
        Price.bindCustomer();
        //Price.bindBigType();
        Price.addPrice();
        
    })

    $("#selCustomer").on("change", function () {
        Price.bindRegion();
    })
//    $("#selBigType").on("change", function () {
//        Price.bindSmallType();

//    })

//    $("#selSmallType").on("change", function () {

//        InitData(currPage, pageSize);
    //    })

    InitData(currPage, pageSize);
})


function editPrice(obj) {
    ClearVal();
    CurrPriceId = $(obj).data("priceid");

    $.ajax({
        type: "get",
        url: "./Handler/SetPrice.ashx?type=getPrice&priceId=" + CurrPriceId,
        cache: false,
        success: function (data) {

            if (data != "") {
                var json = eval(data);

                CurrCustomerId = json[0].CustomerId;
                CurrRegionId = json[0].RegionId;
                CurrBigTypeId = json[0].BigTypeId;
                $("#txtCustomerMaterial").val(json[0].CustomerMaterialName);
                //$("#txtPrintPrice").val(json[0].PrintPrice);
                $("#txtCostPrice").val(json[0].CostPrice);
                $("#txtSalePrice").val(json[0].SalePrice);
                //materialJson
                if (json[0].Materials != null && json[0].Materials.length > 0) {
                    categoryJson = json[0].Materials;
                    
                    CreateMaterialDiv();
                }
            }
            Price.bindCustomer();
            //Price.bindBigType();
            Price.editPrice();
        }
    })
    
   
    
}

function ClearVal() {
    PriceJsonStr = "";
    CurrPriceId = 0;
    CurrCustomerId = 0;
    CurrRegionId = 0;
    CurrBigTypeId = 0;
    CurrSmallTypeId = 0;
    document.getElementById("selCustomer").length = 1;
    document.getElementById("selRegion").length = 1;
//    document.getElementById("selBigType").length = 1;
//    document.getElementById("selSmallType").length = 1;
    $("#editPriceDiv input").val("");
}

function CheckVal() {
    var customerId=$("#selCustomer").val();
    if (customerId == "0") {
        alert("请选择客户");
        return false;
    }
    var regionId = $("#selRegion").val();
    var regionName = $("#selRegion option:selected").text();
    
    if (regionId == "0") {
        alert("请选择区域");
        return false;
    }

    var customerMaterial = $("#txtCustomerMaterial").val();
    if ($.trim(customerMaterial) == "") {
        alert("客户材质名称");
        return false;
    }
//    if ($("#selBigType").val() == "0") {
//        alert("请选择大类");
//        return false;
//    }
//    if ($("#selSmallType").val() == "0") {
//        alert("请选择小类");
//        return false;
    //    }
    //var printPrice = $.trim($("#txtPrintPrice").val());
    var costPrice = $.trim($("#txtCostPrice").val());
    var salePrice = $.trim($("#txtSalePrice").val());
//    if (cost == "") {
//        alert("请填写成本价");
//        return false;
    //    }

//    if ($.trim(printPrice) != "" && isNaN(printPrice)) {
//        alert("机器打印价必须是数值");
//        return false;
//    }
    if ($.trim(costPrice)!="" && isNaN(costPrice)) {
        alert("成本价必须是数值");
        return false;
    }
    if (salePrice == "") {
        alert("请填写销售价格");
        return false;
    }
    if (isNaN(salePrice)) {
        alert("销售价必须是数值");
        return false;
    }
//    var bigTypeId = $("#selBigType").val();
//    var bigTypeName = $("#selBigType option:selected").text();
   
    //PriceJsonStr = '{"Id":' + CurrPriceId + ',"CustomerId":' + $("#selCustomer").val() + ',"RegionId":' + $("#selRegion").val() + ',"MaterialTypeId":' + $("#selSmallType").val() + ',"CostPrice":' + cost + ',"SalePrice":' + price + '}';
    //材料
    var materilJsonStr = "";
    if (categoryJson.length > 0)
        materilJsonStr = JSON.stringify(categoryJson);
    else {
        //alert("请选择材料");
        //return false;
        materilJsonStr = "[]";
    }
    
    //PriceJsonStr = '{"Id":' + CurrPriceId + ',"CustomerId":' + customerId + ',"BigTypeId":' + bigTypeId + ',"CustomerMaterialName":"' + customerMaterial + '","RegionId":' + regionId + ',"RegionName":"' + regionName + '","PrintPrice":' + printPrice + ',"CostPrice":' + costPrice + ',"SalePrice":' + salePrice + '';
    PriceJsonStr = '{"Id":' + CurrPriceId + ',"CustomerId":' + customerId + ',"CustomerMaterialName":"' + customerMaterial + '","RegionId":' + regionId + ',"RegionName":"' + regionName + '","CostPrice":' + costPrice + ',"SalePrice":' + salePrice + '';
    
    if (materilJsonStr.length>0)
        PriceJsonStr += ',"Materials":' + materilJsonStr;
    PriceJsonStr += '}';
    
    return true;
}


//js分页
var bigTypeId = 0;
var smallTypeId = 0;
var pageSize = 30;
var currPage = 0;
function InitData(pageindx, pagesize) {
//    bigTypeId = $("#selBigType").val();
//    smallTypeId = $("#selSmallType").val();
    //ajax获取数据
    $.ajax({
        type: "get", //用GET方式传输
        //dataType: "json", //数据格式:JSON
        //url: "./Handler/Material.ashx?type=getMaterialByType&bigTypeId=" + bigTypeId + "&smallTypeId=" + smallTypeId + "&currPage=" + pageindx + "&pageSize=" + pagesize, //目标地址
        url: "./Handler/SetPrice.ashx?type=getMaterialCategory&currPage=" + pageindx + "&pageSize=" + pagesize, //目标地址
        //cache: false,
        beforeSend: function () { $("#divload").show(); }, //发送数据之前
        complete: function () { $("#divload").hide(); $("#materialData").show(); $("#Pagination").show() }, //接收数据完毕
        success: function (list) {

            if (list != "") {

                var json = eval(list);
                var total = json[0].total;
                var tr = "<tr class='tr_bai'><td style='text-align:left;padding:10px;'>";
                for (var i = 0; i < json.length; i++) {
                    //var rowIndex = pageindx * pagesize + i + 1;
                    //                    tr += "<tr class='tr_bai'>";
                    //                    tr += "<td><input type='checkbox' name='cbOne' value='" + json[i].MaterialId + "' data-materialname='" + json[i].MaterialName + "'/></td>";
                    //                    tr += "<td>" + rowIndex + "</td>";
                    //                    tr += "<td>" + json[i].MaterialName + "</td>";
                    //                    tr += "<td>" + json[i].BigTypeName + "</td>";
                    //                    tr += "<td>" + json[i].SmallTypeName + "</td>";
                    //                    tr += "<td>" + json[i].BrandName + "</td>";
                    //                    tr += "<td>" + json[i].Length + "</td>";
                    //                    tr += "<td>" + json[i].Width + "</td>";
                    //                    tr += "<td>" + json[i].Area + "</td>";
                    //                    tr += "<td>" + json[i].Unit + "</td>";
                    //                    tr += "</tr>";

                    tr += "<div style='width:120px;float:left;margin-right:10px;'><input type='checkbox' name='cbOne11' value='" + json[i].CategoryId + "'/>&nbsp;<span data-cid='" + json[i].CategoryId + "'>" + json[i].CategoryName + "</span></div>";

                }
                tr += "<td></tr>";
                $("#materialBody").html(tr);
                
                var strJS = "pageOperate(" + pageindx + "," + pagesize + "," + total + ")";
                eval(strJS);
            }
            else {
                var tr = "<tr class='tr_bai'><td>--无符合条件的数据--</td></tr>";
                $("#materialBody").html(tr);
            }
        }
    })
}

function pageselectCallback(page_id) {
    InitData(page_id, pageSize);
}


function pageOperate(currPage, pageSize, total) {
    $("#Pagination").pagination(total, {
        callback: pageselectCallback,
        prev_text: '上一页',
        next_text: '下一页',
        items_per_page: pageSize,
        num_display_entries: 6,
        current_page: currPage,
        num_edge_entries: 2
    });
}

var categoryJson = [];
$(function () {
    $("#materialBody").delegate("input[name='cbOne11']", "change", function () {

       
        var mId = $(this).next("span").data("cid");
        var mName = $(this).next("span").html();

        if (this.checked) {

            AddToMaterialJson(mId, mName);
        }
        else {
            removeFromMaterialJson(mId);
        }
        CreateMaterialDiv();
    })

    //删除材料
    $("#materialContentx").delegate("span[name='deleteMaterial']", "click", function () {
        var mId = $(this).data("categoryid");
        removeFromMaterialJson(mId);
        CreateMaterialDiv();
    })
})

function AddToMaterialJson(categoryId, categoryName) {
    
    var canPush = true;
    if (categoryJson.length > 0) { 
        $.each(categoryJson, function (key, val) {
            if (val.MarterialCategoryId == categoryId)
                canPush = false;
        })
    
    }
    
    if (canPush) {
        
        var obj = $.parseJSON('{"MarterialCategoryId":"' + categoryId + '","MarterialCategoryName":"' + categoryName + '"}');
        categoryJson.push(obj);
    }
    
}

function removeFromMaterialJson(mId) {
    var index = 0;
    var i=0;
    var canRemove = false;
    $.each(categoryJson, function (key, val) {
        if (parseInt(val.MarterialCategoryId) == parseInt(mId)) {
            canRemove = true;
            index=i;
        }
        i++;
    })
    if (canRemove) {
        categoryJson.splice(index, 1);
    }
}

function CreateMaterialDiv() {
    var div = "";
   
    for (var i = 0; i < categoryJson.length; i++) {
       
        div += "<div style='float:left;margin-right:10px;'>" + categoryJson[i].MarterialCategoryName + "<span name='deleteMaterial' data-categoryid='" + categoryJson[i].MarterialCategoryId + "' style='color:red; cursor:pointer;'>×</span></div>";

    }
    
    $("#materialContentx").html(div);
}