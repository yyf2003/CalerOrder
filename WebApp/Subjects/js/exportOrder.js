



//var subjectId = "";
var region = "";
var province = "";
var city = "";
$(function () {
    GetRegoin();
    //选择区域
    $("#RegionDiv").delegate("input[name='regionCB']", "change", function () {

        ChangeRegion();
    })
    //选择省份
    $("#ProvinceDiv").delegate("input[name='provinceCB']", "change", function () {
        ChangeProvince();

    })

    //选择城市
    $("#CityDiv").delegate("input[name='cityCB']", "change", function () {
        var city = "";
        $("#CityDiv input[name='cityCB']:checked").each(function () {
            city += $(this).val() + ",";
        })
        if (city != "") {
            city = city.substring(0, city.length - 1);
        }
        $("#hfCity").val(city);
    })


    $("#btnExport").click(function () {

        GetCondition();
        $(this).attr("disabled", true).next("img").show();
        checkExport($(this));
        var url = "Handler/ExportOrders.ashx?type=export&subjectids=" + subjectId + "&regions=" + region + "&province=" + province + "&city=" + city;
        $("#exportFrame").attr("src", url);
    })


    //导出350
    $("#btnExport350").click(function () {
        GetCondition();
        if (subjectId == "") {
            alert("请选择项目");
            return false;
        }

        $(this).attr("disabled", true).next("img").show();
        checkExport350($(this));
        var url = "Handler/ExportOrders.ashx?type=export350&subjectids=" + subjectId + "&regions=" + region + "&province=" + province + "&city=" + city;
        $("#exportFrame").attr("src", url);
    })
})

var timer;
function checkExport(obj) {
    timer = setInterval(function () {
        $.ajax({
            type: "get",
            url: "Handler/CheckExport.ashx",
            cache: false,
            success: function (data) {

                if (data == "ok") {
                    $(obj).attr("disabled", false).next("img").hide();
                    clearInterval(timer);
                }

            }
        })

    }, 1000);
}

var timer1;
function checkExport350(obj) {
    timer1 = setInterval(function () {
        $.ajax({
            type: "get",
            url: "Handler/CheckExport350.ashx",
            cache: false,
            success: function (data) {

                if (data == "ok") {
                    $(obj).attr("disabled", false).next("img").hide();
                    clearInterval(timer1);
                }

            }
        })

    }, 1000);
}


function GetRegoin() {

    $.ajax({
        type: "get",
        url: "Handler/Orders.ashx",
        data: { type: "getRegion", subjectIds: subjectId },
        success: function (data) {
            $("#RegionDiv").html("");

            if (data != "") {

                var json = eval(data);
                var regions = $("#hfRegion").val() || "";
                var arr = [];
                if (regions.length > 0) {
                    arr = regions.split(',');
                }
                for (var i = 0; i < json.length; i++) {
                    var check = "";
                    if (arr.length > 0) {
                        for (var j = 0; j < arr.length; j++) {
                            if (arr[j] == json[i].Region)
                                check = "checked='checked'";
                        }
                    }
                    var div = "<input type='checkbox' name='regionCB' value='" + json[i].Region + "' " + check + "/><span>" + json[i].Region + "</span>&nbsp;";
                    $("#RegionDiv").append(div);
                }
                if(arr.length>0)
                    ChangeRegion();
            }
        }

    })
}

function ChangeRegion() {

    GetProvince();
    
}

function GetProvince() {
    var regions = "";
    $("#ProvinceDiv").html("");
    $("#CityDiv").html("");
    $("#RegionDiv input[name='regionCB']:checked").each(function () {
        regions += $(this).val() + ",";
    })
    if (regions != "") {
        regions = regions.substring(0, regions.length - 1);
    }
    $("#hfRegion").val(regions);
    $.ajax({
        type: "get",
        url: "Handler/Orders.ashx",
        data: { type: "getProvince", region: regions, subjectIds: subjectId },
        success: function (data) {
            
            if (data != "") {

                var provinces = $("#hfProvince").val() || "";
                var arr = [];
                if (provinces.length > 0) {
                    arr = provinces.split(',');
                }
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    var check = "";
                    if (arr.length > 0) {
                        for (var j = 0; j < arr.length; j++) {
                            if (arr[j] == json[i].Province)
                                check = "checked='checked'";
                        }
                    }
                    div += "<div style='width:70px;float:left;'>";
                    div += "<input type='checkbox' name='provinceCB' value='" + json[i].Province + "' " + check + "/><span>" + json[i].Province + "</span>&nbsp;";
                    div += "</div>";

                }
                $("#ProvinceDiv").html(div);
                if (arr.length > 0)
                    ChangeProvince();
            }

        }

    })
}

function ChangeProvince() {
    GetCity();
    
}

function GetCity() {
    var provinces = "";
    $("#CityDiv").html("");
    $("#ProvinceDiv input[name='provinceCB']:checked").each(function () {
        provinces += $(this).val() + ",";
    })
    if (provinces != "") {
        provinces = provinces.substring(0, provinces.length - 1);
    }
    $("#hfProvince").val(provinces);
    $.ajax({
        type: "get",
        url: "Handler/Orders.ashx",
        data: { type: "getCity", province: provinces, subjectIds: subjectId },
        success: function (data) {
            

            if (data != "") {
                var cities = $("#hfCity").val() || "";
                var arr = [];
                if (cities.length > 0) {
                    arr = cities.split(',');
                }

                var json = eval(data);
                var div = "";

                for (var i = 0; i < json.length; i++) {
                    var check = "";
                    if (arr.length > 0) {
                        for (var j = 0; j < arr.length; j++) {
                            if (arr[j] == json[i].City)
                                check = "checked='checked'";
                        }
                    }
                    div += "<div style='float:left;'>";
                    div += "<input type='checkbox' name='cityCB' value='" + json[i].City + "' " + check + "/><span>" + json[i].City + "</span>&nbsp;";
                    div += "</div>";
                }
                
                $("#CityDiv").html(div);

            }

        }

    })
}


function GetCondition() {
    
    region = "";
    $("input[name='regionCB']:checked").each(function () {
        region += $(this).val() + ",";
    })
    province = "";
    $("input[name='provinceCB']:checked").each(function () {
        province += $(this).val() + ",";
    })
    city = "";
    $("input[name='cityCB']:checked").each(function () {
        city += $(this).val() + ",";
    })

   
   
    if (region.length > 0) {
        region = region.substring(0, region.length - 1);
    }
    if (province.length > 0) {
        province = province.substring(0, province.length - 1);
    }
    if (city.length > 0) {
        city = city.substring(0, city.length - 1);
    }
    
}