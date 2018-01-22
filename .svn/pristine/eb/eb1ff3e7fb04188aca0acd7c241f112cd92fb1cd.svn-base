
var cityJson = {};
var loadProjectId = "";
$(function () {

    //导入
    $("#btnImport").on("click", function () {
        var customerId = $("#ddlCustomer").val() || 0;
        if (customerId == 0) {
            alert("请选择客户");
            return false;
        }
        var url = "ImportCheckPlan.aspx?customerId=" + customerId;
        $.fancybox.open({
            href: url,
            type: 'iframe',
            padding: 5,
            width: "98%"
        });
    })


    GetContents();
    //GetPosition();
    Plan.getList();
    $("#btnSearchSubject").on("click", function () {
        GetProjects();
    })

    $("#projectsDiv").delegate("input[name='projectCB']", "change", function () {
        ChangeSubject();
    })

    $("#RegionDiv").delegate("input[name='regionCB']", "change", function () {

        GetProvince();
        $("#CityDiv").html("");
        $("#cityAllDiv").hide();
        $("#cityCBAll").attr("checked", false);
        selectedCity = "";
        selectedCityNames = "";
    })

    //选择省份
    $("#ProvinceDiv").delegate("input[name='provinceCB']", "change", function () {
        GetCity();
        var cbcount = $("#ProvinceDiv input[name='provinceCB']").length;
        var checkedcount = $("#ProvinceDiv input[name='provinceCB']:checked").length;
        if (cbcount == checkedcount) {
            $("#provinceCBAll").attr("checked", "checked");
        }
        else
            $("#provinceCBAll").attr("checked", false);
    })

    //省份全选
    $("#provinceCBAll").on("click", function () {
        var checked = this.checked;
        $("#ProvinceDiv input[name='provinceCB']").each(function () {
            $(this).attr("checked", checked);
        })
        GetCity();
    })
    //城市全选
    $("#cityCBAll").on("click", function () {
        var checked = this.checked;
        $("#CityDiv input[name='cityCB']").each(function () {
            $(this).attr("checked", checked);
        })

    })

    //更多城市全选
    $("#moreCityCBAll").on("click", function () {
        var checked = this.checked;
        $("#showCityListDiv input[name='moreCityCB']").each(function () {
            $(this).attr("checked", checked);
        })
        ChangeMoreCitySelect();
    })


    //查找城市
    $("#btnSearchCity").on("click", function () {
        $("#showCityListDiv span").each(function () {
            $(this).css("color", "#010204");
        })
        var key = $("#txtSearchCity").val();
        //$(this).css("color", "#010204");
        if (key != "") {
            var arr = key.split(',');
            for (var i = 0; i < arr.length; i++) {
                $("#showCityListDiv span").each(function () {

                    if ($(this).html().indexOf(arr[i]) != -1) {
                        $(this).css("color", "#f00");
                    }

                })
            }

        }
    })

    //选择城市
    $("#CityDiv").delegate("input[name='cityCB']", "change", function () {

        var cbcount = $("#CityDiv input[name='cityCB']").length;
        var checkedcount = $("#CityDiv input[name='cityCB']:checked").length;
        if (cbcount == checkedcount) {
            $("#cityCBAll").attr("checked", "checked");
        }
        else
            $("#cityCBAll").attr("checked", false);


    })
    //选择城市（更多城市）
    $("#showCityListDiv").delegate("input[name='moreCityCB']", "change", function () {
        ChangeMoreCitySelect();
    })

    //添加方案内容AddPlanContent
    $("#btnAddContent").on("click", function () {
        AddPlanContent();
        CheckPlanContent();
    })

    //提交方案
    $("#btnSubmitPlan").on("click", function () {
        Plan.submit("add");

    })

    //更新方案
    $("#btnUpdatePlan").on("click", function () {
        Plan.update();

    })

    //刷新方案
    $("#btnRefresh").on("click", function () {
        Plan.getList();
    })


    //删除方案
    $("#btnDelete").on("click", function () {
        var selectRow = $("#tbPlanList").datagrid("getSelected");
        if (selectRow != null) {
            var pid = selectRow.Id;
            $.ajax({
                type: "get",
                url: "../Handler/CheckOrder.ashx?type=deletePlan&planId=" + pid,
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        Plan.getList();
                        //ClearConditionVal();
                        CurrPlanId = 0;
                        LoadPlanDetail();
                    }
                    else {
                        alert("删除失败：" + data);
                    }
                }
            })
        }

    })

    //删除方案内容
    $("#contentBody").delegate(".deletePlanDetail", "click", function () {
        $(this).parent().parent().remove();
    })

    //清空方案内容
    $("#spanClareContent").on("click", function () {
        $("#contentBody").html("");
    })
})

