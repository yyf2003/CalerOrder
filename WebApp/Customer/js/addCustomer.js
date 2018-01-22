//省份json对象，
var provinceJson = {};
var customerJsonStr = "";
var currProvinceIds = "";
var isCopy = false;
$(function () {
    region.getProvince();
    if (currCustomerId > 0) {
        region.loadRegion(currCustomerId);
    }
    $("#btnAddRegion").on("click", function () {
        region.addRegion();
    })

    //删除区域
    $("#regionDiv").delegate("span[name='deleteRegion']", "click", function () {

        $(this).parent().parent().parent().remove();
    })

    //复制区域
    $("#spanCopy").on("click", function () {
        var otherCurstomerId = $("#ddlOtherCustomer").val();
        if (otherCurstomerId == "0") {
            alert("请选择其他客户");
        }
        else {
            isCopy = true;
            region.loadRegion(otherCurstomerId);
        }
    })

    //取消复制
    $("#spanCancelCopy").on("click", function () {
        var otherCurstomerId = currCustomerId || 0;
        isCopy = false;
        region.loadRegion(otherCurstomerId);
    })

    //提交客户（保存）
    $("#btnSubmit").on("click", function () {
        if (CheckCustomerVal()) {
            var optype = "add";
            if (currCustomerId > 0)
                optype = "update";
            $.ajax({
                type: "post",
                url: "./Handler/AddCustomer.ashx?type=add&optype=" + optype,
                data: { jsonStr: escape(customerJsonStr) },
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        alert("提交成功！");
                        window.location = "./CustomerList.aspx";
                    }
                    else {
                        if (isCopy) {
                            alert("提交失败！");
                            isCopy = false;
                        }

                    }
                }
            })
        }
    })
})

var region = {
    getProvince: function () {
        var province = $("#hfProvinces").val();
        if (province != "")
            provinceJson = eval(province);
    },
    addRegion: function () {
        var regionName = $("#txtRegionName").val();
        if ($.trim(regionName) != "") {
            {
                CreateRegionDiv(regionName, {});
                $("#txtRegionName").val("");
            }
        }
        else {
            alert("请输入区域名称");
        }
    },
    loadRegion: function (customerId1) {
        
        $.getJSON("./Handler/AddCustomer.ashx?type=getRegion&customerId=" + customerId1 + "&t=" + new Date().getTime(), function (data) {
            if (data.length > 0) {
                $("#regionDiv").html("");
                for (var i = 0; i < data.length; i++) {
                    var regionName = data[i].RegionName;
                    //currProvinceIds = data[i].ProvinceId||"";
                    CreateRegionDiv(regionName, data[i]);
                }
            }
            else {
                alert("该客户还没设置区域信息");
            }
        })
    }
}

function CreateProvinceDiv(json) {
    var div = "";
    var arr = [];

    if (json != null) {
        
        var pids=json.ProvinceId||"";
        if(pids.length>0)
          arr = pids.split(',');
    }
    if (provinceJson.length > 0) {
        for (var i = 0; i < provinceJson.length; i++) {
            var check = "";
            if (arr.length > 0) {
                for (var j = 0; j < arr.length; j++) {
                    if (parseInt(arr[j]) == parseInt(provinceJson[i].ProvinceId))
                        check = "checked='checked'";
                }
            }
            div += "<div style='width:100px;float:left;'><input type='checkbox' name='provinceCB' value='" + provinceJson[i].ProvinceId + "' " + check + "/>" + provinceJson[i].ProvinceName + "&nbsp;"
            div += "</div>";
        }
    }
    return div;
}

function CreateRegionDiv(regionName,json) {
    var div = "<table class='table'  style='margin-bottom:8px;'>";
    div += "<tr class='tr_hui'><td style='width:120px;'>区域名称</td><td style='text-align:left;padding-left:5px;'><input type='text' value='" + regionName + "' name='txtRegionVal' />&nbsp;&nbsp;<span name='deleteRegion' style='color:red;cursor:pointer;'>删除</span><td></tr>";
    div += "<tr class='tr_bai'><td>省份</td><td style='text-align:left;padding:5px;'>" + CreateProvinceDiv(json) + "<td></tr>";
    div += "</table>";
    $("#regionDiv").append(div);

}

function CheckCustomerVal() {
    customerJsonStr = "";
    var customerCode = $("#txtCode").val();
    var customerName = $("#txtCustomerName").val();
    var shortName = $("#txtShortName").val();
    var contact = $("#txtContact").val();
    var tel = $("#txtTel").val();
    if ($.trim(customerName) == "") {
        alert("请填写客户名称");
        return false;
    }
    customerJsonStr = '{"Id":' + currCustomerId + ',"CustomerCode":"' + customerCode + '","CustomerName":"' + customerName + '","CustomerShortName":"' + shortName + '","Contact":"' + contact + '","Tel":"' + tel + '"';
    var region = CheckRegionVal();
    if (region.length > 0) {
        customerJsonStr += ',Regions:[' + region + ']';
    }
    customerJsonStr += '}';
    
    return true;
}

function CheckRegionVal() {
    var region = $("#regionDiv");
    var jsonstr = "";
    if (region.html() != "") {
        $(region).find("table").each(function () {
            //var regionName = $(this).find("tr").eq(0).find("span[name='spanregion']").html();txtRegionVal
            var regionName = $(this).find("tr").eq(0).find("input[name='txtRegionVal']").val();
            if ($.trim(regionName) != "") {
                var provinceTr = $(this).find("tr").eq(1);
                var provinceIds = "";
                $(provinceTr).find("input[name='provinceCB']:checked").each(function () {
                    provinceIds += $(this).val() + ",";
                })
                if (provinceIds.length > 0)
                    provinceIds = provinceIds.substring(0, provinceIds.length - 1);
                jsonstr += '{"RegionName":"' + regionName + '","ProvinceIds":"' + provinceIds + '"},';
            }
            

        })
    }
    if (jsonstr.length > 0) {
        jsonstr = jsonstr.substring(0, jsonstr.length - 1);
    }
   
    return jsonstr;
}