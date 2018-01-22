
$(function () {

    //全选
    $("#cbALL").on("click", function () {
        var check = this.checked;
        $("input[name='projectCB']").each(function () {
            this.checked = check;
        })
    })


    $("#btnSearchSubject").on("click", function () {
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
        GetProjects();
        Plan.getList();
    })

    
    //开始核单
    $("#btnCheckOrder").on("click", function () {
        var customerId = $("#ddlCustomer").val() || "0";
        var projectId = "";

        $("#projectsDiv").find("input[name='projectCB']:checked").each(function () {
            projectId += $(this).val() + ",";
        })
        if (projectId == "") {
            alert("请选择项目");
            return false;
        }
        var planIds = GetSelect();

        if (planIds.length == 0) {
            alert("请选择方案");
            return false;
        }
        else {
            $("#showButtons").hide();
            $("#showLoading").show();
            planIds = planIds.substring(0, planIds.length - 1);
            var begin = $.trim($("#txtBeginDate").val());
            var end = $.trim($("#txtEndDate").val());
            $.ajax({
                type: "get",
                url: "../Handler/CheckOrder.ashx?type=checkOrder&customerId=" + customerId + "&subjectIds=" + projectId + "&planIds=" + planIds + "&begin=" + begin + "&end=" + end,
                cache: false,
                success: function (data) {
                    $("#showLoading").hide();
                    $("#showButtons").show();
                    if (data != "-1") {
                        //alert("拆单成功！");
                        window.location = "CheckOrderSuccess.aspx?checkOrderId=" + data;
                    }
                    else {
                        alert("操作失败");
                    }
                }
            })
        }
    })

})
//获取选中的方案
function GetSelect() {
    var ids = "";
//    $("input[name='cbox']:checked").each(function () {
//        ids += $(this).val() + ",";
//    })
//    return ids;

    var rows = $("#tbPlanList").datagrid("getSelections");
    for (var i = 0; i < rows.length; i++) {
        ids += rows[i].Id + ",";
    }
    return ids;
}

function GetProjects() {
    var customerId = $("#ddlCustomer").val() || "0";
    var begin = $.trim($("#txtBeginDate").val());
    var end = $.trim($("#txtEndDate").val());
//    if (customerId == "0") {
//        alert("请选择客户");
//        return false;
//    }
//    if (begin == "" || end == "") {
//        alert("请选择开始时间和结束时间");
//        return false;
//    }
    if (begin != "" && end != "") {
        var loading = "<img src='../../image/WaitImg/loading1.gif' />";
        $("#projectsDiv").html(loading);
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
                        var div = "<div style='float:left;'><input type='checkbox' name='projectCB' value='" + json[i].Id + "' /><span>" + json[i].SubjectName + "</span>&nbsp;</div>";
                        $("#projectsDiv").append(div);
                    }
                }

            }

        })
    }
    
}


//方案对象
var Plan = {
    getList: function () {
        var customerId1 = $("#ddlCustomer").val() || "0";
        var beginDate = $("#txtBeginDate").val();
        var endDate = $("#txtEndDate").val();
        $("#tbPlanList").datagrid({
            queryParams: { type: 'getList', customerId: customerId1,beginDate:beginDate,endDate:endDate, t: Math.random() * 1000 },
            method: 'get',
            url: '../Handler/CheckOrder.ashx',
            cache: false,
            columns: [[
            //{
            //field: 'checked', formatter: function (value, row, index) {
            // return '<input type="checkbox"  value="' + row.Id + '" name="cbox">';
            // }
            // },
                            {field: 'Id', hidden: true },
                            { field: 'checked', checkbox: true },
                            { field: 'ProjectName', title: '项目/活动', width: 200 },
                            { field: 'CustomerName', title: '客户', width: 200 },
                            { field: 'AddDate', title: '方案添加时间', width: 200 },
                            { field: 'BeginDate', title: '开始时间', width: 200 },
                            { field: 'EndDate', title: '结束时间', width: 200 },


                ]],
            singleSelect: false,
            toolbar: '#toolbar',
            striped: true,
            border: false,
            pagination: false,
            fitColumns: true,
            //height: 450,
            height: 'auto',
            iconCls: 'icon-save',
            emptyMsg: '没有相关记录',
            onLoadSuccess: function (data) {
                $(this).datagrid('clearSelections');
            },
            onClickRow: function (rowIndex, rowData) {
                $(this).datagrid("unselectRow", rowIndex);

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
        cache: false,
        queryParams: { planId: id, type: "getDetail", t: Math.random() * 1000 },
        fitColumns: false,
        height: 'auto',
        pagination: false,
        fit: false,
        columns: [[
                    { field: 'Id', hidden: true },
                    { field: 'RegionNames', title: '区域' },
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
                    { field: 'IsInstall', title: '是否安装' },
                    { field: 'CityTier', title: '城市级别' },
                    { field: 'Format', title: '店铺类型' },
                    { field: 'MaterialSupport', title: '物料支持'},
                    { field: 'POSScale', title: '店铺规模大小'},
                    { field: 'PositionId', title: 'POP位置' },
                    { field: 'Gender', title: '性别' },
                    { field: 'ChooseImg', title: '选图' }
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


function add() {
    var url = "AddCheckOrderPlan.aspx";
    $.fancybox.open({
        href: url,
        type: 'iframe',
        padding: 5,
        width: "98%"
    });
}

function refresh() {
    Plan.getList();
}

function deletePlan() {
    var planIds = GetSelect();
    if (planIds.length == 0) {
        alert("请选择方案");
        return false;
    }
    $.ajax({
        type: "get",
        url: "../Handler/CheckOrder.ashx?type=deletePlans&planIds=" + planIds,
        cache: false,
        success: function (data) {
            if (data == "ok") {
                Plan.getList();
                
            }
            else {
                alert("删除失败：" + data);
            }
        }
    })
}