function ChangeMoreCitySelect() {
    var cbcount = $("#showCityListDiv input[name='moreCityCB']").each(function () {
        var cid = $(this).val();
        var checked = this.checked;
        $("#CityDiv input[name='cityCB']").each(function () {
            if ($(this).val() == cid) {
                this.checked = checked;
            }
        })
    }).length;
    var checkedcount = $("#showCityListDiv input[name='moreCityCB']:checked").length;
    if (cbcount == checkedcount) {

        $("#cityCBAll,#moreCityCBAll").attr("checked", "checked");

    }
    else {
        $("#cityCBAll,#moreCityCBAll").attr("checked", false);
    }



}


function GetProjects() {
    var customerId = $("#ddlCustomer").val() || "0";
    var begin = $.trim($("#txtBeginDate").val());
    var end = $.trim($("#txtEndDate").val());
    if (customerId == "0") {
        alert("请选择客户");
        return false;
    }
    if (begin == "" || end == "") {
        alert("请选择开始时间和结束时间");
        return false;
    }
    if (begin != "" && end != "") {
        var loading = "<img src='../../image/WaitImg/loading1.gif' />";
        $("#projectsDiv").html(loading);
        var arr = [];
        if (loadProjectId.length > 0) {
            arr = loadProjectId.split(',');
        }
        
        $.ajax({
            type: "get",
            url: "../Handler/CheckOrder.ashx",
            data: { type: "getProjects", customerId: customerId, beginDate: begin, endDate: end },
            cache:false,
            success: function (data) {
                $("#projectsDiv").html("");
                
                if (data != "") {

                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var check = "";
                        if (arr.length > 0) {
                            for (var j = 0; j < arr.length; j++) {
                                if (parseInt(arr[j]) == parseInt(json[i].Id)) {
                                    check = "checked='checked'";
                                }
                            }
                        }
                        var div = "<div style='float:left;'><input type='checkbox' name='projectCB' value='" + json[i].Id + "' " + check + "/><span>" + json[i].SubjectName + "</span>&nbsp;</div>";
                        $("#projectsDiv").append(div);
                    }
                }
                if (arr.length > 0) {
                    ChangeSubject();
                }
            }

        })
    }

}
var projectIds = "";
function ChangeSubject() {
    projectIds = "";
    $("#projectsDiv").find("input[name='projectCB']:checked").each(function () {
        projectIds += $(this).val() + ",";

    })
    if (projectIds.length > 0) {
        projectIds = projectIds.substring(0, projectIds.length - 1);
    }
   
    GetRegoin();
    GetContents();
}

function GetRegoin() {
    var customerId = $("#ddlCustomer").val() || "0";
    
    
    $.ajax({
        type: "get",
        url: "../Handler/Orders.ashx",
        //data: { type: "getRegion", customerId: customerId },
        data: { type: "getRegion", subjectIds: projectIds },
        success: function (data) {
            $("#RegionDiv").html("");
            if (data != "") {

                var json = eval(data);
                for (var i = 0; i < json.length; i++) {
                    var div = "<input type='checkbox' name='regionCB' value='" + json[i].Region + "' /><span>" + json[i].Region + "</span>&nbsp;";
                    $("#RegionDiv").append(div);
                }
            }
        }

    })
}

function GetProvince() {

    var regions = "";
    $("#provinceCBAll").attr("checked", false);
    $("#RegionDiv input[name='regionCB']:checked").each(function () {
        regions += $(this).val() + ",";
    })

    if (regions != "") {
        regions = regions.substring(0, regions.length - 1);
    }
    $.ajax({
        type: "get",
        url: "../Handler/Orders.ashx",
        data: { type: "getProvince", region: regions, subjectIds: projectIds },
        success: function (data) {
            $("#ProvinceDiv").html("");
            if (data != "") {
                $("#provinceAllDiv").show();
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='width:100px;float:left;'>";
                    div += "<input type='checkbox' name='provinceCB' value='" + json[i].Province + "' /><span>" + json[i].Province + "</span>&nbsp;";
                    div += "</div>";

                }
                $("#ProvinceDiv").html(div);
            }
            else {
                $("#provinceAllDiv").hide();

            }
        }

    })
}

