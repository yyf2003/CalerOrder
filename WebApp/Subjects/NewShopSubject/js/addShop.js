
$(function () {
    Shop.getList();
    $("#btnAdd").click(function () {
        Shop.getProvince();
        layer.open({
            type: 1,
            time: 0,
            title: '添加店铺',
            skin: 'layui-layer-rim', //加上边框
            area: ['80%', '480px'],
            content: $("#editShopDiv"),
            id: 'editLayer',
            btn: ['确定提交'],
            btnAlign: 'c',
            yes: function () {
                alert("ok");
            },
            cancel: function (index) {

                layer.closeAll();
                $("#editShopDiv").hide();

            }

        });
    })
    $("#btnEdit").click(function () {
        alert("edit");
    })
    $("#btnDelete").click(function () {
        alert("btnDelete");
    })
})

var Shop = {
    model: function () {
        this.Id = 0;
        this.ShopName = "";
        this.ShopNo = "";
        this.CustomerId = 0;
        this.RegionName = "";
        this.ProvinceId = 0;
        this.ProvinceName = "";
        this.CityId = 0;
        this.CityName = "";
        this.AreaId = 0;
        this.AreaName = "";
        this.CityTier = "";
        this.IsInstall = "";
        this.AgentCode = "";
        this.AgentName = "";
        this.POPAddress = "";
        this.Contact1 = "";
        this.Tel1 = "";
        this.Contact2 = "";
        this.Tel2 = "";
        this.Channel = "";
        this.Format = "";
        this.LocationType = "";
        this.BusinessModel = "";
        this.Remark = "";
        this.CSUserId = 0;
        this.BasicInstallPrice = 0;
        this.SubjectId = 0;

    },
    getList: function () {
        $.ajax({
            type: "get",
            url: "handler/AddShop.ashx?type=getShopList?subjectId=" + subjectId,
            success: function (data) {
                if (data != "") {
                    var josn = eval(data);
                    if (josn.length > 0) {
                        var tr = "";
                        for (var i in json) {
                            tr += "<tr>";
                            tr += "<td><input type='checkbox' name='cbOne' value='" + json[i].ShopId + "'></td>";
                            tr += "<td>" + json[i].ShopNo + "</td>";
                            tr += "<td>" + json[i].ShopName + "</td>";
                            tr += "<td>" + json[i].RegionName + "</td>";
                            tr += "<td>" + json[i].ProvinceName + "</td>";
                            tr += "<td>" + json[i].CityName + "</td>";
                            tr += "<td>" + json[i].CityTier + "</td>";
                            tr += "<td>" + json[i].IsInstall + "</td>";
                            tr += "<td>" + json[i].Channel + "</td>";
                            tr += "<td>" + json[i].Format + "</td>";
                            tr += "<td>" + json[i].OpenDate + "</td>";
                            tr += "<td><button name='btnAddOrder' data-shopid='" + json[i].ShopId + "' class='layui-btn layui-btn-small layui-btn-primary'><i class='layui-icon'>&#x1002;</i></button></td>";
                            tr += "<td><button name='btnEditShop' data-shopid='" + json[i].ShopId + "' class='layui-btn layui-btn-small layui-btn-primary'><i class='layui-icon'>&#xe642;</i></button></td>";
                            tr += "<td><button name='btnDeleteShop' data-shopid='" + json[i].ShopId + "' class='layui-btn layui-btn-small layui-btn-primary'><i class='layui-icon'>&#xe642;</i></button></td>";
                            tr += "</tr>";
                        }
                        $("#tbodyData").html(tr).show();
                        $("#tbodyEmpty").hide();
                    }
                }
                else {
                    $("#tbodyData").hide();
                    $("#tbodyEmpty").show();
                }
            }
        })
    },
    getProvince: function () {
        document.getElementById("seleProvince").length = 1;
        $.ajax({
            type: "get",
            url: "/Customer/Handler/Shops.ashx?type=bindProvince&regionId=" + regionId,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    var isSelected = false;
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        //if (json[i].Id == currProvinceId) {
                            //selected = "selected='selected'";
                            //isSelected = true;
                        //}
                        var option = "<option value='" + json[i].Id + "' " + selected + ">" + json[i].ProvinceName + "</option>";
                        $("#seleProvince").append(option);
                    }
                    
                }
            }
        })
    },
    submitShop:function(){
    
    },
    checkValue: function () { 
       
    }
};