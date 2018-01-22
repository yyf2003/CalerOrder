
$(function () {
    Plan.getList();
})


var Plan = {
    getList: function () {
        
        $("#tbPlanList").datagrid({
            queryParams: { type: 'getList', CustomerId: customerId, SubjectId: subjectId, planType: 1, t: Math.random() * 1000 },
            method: 'get',
            url: '../Handler/SplitOrder.ashx',
            cache: false,
            columns: [[
                            { field: 'Id', hidden: true },
                            {
                                field: 'checked', formatter: function (value, row, index) {
                                    return '<input type="checkbox"  value="' + row.Id + '" name="cbox">';
                                }
                            },
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
                            { field: 'CityTier', title: '城市级别' },
                            { field: 'IsInstall', title: '是否安装' },
                            { field: 'Format', title: '店铺类型' },
                            { field: 'ShopNos', title: '店铺编号' },
                            { field: 'MaterialSupport', title: '物料支持类别' },
                            { field: 'POSScale', title: '店铺规模' },

                            { field: 'PositionName', title: 'POP位置' },
                            { field: 'MachineFrameNames', title: '器架类型' },
                            { field: 'Gender', title: '性别' },
                            { field: 'Quantity', title: '数量' },
                            { field: 'GraphicNo', title: 'POP编号' },
                            { field: 'GraphicMaterial', title: 'POP材质' },
                            { field: 'GraphicWidth', title: 'POP宽' },
                            { field: 'GraphicLength', title: 'POP高' },
                            { field: 'WindowWidth', title: '位置宽' },
                            { field: 'WindowHigh', title: '位置高' },
                            { field: 'WindowDeep', title: '位置深' },
                ]],
            singleSelect: true,
            toolbar: '#toolbar',
            striped: true,
            border: false,
            pagination: false,
            fitColumns: false,
            height: 'auto',
            fit: false,
            iconCls: 'icon-save',
            emptyMsg: '没有相关记录',
            onLoadSuccess: function (data) {

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
        url: '../Handler/SplitOrder.ashx',
        cache: false,
        queryParams: { planId: id, type: "getDetail", t: Math.random() * 1000 },
        fitColumns: false,
        height: 'auto',
        pagination: false,
        //singleSelect: true,

        columns: [[
                    { field: 'Id', hidden: true },
                    { field: 'OrderType', title: '类型' },
                    { field: 'GraphicWidth', title: '宽' },
                    { field: 'GraphicLength', title: '高' },
                    { field: 'GraphicMaterial', title: '材质' },
                    { field: 'Supplier', title: '供货方' },
                    { field: 'RackSalePrice', title: '道具销售价' },
                    { field: 'Quantity', title: '数量' },
                    { field: 'Remark', title: '备注' }

            ]],
        onResize: function () {
            $(target).datagrid('fixDetailRowHeight', index);
        },
        onLoadSuccess: function (data) {
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


function checkFile() {
//    var planIds = GetSelect();
//    if (planIds.length == 0) {
//        alert("请选择方案");
//        return false;
//    }
//    
//    $("#hfPlanId").val(planIds);
    var val = $("#FileUpload1").val();
    if (val != "") {
        var extent = val.substring(val.lastIndexOf('.') + 1);
        if (extent != "xls" && extent != "xlsx") {
            alert("只能上传Excel文件");
            return false;
        }
    }
    else {
        alert("请选择文件");
        return false;
    }

    $("#showButton").css({ display: "none" });
    $("#showWaiting").css({ display: "" });

}

function GetSelect() {
    var ids = "";
    $("input[name='cbox']:checked").each(function () {
        ids += $(this).val() + ",";
    })
    if (ids.length > 0)
        ids = ids.substring(0, ids.length - 1);
    return ids;
}