function GetCity() {

    var provinces = "";
    $("#cityCBAll").attr("checked", false);
    selectedCity = "";
    selectedCityNames = "";
    $("#ProvinceDiv input[name='provinceCB']:checked").each(function () {
        provinces += $(this).val() + ",";
    })
    if (provinces != "") {
        provinces = provinces.substring(0, provinces.length - 1);
    }
    $.ajax({
        type: "get",
        url: "../Handler/Orders.ashx",
        data: { type: "getCity", province: provinces, subjectIds: projectIds },
        success: function (data) {
            $("#CityDiv").html("");
            if (data != "") {
                $("#cityAllDiv").show();
                var json = eval(data);
                var div = "";
                var count = json.length;
                count = count > 50 ? 50 : count;
                for (var i = 0; i < count; i++) {
                    div += "<div style='float:left;'>";
                    div += "<input type='checkbox' name='cityCB' value='" + json[i].City + "' /><span>" + json[i].City + "</span>&nbsp;";
                    div += "</div>";
                }
                if (json.length > 50) {
                    cityJson = json;
                    div += "<div style='float:left;'>";
                    div += "<span id='showMoreCitySpan' onclick='ShowMoreCity()' style='text-decoration:underline;cursor:pointer;color:blue;'>更多...</span>";
                    div += "</div>";
                }
                $("#CityDiv").html(div);
            }
            else {
                $("#cityAllDiv").hide();
                $("#cityCBAll").attr("checked", false);
            }
        }

    })
}


function ShowMoreCity() {

    if (cityJson != null && cityJson.length > 0) {
        var div = "";
        var checked = "";
        var isCheckAll = 0;
        var arr = [];
        if ($("#cityCBAll").attr("checked") == "checked") {
            checked = "checked='checked'";
            isCheckAll = 1;
            $("#moreCityCBAll").attr("checked", "checked");
        }
        else {
            if (selectedCity.length > 0) {
                selectedCity = selectedCity.substring(0, selectedCity.length - 1);
                arr = selectedCity.split(',');
            }
        }
        for (var i = 0; i < cityJson.length; i++) {
            var check1 = checked;
            if (isCheckAll == 0) {
                if (arr.length > 0) {
                    for (var j = 0; j < arr.length; j++) {
                        if (parseInt(cityJson[i].CityId) == parseInt(arr[j])) {
                            check1 = "checked='checked'";
                        }
                    }
                }
                else {
                    $("#CityDiv input[name='cityCB']:checked").each(function () {
                        if (parseInt(cityJson[i].CityId) == parseInt($(this).val())) {
                            check1 = "checked='checked'";
                        }
                    })
                }
            }

            div += "<div style='float:left; line-height:25px;'>";
            div += "<input type='checkbox' name='moreCityCB' value='" + cityJson[i].CityId + "' " + check1 + "/><span>" + cityJson[i].CityName + "</span>&nbsp;";
            div += "</div>";
        }
        $("#showCityListDiv").html(div);
    }
    else {
        $("#showCityListDiv").html("");
    }
    $("#showCityDiv").show().dialog({
        modal: true,
        width: 650,
        height: 400,
        iconCls: 'icon-search',
        resizable: false,
        buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            $("#showCityListDiv").find("input[name='moreCityCB']:checked").each(function () {
                                selectedCity += $(this).val() + ",";
                                selectedCityNames += $(this).next().html() + ",";
                                $("#showCityDiv").dialog('close');
                            })

                        }
                    },
                    {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#showCityDiv").dialog('close');
                        }
                    }
                ]
    });
}
//获取内容的信息
function GetContents() {
    //var customerId1 = $("#ddlCustomer").val() || "0";

    $.getJSON("../Handler/CheckOrder.ashx", { type: "getContents", subjectIds: projectIds }, function (data) {

        if (data.length > 0) {
            $("#FormatDiv").html(InitContents("FormatDiv", data[0].Format));
            $("#MaterialSupportDiv").html(InitContents("MaterialSupportDiv", data[0].MaterialSupport));
            $("#InstallDiv").html(InitContents("InstallDiv", data[0].IsInstall));
            $("#ScaleDiv").html(InitContents("ScaleDiv", data[0].POSScale));
            $("#CityTierDiv").html(InitContents("CityTierDiv", data[0].CityTier));
            $("#GenderDiv").html(InitContents("GenderDiv", data[0].Gender));
            $("#SheetDiv").html(InitContents("SheetDiv", data[0].Sheet));
            $("#ChooseImgDiv").html(InitContents("ChooseImgDiv", data[0].ChooseImg));
        }
    })
}

