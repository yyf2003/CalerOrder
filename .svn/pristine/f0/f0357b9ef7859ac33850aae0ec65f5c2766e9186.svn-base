
var JsonStr = "";
var CurrId = 0;//选中的材料ID
var CurrSmallTypeId = 0; //选中的材料小类ID
var CurrBrandId = 0; //选中的材料品牌ID
var CurrCategoryId = 0;
$(function () {
    BindSmallType();
    $("#ddlBigType").on("change", function () {
        $("#hfSmallTypeId").val("0");
        BindSmallType();
    })

    $("#ddlSmallType").on("change", function () {
        $("#hfSmallTypeId").val($(this).val());
    })



    //添加
    $("#btnAdd").click(function () {
        ClearVal();
        CurrId = 0;
        CurrSmallTypeId = 0;
        CurrBrandId = 0;
        CurrCategoryId = 0;
        Material.bindBigTypeList(0);
        Material.bindBrandList();
        Material.bindCategoryList();
        Material.add();
    })

    $("#selBigType").on("change", function () {
        var small = document.getElementById("selSmallType");
        small.length = 1;
        Material.bindSmallTypeList();
    })

    $("#txtWidth,#txtLength").on("blur", function () {
        
        CountArea();
    })
})

function BindSmallType() {
    var pid = $("#ddlBigType").val();
    pid = pid > 0 ? pid : -1;
    var ddlSmallType = document.getElementById("ddlSmallType");
    ddlSmallType.length = 1;
    var smallTypeId = $("#hfSmallTypeId").val() || "0";
    $.ajax({
        type: "get",
        url: "./Handler/Material.ashx?type=getSmallType&parentId=" + pid,
        cache: false,
        success: function (data) {

            if (data != "") {
                var json = eval(data);
                for (var i = 0; i < json.length; i++) {
                    var select = "";
                    if (parseInt(json[i].Id) == parseInt(smallTypeId)) {
                        select = "selected='selected'";
                    }
                    var option = "<option value='" + json[i].Id + "' " + select + ">" + json[i].TypeName + "</option>";
                    $("#ddlSmallType").append(option);
                }
            }

        }
    })

}


function editMaterial(obj) {
    ClearVal();
    var specification = $(obj).parent().siblings("td").eq(5).html();
    var width = $(obj).parent().siblings("td").eq(6).html();
    var length = $(obj).parent().siblings("td").eq(7).html();

    var area = $(obj).parent().siblings("td").eq(8).html();
    var unit = $(obj).parent().siblings("td").eq(9).html();
    var bigTypeId = $(obj).siblings("input[name$='hfBigTypeId']").val()||"0";
    CurrSmallTypeId = $(obj).siblings("input[name$='hfSmallTypeId']").val() || "0";
    CurrCategoryId = $(obj).siblings("input[name$='hfCategoryId']").val() || "0";
    CurrBrandId = $(obj).siblings("input[name$='hfBrandId']").val() || "0";
    CurrId = $(obj).data("materialid");

    $("#txtSpecification").val(specification);
    $("#txtWidth").val(width);
    $("#txtLength").val(length);
    $("#txtArea").val(area);
    $("#txtUnit").val(unit);
    Material.bindBigTypeList(bigTypeId);
    Material.bindBrandList();
    Material.bindCategoryList();
    Material.edit();
}



