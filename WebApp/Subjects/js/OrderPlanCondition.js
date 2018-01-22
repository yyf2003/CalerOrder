

$(function () {
    //获取条件
    $.ajax({
        type: "get",
        url: "../Handler/SplitOrder.ashx?type=getCondition&customerId=" + customerId,
        cache: false,
        success: function (data) {
            if (data != "") {

                var json = eval(data);
                if (json.length > 0) {

                    $("#FormatDiv").html(InitCondition("FormatDiv", json[0].Format));
                    $("#MaterialSupportDiv").html(InitCondition("MaterialSupportDiv", json[0].MaterialSupport));
                    $("#InstallDiv").html(InitCondition("InstallDiv", json[0].IsInstall));
                    $("#ScaleDiv").html(InitCondition("ScaleDiv", json[0].POSScale));
                    //$("#SheetDiv").html(InitCondition("SheetDiv", json[0].Sheet));
                    $("#GenderDiv").html(InitCondition("GenderDiv", json[0].Gender));

                }

            }
        }

    })

    //获取位置
    $.ajax({
        type: "get",
        url: "../Handler/SplitOrder.ashx?type=getPosition&customerId=" + customerId,
        cache: false,
        success: function (data) {
            if (data != "") {
                var json = eval(data);
                if (json.length > 0) {
                    var input = "";
                    for (var i = 0; i < json.length; i++) {
                        input += "<input type='radio' name='PositionDivrd' value='" + json[i].Id + "'/><span>" + json[i].PositionName + "</span>&nbsp;";
                    }
                    $("#PositionDiv").html(input);
                }

            }
        }
    })

    //选择位置后获取器架类型
    $("#PositionDiv").delegate("input[name='PositionDivrd']", "change", function () {
        GetMachineFrame("");
    })
})


//初始化条件值
function InitCondition(element, valStr) {
    var input = "";
    if ($.trim(valStr) != "") {
        var arr = valStr.split(',');
        if (arr.length > 0) {

            var input = "";
            var cbName = element + "cb";
            for (var i = 0; i < arr.length; i++) {

                input += "<input type='checkbox' name='" + cbName + "' value='" + arr[i] + "'/>&nbsp;" + arr[i] + "";
            }

        }
    }
    return input;
}

//获取器架类型
function GetMachineFrame(MachineFrameids) {
    $("#MachineFrameDiv").html("");
    var positionid = $("input:radio[name='PositionDivrd']:checked").val() || "0";

    var MachineFrameIdsArr = [];
    if (MachineFrameids.length > 0) {
        MachineFrameIdsArr = MachineFrameids.split(',');

    }

    $.ajax({
        type: "get",
        url: "../Handler/SplitOrder.ashx?type=getMachineFrame&positionId=" + positionid,
        cache: false,
        success: function (data) {

            if (data != "") {
                var json = eval(data);
                if (json.length > 0) {
                    var input = "";
                    for (var i = 0; i < json.length; i++) {
                        var check = "";
                        for (var j = 0; j < MachineFrameIdsArr.length; j++) {
                            if (parseInt(json[i].Id) == parseInt(MachineFrameIdsArr[j])) {

                                check = "checked='checked'";
                            }
                        }
                        input += "<input type='checkbox' name='MachineFrameDivcb' value='" + json[i].Id + "' " + check + "/>&nbsp;<span>" + json[i].CategoryName + "</span>&nbsp;";
                    }
                    $("#MachineFrameDiv").html(input);
                }

            }
        }
    })
}