//初始化内容值
function InitContents(element, valStr) {
    var input = "";
    if ($.trim(valStr) != "") {
        var arr = valStr.split(',');
        if (arr.length > 0) {

            var input = "";
            var cbName = element + "cb";
            for (var i = 0; i < arr.length; i++) {
                input += "<input type='checkbox' name='" + cbName + "' value='" + arr[i] + "'/>" + arr[i] + "&nbsp;";
            }

        }
    }
    return input;
}

//获取位置
function GetPosition() {

    var customerId = $("#ddlCustomer").val() || "0";
    $.ajax({
        type: "get",
        url: "../Handler/CheckOrder.ashx?type=getPosition&customerId=" + customerId,
        cache: false,
        success: function (data) {
            if (data != "") {
                var json = eval(data);
                if (json.length > 0) {
                    var input = "";
                    for (var i = 0; i < json.length; i++) {
                        input += "<input type='checkbox' name='PositionDivcb' value='" + json[i].Id + "'/><span>" + json[i].PositionName + "</span>&nbsp;";
                    }
                    $("#PositionDiv").html(input);
                }

            }
        }
    })
}

//添加内容
var selectedCity = "";
var selectedCityNames = "";
function AddPlanContent() {
    var flag = false;

    var region = "";
    //var regionNames = "";
    $("#RegionDiv").find("input[name='regionCB']:checked").each(function () {
        region += $(this).val() + ",";
        //regionNames += $(this).next().html() + ",";
        flag = true;
    })
    var province = "";
    $("#ProvinceDiv").find("input[name='provinceCB']:checked").each(function () {
        province += $(this).val() + ",";
       
    })
    var city = selectedCity;
    
    if (city == "") {
        $("#CityDiv").find("input[name='cityCB']:checked").each(function () {
            city += $(this).val() + ",";

        })
    }
    var Format = "";
    $("#FormatDiv").find("input[name='FormatDivcb']:checked").each(function () {
        Format += $(this).val() + ",";
        flag = true;
    })
    var MaterialSupport = "";
    $("#MaterialSupportDiv").find("input[name='MaterialSupportDivcb']:checked").each(function () {
        MaterialSupport += $(this).val() + ",";
        flag = true;
    })
    var Install = "";
    $("#InstallDiv").find("input[name='InstallDivcb']:checked").each(function () {
        Install += $(this).val() + ",";
        flag = true;
    })
    var Scale = "";
    $("#ScaleDiv").find("input[name='ScaleDivcb']:checked").each(function () {
        Scale += $(this).val() + ",";
        flag = true;
    })

    var Position = "";
    //var PositionName = "";
    $("#SheetDiv").find("input[name='SheetDivcb']:checked").each(function () {
        Position += $(this).val() + ",";
        //PositionName += $(this).next().html() + ",";
        flag = true;
    })
    var Gender = "";
    $("#GenderDiv").find("input[name='GenderDivcb']:checked").each(function () {
        Gender += $(this).val() + ",";
        flag = true;
    })
    var CityTier = "";
    $("#CityTierDiv").find("input[name='CityTierDivcb']:checked").each(function () {
        CityTier += $(this).val() + ",";
        flag = true;
    })
    var ChooseImg = "";
    $("#ChooseImgDiv").find("input[name='ChooseImgDivcb']:checked").each(function () {
        ChooseImg += $(this).val() + ",";
        flag = true;
    })
    if (region.length > 0)
        region = region.substring(0, region.length - 1);
    
    if (province.length > 0)
        province = province.substring(0, province.length - 1);
    if (city.length > 0)
        city = city.substring(0, city.length - 1);
    
    if (Format.length > 0)
        Format = Format.substring(0, Format.length - 1);
    if (MaterialSupport.length > 0)
        MaterialSupport = MaterialSupport.substring(0, MaterialSupport.length - 1);
    if (Install.length > 0)
        Install = Install.substring(0, Install.length - 1);
    if (Scale.length > 0)
        Scale = Scale.substring(0, Scale.length - 1);
    if (Position.length > 0)
        Position = Position.substring(0, Position.length - 1);
    
    if (Gender.length > 0)
        Gender = Gender.substring(0, Gender.length - 1);
    if (CityTier.length > 0)
        CityTier = CityTier.substring(0, CityTier.length - 1);
    if (ChooseImg.length > 0)
        ChooseImg = ChooseImg.substring(0, ChooseImg.length - 1);
    if (flag) {
        var pName = province;
        if (pName.length > 50) {
            pName = pName.substring(0, 50) + "...<span style='color:blue;cursor:pointer;'>更多</span>";
        }
        var cName = city;
        if (cName.length > 50) {
            cName = cName.substring(0, 50) + "...<span style='color:blue;cursor:pointer;'>更多</span>";
        }
        var tr1 = "<tr class='tr_bai'>";
        tr1 += "<td>" + region + "</td>";
        tr1 += "<td><span data-province='" + province + "'>" + pName + "</span></td>";
        tr1 += "<td><span data-city='" + city + "'>" + cName + "</span></td>";
        tr1 += "<td>" + CityTier + "</td>";
        tr1 += "<td>" + Install + "</td>";
        tr1 += "<td>" + Format + "</td>";
        tr1 += "<td>" + MaterialSupport + "</td>";
        tr1 += "<td>" + Scale + "</td>";
        tr1 += "<td>" + Position + "</td>";
        tr1 += "<td>" + Gender + "</td>";
        tr1 += "<td>" + ChooseImg + "</td>";
        tr1 += "<td><span class='deletePlanDetail' style='color:red;cursor:pointer;'>删除</span></td>";
        tr1 += "</tr>";
        $("#contentBody").append(tr1);
    }
    
}

