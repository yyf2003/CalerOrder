

$(function () {

    GetCity();

    $("#ddlProvince").on("change", function () {
        $("#hfCityId").val("0");
        GetCity();
    })

    //选择城市
    $("#ddlCity").on("change", function () {
        var cityId = $(this).val();
        if ($("#hfCityName")) {
            $("#hfCityName").val($("#ddlCity option:checked").text());
        }
        
        $("#hfCityId").val(cityId);
    })


})

function GetCity() {
    var pid = $("#ddlProvince").val() || 0;
    var city = document.getElementById("ddlCity");
    city.length = 1;
    $.ajax({
        type: "get",
        url: "../../Handler/GetCity.ashx?provinceid=" + pid,
        cache: false,
        success: function (data) {
            if (data != "") {
                var json = eval(data);
                for (var i = 0; i < json.length; i++) {
                    var cityid = $("#hfCityId").val() || 0;
                    var select = cityid == json[i].ID ? "selected='selected'" : "";
                    var option = "<option " + select + " value='" + json[i].ID + "'>" + json[i].PlaceName + "</option>";
                    $("#ddlCity").append(option);
                }
            }
        }
    })
}