var Material = {
    bindBigTypeList: function (id) {
        $.ajax({
            type: "get",
            url: "./Handler/Material.ashx?type=getBigType",
            cache: false,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var select = "";
                        if (parseInt(json[i].Id) == parseInt(id)) {
                            select = "selected='selected'";
                        }
                        var option = "<option value='" + json[i].Id + "' " + select + ">" + json[i].TypeName + "</option>";
                        $("#selBigType").append(option);

                    }
                    Material.bindSmallTypeList();
                }

            }
        })
    },
    bindSmallTypeList: function () {
        var pid = $("#selBigType").val();
        pid = pid == 0 ? -1 : pid;
        $.ajax({
            type: "get",
            url: "./Handler/Material.ashx?type=getSmallType&parentId=" + pid,
            cache: false,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var select1 = "";
                        if (parseInt(json[i].Id) == parseInt(CurrSmallTypeId)) {
                            select1 = "selected='selected'";

                        }
                        var option1 = "<option value='" + json[i].Id + "' " + select1 + ">" + json[i].TypeName + "</option>";
                        $("#selSmallType").append(option1);
                    }
                }

            }
        })
    },
    bindCategoryList: function () {
        $.ajax({
            type: "get",
            url: "./Handler/Material.ashx?type=getCategory",
            cache: false,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var select1 = "";
                        if (parseInt(json[i].Id) == parseInt(CurrCategoryId)) {
                            select1 = "selected='selected'";

                        }
                        var option1 = "<option value='" + json[i].Id + "' " + select1 + ">" + json[i].CategoryName + "</option>";
                        $("#selCategory").append(option1);
                    }
                }

            }
        })
    },
    bindBrandList: function () {
        $.ajax({
            type: "get",
            url: "./Handler/Material.ashx?type=getBrand",
            cache: false,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var select1 = "";
                        if (parseInt(json[i].Id) == parseInt(CurrBrandId)) {
                            select1 = "selected='selected'";

                        }
                        var option1 = "<option value='" + json[i].Id + "' " + select1 + ">" + json[i].BrandName + "</option>";
                        $("#selBrand").append(option1);
                    }
                }

            }
        })
    },
    add: function () {
        $("#editDiv").show().dialog({
            modal: true,
            width: 480,
            height: 400,
            iconCls: 'icon-add',
            title: '添加材料',
            resizable: false,
            buttons: [

                    {
                        text:  '添加',
                        iconCls: 'icon-add',
                        visible: false,
                        handler: function () {
                            if (CheckVal()) {
                                $.ajax({
                                    type: "get",
                                    url: "./Handler/Material.ashx?type=edit&optype=add&jsonString=" + escape(JsonStr),
                                    cache: false,
                                    success: function (data) {
                                        if (data == "exist") {
                                            alert("该材料已存在！");

                                        }
                                        else if (data == "ok") {
                                            //alert("提交成功！");

                                            window.location.href = "MaterialList.aspx";
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
                            $("#editDiv").dialog('close');
                        }
                    }
                ]
        });
    },
    edit: function () {
        $("#editDiv").show().dialog({
            modal: true,
            width: 480,
            height: 400,
            iconCls: 'icon-edit',
            title:  '编辑材料',
            resizable: false,
            buttons: [

                    {
                        text:  '更新',
                        iconCls: 'icon-edit',
                        visible: false,
                        handler: function () {
                            if (CheckVal()) {
                                $.ajax({
                                    type: "get",
                                    url: "./Handler/Material.ashx?type=edit&optype=update&jsonString=" + escape(JsonStr),
                                    cache: false,
                                    success: function (data) {
                                        if (data == "exist") {
                                            alert("该材料已存在！");

                                        }
                                        else if (data == "ok") {
                                            //alert("提交成功！");

                                            window.location.href = "MaterialList.aspx";
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
                                    url: "./Handler/Material.ashx?type=edit&optype=add&jsonString=" + escape(JsonStr),
                                    cache: false,
                                    success: function (data) {
                                        if (data == "exist") {
                                            alert("该材料已存在！");

                                        }
                                        else if (data == "ok") {
                                            //alert("提交成功！");

                                            window.location.href = "MaterialList.aspx";
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
                            $("#editDiv").dialog('close');
                        }
                    }
                ]
        });


        //Material.add("update");
    }
}



function CheckVal() {
    var bigTypeId = $("#selBigType").val();
    var smallTypeId = $("#selSmallType").val();
    var categoryId = $("#selCategory").val();
    var brandId = $("#selBrand").val();
    if (bigTypeId == "0") {
        alert("请选择材料大类");
        return false;
    }
    if (smallTypeId == "0") {
        alert("请选择材料小类");
        return false;
    }
    if (categoryId == "0") {
        alert("请选择材质类型");
        return false;
    }
    if ($.trim($("#txtSpecification").val()) == "") {
        alert("请填写材质名称");
        return false;
    }
    if ($.trim($("#txtWidth").val()) == "") {
        alert("请填写材料宽");
        return false;
    }
    if ($.trim($("#txtLength").val()) == "") {
        alert("请填写材料高");
        return false;
    }
    var width = $.trim($("#txtWidth").val()) || 0;
    var length = $.trim($("#txtLength").val()) || 0;
    var area = $.trim($("#txtArea").val()) || 0;
    if (isNaN(width)) {
        $("#msgWidth").html("请填写数值");
        return false;
    }
    if (isNaN(length)) {
        $("#msgLength").html("请填写数值");
        return false;
    }
    JsonStr = '{"Id":' + CurrId + ',"BigTypeId":' + bigTypeId + ',"SmallTypeId":' + smallTypeId + ',"MaterialCategoryId":' + categoryId + ',"MaterialBrandId":' + brandId + ',"MaterialName":"' + $.trim($("#txtSpecification").val()) + '","Width":' + width + ',"Length":' + length + ',"Area":' + area + ',"Unit":"' + $.trim($("#txtUnit").val()) + '"}';
    return true;
}

function ClearVal() {
    var small = document.getElementById("selSmallType");
    small.length = 1;
    var big = document.getElementById("selBigType");
    big.length = 1;
    var brand = document.getElementById("selBrand");
    brand.length = 1;
    document.getElementById("selCategory").length = 1;
//    $("#txtName").val("");
//    $("#txtWidth").val("");
//    $("#txtLength").val("");
//    $("#txtArea").val("");
//    $("#txtUnit").val("");
    $("#msgWidth").html("");
    $("#msgLength").html("");
    $("#editDiv").find("input").val("");
    
    JsonStr = "";
}

function CountArea() {
    var width = $("#txtWidth").val()||"0";
    var length = $("#txtLength").val()||"0";
    $("#txtArea").val("");
    var flag1 = true;
    var flag2 = true;
    if (isNaN(width)) {
        $("#msgWidth").html("请填写数值");
        flag1 = false;
    }
    else {
        $("#msgWidth").html("");
    }
    if (isNaN(length)) {
        $("#msgLength").html("请填写数值");
        flag2 = false;
    }
    else {
        $("#msgLength").html("");

    }
    if (flag1&&flag2) {
        var area = parseFloat(width) * parseFloat(length) / 1000000;
        $("#txtArea").val(area.toFixed(2));
    }
}