//检查是否有方案内容信息
function CheckPlanContent() {
    var addcontent = $("#contentBody tr");
    if (addcontent.length > 0) {
        $("#planContents").show();
    }
    else {
        $("#planContents").hide();
    }
}

function SubmitPlan() {
    var customerId = $("#ddlCustomer").val() || "0";
    var begin = $.trim($("#txtBeginDate").val());
    var end = $.trim($("#txtEndDate").val());

    if (customerId == "0") {
        alert("请选择客户");
        return false;
    }
    if (begin == "" || end == "") {
        alert("请选择开始时间和结束时间");
        return false;
    }

    var projectId = "";
    var projectName = "";

    $("#projectsDiv").find("input[name='projectCB']:checked").each(function () {
        projectId += $(this).val() + ",";
        projectName += $(this).next().html() + ",";
    })
    if (projectId == "") {
        alert("请选择项目");
        return false;
    }
    if (projectId.length > 0) {
        projectId = projectId.substring(0, projectId.length - 1);
        projectName = projectName.substring(0, projectName.length - 1);
    }


    var addcontent = $("#contentBody tr");
//    if (addcontent.length == 0) {
//        alert("请先添加方案内容");
//        return false;
//    }
    var planDetailJson = "";
    for (var i = 0; i < addcontent.length; i++) {
        var td = $(addcontent[i]).find("td");
        var regions = $(td).eq(0).html();
        var provinces = $(td).eq(1).find("span").data("province");
        var citys = $(td).eq(2).find("span").data("city");
        
        var CityTier = $(td).eq(3).html();
        var Install = $(td).eq(4).html();
        var Format = $(td).eq(5).html();
        var MaterialSupport = $(td).eq(6).html();
        var Scale = $(td).eq(7).html();
        var positions = $(td).eq(8).html();
        
        var Gender = $(td).eq(9).html();
        var ChooseImg = $(td).eq(10).html();
        planDetailJson += '{"RegionNames":"' + regions + '","ProvinceId":"' + provinces + '","CityId":"' + citys + '","CityTier":"' + CityTier + '","IsInstall":"' + Install + '","PositionId":"' + positions + '","Format":"' + Format + '","MaterialSupport":"' + MaterialSupport + '","POSScale":"' + Scale + '","Gender":"' + Gender + '","ChooseImg":"' + ChooseImg + '"},';
    }
    if (planDetailJson.length > 0) {
        planDetailJson = planDetailJson.substring(0, planDetailJson.length - 1);
        PlanJsonStr = '{"Id":' + CurrPlanId + ',"CustomerId":' + customerId + ',"ProjectId":"' + projectId + '","ProjectName":"' + projectName + '","BeginDate":"' + begin + '","EndDate":"' + end + '"';
        PlanJsonStr += ',"CheckOrderPlanDetail":[' + planDetailJson + ']}';
    }
    
    return true;
}

