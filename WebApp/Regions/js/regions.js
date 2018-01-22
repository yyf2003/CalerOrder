


var JsonStr = "";
var CurrRegionId = 0;
var CurrProvinceIds = "";
var Region = {
    bindProvinceList: function () {

        if ($.trim(CurrProvinceIds) != "") {
            var pidArr = CurrProvinceIds.split(',');
            for (var i = 0; i < pidArr.length; i++) {
                $("#cblProvince input[type='checkbox']").each(function () {
                    if (parseInt($(this).val()) == parseInt(pidArr[i])) {
                        $(this).attr("checked", "checked");
                    }
                })
            }
        }

    },
    addRegion: function (optype) {
        $("#editDiv").show().dialog({
            modal: true,
            width: 520,
            height: 350,
            iconCls: 'icon-add',
            title: optype == "add" ? '添加区域' : '编辑区域',
            resizable: false,
            buttons: [

                    {
                        text: optype == "add" ? '添加' : '更新',
                        iconCls: optype == "add" ? 'icon-add' : 'icon-edit',
                        visible: false,
                        handler: function () {
                            if (CheckVal()) {
                                $.ajax({
                                    type: "get",
                                    url: "./Handler/Handler1.ashx?type=edit&optype=" + optype + "&jsonString=" + escape(JsonStr),
                                    cache: false,
                                    success: function (data) {
                                        if (data == "exist") {
                                            alert("该区域已存在！");

                                        }
                                        else if (data == "ok") {

                                            //window.location.href = "List.aspx";
                                            $("#btnSearch").click();
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
    }
}


$(function () {
    //添加
    $("#btnAdd").click(function () {
        ClearVal();
        $("#ddlCustomer").val($("#ddlCustomerSearch").val());
        Region.addRegion("add");
    })
})


function editRegion(obj) {
    ClearVal();
    var name = $(obj).parent().siblings("td").eq(2).html();
    var CustomerId = $(obj).siblings("input[name$='hfCustomerId']").val() || "0";
    CurrProvinceIds = $(obj).siblings("input[name$='hfProvinceIds']").val() || "";
    CurrRegionId = $(obj).data("regionid");
    $("#ddlCustomer").val(CustomerId);
    $("#txtRegionName").val(name);
    Region.bindProvinceList();
    Region.addRegion("update");
}

function CheckVal() {
    var customerId = $("#ddlCustomer").val();
    if (customerId == "0") {
        alert("请选择客户");
        return false;
    }
    var regionName = $("#txtRegionName").val();
    if ($.trim(regionName) == "") {
        alert("请填写区域名称");
        return false;
    }
    var pids = "";
    $("#cblProvince input:checked").each(function () {
        pids += $(this).val()+",";
    })
    
    if (pids.length > 0)
        pids = pids.substring(0, pids.length - 1);
    else {
        //alert("请选择省份");
        //return false;
    }
    JsonStr = '{"Id":' + CurrRegionId + ',"CustomerId":' + customerId + ',"RegionName":"' + regionName + '","ProvinceIds":"' + pids + '"}';
    
    return true;
}

function ClearVal() {
    JsonStr = "";
    CurrRegionId = 0;
    CurrProvinceIds = "";
    $("#txtRegionName").val("");
   
    $("#cblProvince input:checked").each(function () {
        $(this).attr("checked", false);
    })
}