
$(function () {
    GetOrderList();
    $("#btnSearch").click(function () {
        $("#loadingImg").show();
        GetOrderList();
    })
})


function GetOrderList() {
    var shopno = $.trim($("#txtShopNo").val());
    var shopname = $.trim($("#txtShopName").val());
    $.ajax({
        type: "get",
        url: "handler/CheckOrderDetail.ashx",
        data: { type: 'getList', subjectId: subjectId, shopNo: shopno, shopName: shopname },
        success: function (data) {
            
            if (data != "") {
                var json = eval(data);

                if (json.length > 0) {
                    var tr = "";
                    var index = 0;
                    for (var i in json) {

                        tr += "<tr>";
                        tr += "<td>" + (++index) + "</td>";
                        tr += "<td>" + json[i].OrderType + "</td>";

                        tr += "<td>" + json[i].ShopNo + "</td>";
                        tr += "<td>" + json[i].ShopName + "</td>";
                        tr += "<td>" + json[i].RegionName + "</td>";
                        tr += "<td>" + json[i].ProvinceName + "</td>";
                        tr += "<td>" + json[i].CityName + "</td>";
                        tr += "<td>" + json[i].Format + "</td>";
                        tr += "<td>" + json[i].MaterialSupport + "</td>";
                        tr += "<td>" + json[i].Price + "</td>";
                        tr += "<td>" + json[i].PayPrice + "</td>";
                        tr += "<td>" + json[i].Sheet + "</td>";
                        tr += "<td>" + json[i].Gender + "</td>";
                        tr += "<td>" + json[i].Quantity + "</td>";
                        tr += "<td>" + json[i].GraphicWidth + "</td>";
                        tr += "<td>" + json[i].GraphicLength + "</td>";
                        tr += "<td>" + json[i].GraphicMaterial + "</td>";
                        tr += "<td>" + json[i].PositionDescription + "</td>";
                        tr += "<td>" + json[i].ChooseImg + "</td>";
                        tr += "<td>" + json[i].Remark + "</td>";
                        tr += "<td>" + json[i].AddDate + "</td>";
                        tr += "</tr>";
                    }
                    $("#tbodyOrderData").html(tr).show();
                    $("#tbodyOrderEmpty").hide();
                }
            }
            else {
                $("#tbodyOrderData").hide();
                $("#tbodyOrderEmpty").show();
            }
        },
        complete: function () {
            $("#loadingImg").hide();
        }
    })
}