var CurrPlanId = 0;
var PlanJsonStr = "";
//方案对象
var Plan = {
    getList: function () {
        var customerId1 = $("#ddlCustomer").val() || "0";
        $("#tbPlanList").datagrid({
            queryParams: { type: 'getList', customerId: customerId1, t: Math.random() * 1000 },
            method: 'get',
            url: '../Handler/CheckOrder.ashx',
            //cache: false,
            columns: [[
                            { field: 'Id', hidden: true },
                            { field: 'CustomerId', hidden: true },
                            { field: 'ProjectId', hidden: true },
                            { field: 'ProjectName', title: '项目/活动', width: 200 },
                            { field: 'CustomerName', title: '客户', width: 200 },
                            { field: 'AddDate', title: '方案添加时间', width: 200 },
                            { field: 'BeginDate', title: '开始时间', width: 200 },
                            { field: 'EndDate', title: '结束时间', width: 200 },


                ]],
            singleSelect: true,
            toolbar: '#toolbar',
            striped: true,
            border: false,
            pagination: false,
            fitColumns: true,
            collapsible: true,
            //height: 450,
            height: 'auto',
            iconCls: 'icon-save',
            emptyMsg: '没有相关记录',
            onLoadSuccess: function (data) {
                $(this).datagrid('clearSelections');
            },
            onSelect: function (rowIndex, rowData) {
                LoadCondition(rowData);

            },
            view: detailview,
            detailFormatter: function (index, row) {
                return '<div style="padding:2px;"> <table id="ddv_' + index + '"></table></div>';
            },
            onExpandRow: function (index, row) {
                commonExpandRow(index, row, this, 'ddv');
            }

        });
    },
    submit: function (optype) {
        if (SubmitPlan()) {
            $.ajax({
                type: "post",
                url: "../Handler/CheckOrder.ashx?type=add&optype=" + optype,
                data: { jsonStr: escape(PlanJsonStr) },
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        $("#contentBody").html("");
                        Plan.getList();
                        //ClearConditionVal();
                        //ClearVal();
                        //alert("添加成功");
                    }
                    else {
                        alert("操作失败：" + data);
                    }
                }
            })
        }
    },
    update: function () {
        this.submit("update");
    },
    refresh: function () {
        $("#tbPlanList").datagrid("reload");
    }

}


function commonExpandRow(index, row, target, nextName) {

    var curName = nextName + '_' + index;
    var id = row.Id;
    $('#' + curName).datagrid({
        method: 'get',
        url: '../Handler/CheckOrder.ashx',

        queryParams: { planId: id, type: "getDetail", t: Math.random() * 1000 },
        fit: false,
        collapsible:true,
        fitColumns: false,
        height: 'auto',
        
        pagination: false,
        //cache: false,
        columns: [[
                    { field: 'Id', hidden: true },
                    
                    
                    //{ field: 'PositionId', hidden: true },
                    { field: 'RegionNames', title: '区域'},
                    {
                        field: 'ProvinceName1', title: '省份', formatter: function (value, row, index) {
                            var provinceNames = row.ProvinceName;
                            if (provinceNames.length > 50) {
                                provinceNames = provinceNames.substring(0, 50) + "...<span style='color:blue;cursor:pointer;'>更多</span>";
                            }
                            return '<span>' + provinceNames + '</span>';
                        }
                    },
                    {
                        field: 'CityName1', title: '城市', formatter: function (value, row, index) {
                            var cityNames = row.CityName;
                            if (cityNames.length > 50) {
                                cityNames = cityNames.substring(0, 50) + "...<span style='color:blue;cursor:pointer;'>更多</span>";
                            }
                            return '<span>' + cityNames + '</span>';
                        }
                    },
                    { field: 'CityTier', title: '城市级别' },
                    { field: 'IsInstall', title: '是否安装' },
                    { field: 'Format', title: '店铺类型'},
                    { field: 'MaterialSupport', title: '物料支持' },
                    { field: 'POSScale', title: '店铺规模大小' },
                    { field: 'PositionId', title: 'POP位置' },
                    { field: 'Gender', title: '性别' },
                    { field: 'ChooseImg', title: '选图' }

            ]],
        frozenColumns: [[
                     {
                         field: 'edit', title: '删除', align: 'center',
                         formatter: function (value, rec) {
                             var btn = '<a href="javascript:void(0)" onclick="deleteDetail(' + rec.Id + ')" style="color:red;">删除</a>';
                             return btn;
                         }
                     }
                     ]],

        onResize: function () {
            $(target).datagrid('fixDetailRowHeight', index);
        },
        onLoadSuccess: function (data) {
            $(target).datagrid('clearSelections');
            setTimeout(function () {
                $(target).datagrid('fixDetailRowHeight', index);
            }, 0);

        },
        onClickRow: function (rowIndex, rowData) {
            $(this).datagrid("unselectRow", rowIndex);

        }

    });
    $(target).datagrid('fixDetailRowHeight', index);

}

//删除方案列表中的方案内容
function deleteDetail(detailId) {
    $.ajax({
        type: "get",
        url: "../Handler/CheckOrder.ashx?type=deleteDetail&detailId=" + detailId,
        cache: false,
        success: function (data) {
            if (data == "ok") {
                Plan.refresh();
            }
            else {
                alert("删除失败：" + data);
            }
        }
    })
}

function LoadCondition(row) {
    CurrPlanId = row.Id;
    
    var customerId = row.CustomerId;
    loadProjectId = row.ProjectId;
    
    var beginDate = row.BeginDate;
    var endDate = row.EndDate;
    if ($.trim(customerId) != "" && customerId > 0) {
        $("#ddlCustomer").val(customerId);
    }
    if ($.trim(beginDate) != "") {
        $("#txtBeginDate").val(beginDate);
    }
    if ($.trim(endDate) != "") {
        $("#txtEndDate").val(endDate);
    }
    $("#btnSearchSubject").click();

    LoadPlanDetail();
    
}

function LoadPlanDetail() {
    $.ajax({
        type: 'get',
        url: '../Handler/CheckOrder.ashx',
        data: { planId: CurrPlanId, type: "getDetail", t: Math.random() * 1000 },
        success: function (data) {
            $("#contentBody").html("");
            if (data != "") {
                var json = eval(data);
                if (json.length > 0) {
                    for (var i = 0; i < json.length; i++) {
                        var pName = json[i].ProvinceName;
                        if (pName.length > 50) {
                            pName = pName.substring(0, 50) + "...<span style='color:blue;cursor:pointer;'>更多</span>";
                        }
                        var cName = json[i].CityName;
                        if (cName.length > 50) {
                            cName = cName.substring(0, 50) + "...<span style='color:blue;cursor:pointer;'>更多</span>";
                        }
                        var tr1 = "<tr class='tr_bai'>";
                        tr1 += "<td>" + json[i].RegionNames + "</td>";
                        tr1 += "<td><span data-province='" + json[i].ProvinceName + "'>" + pName + "</span></td>";
                        tr1 += "<td><span data-city='" + json[i].CityName + "'>" + cName + "</span></td>";
                        tr1 += "<td>" + json[i].CityTier + "</td>";
                        tr1 += "<td>" + json[i].IsInstall + "</td>";
                        tr1 += "<td>" + json[i].Format + "</td>";
                        tr1 += "<td>" + json[i].MaterialSupport + "</td>";
                        tr1 += "<td>" + json[i].POSScale + "</td>";
                        tr1 += "<td>" + json[i].PositionId + "</td>";
                        tr1 += "<td>" + json[i].Gender + "</td>";
                        tr1 += "<td>" + json[i].ChooseImg + "</td>";
                        tr1 += "<td><span class='deletePlanDetail' style='color:red;cursor:pointer;'>删除</span></td>";
                        tr1 += "</tr>";

                        $("#contentBody").append(tr1);
                    }
                }
                CheckPlanContent();
            }
        }
    })
}