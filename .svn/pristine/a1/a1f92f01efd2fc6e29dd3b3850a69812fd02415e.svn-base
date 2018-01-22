
var planIds = $("#ShowOrderDetail1_hfPlanIds").val() || $("#hfPlanIds").val() || "";
$(function () {

    Plan.getList();


})

var Plan = {
    getList: function () {

        $.ajax({
            type: "get",
            url: "/Subjects/Handler/SplitOrder1.ashx?type=getList&Ids=" + planIds + "&t=" + Math.random() * 1000,
            success: function (data) {

                $("#showLoading").hide();
                $("#tbPlanList1").datagrid({
                    data: eval(data),
                    columns: [[
                            { field: 'Id', hidden: true },
                            { field: 'ShopNos1', title: '店铺编号',
                                formatter: function (value, rec) {
                                    var val = rec.ShopNos;
                                    val = val.length > 15 ? (val.substring(0, 15) + "...") : val;

                                    return val;
                                }

                            },
                            { field: 'PositionName', title: 'POP位置', sortable: true },
                            { field: 'RegionNames', title: '区域' },
                            {
                                field: 'ProvinceName1', title: '省份', formatter: function (value, row, index) {
                                    var provinceNames = row.ProvinceName;

                                    if (provinceNames.length > 20) {
                                        provinceNames = provinceNames.substring(0, 20) + "...<span style='color:blue;cursor:pointer;'>更多</span>";
                                    }
                                    return '<span>' + provinceNames + '</span>';
                                }
                            },
                            {
                                field: 'CityName1', title: '城市', formatter: function (value, row, index) {
                                    var cityNames = row.CityName;
                                    if (cityNames.length > 20) {
                                        cityNames = cityNames.substring(0, 20) + "...<span style='color:blue;cursor:pointer;'>更多</span>";
                                    }
                                    return '<span>' + cityNames + '</span>';
                                }
                            },
                            { field: 'Gender', title: '性别', sortable: true },
                            { field: 'CornerType', title: '角落类型', sortable: true },
                            { field: 'MachineFrameNames', title: '器架类型', sortable: true },
                            { field: 'CityTier', title: '城市级别', sortable: true },
                            { field: 'Format', title: '店铺类型', sortable: true },
                            { field: 'MaterialSupport', title: '物料支持类别', sortable: true },
                            { field: 'POSScale', title: '店铺规模', sortable: true },
                            { field: 'IsInstall', title: '是否安装', sortable: true },
                            { field: 'Quantity', title: '数量' },
                            { field: 'GraphicMaterial', title: 'POP材质' },
                            { field: 'POPSize', title: 'POP尺寸', sortable: true },
                            { field: 'WindowSize', title: 'Window尺寸' },
                            { field: 'IsElectricity', title: '通电否' },
                            { field: 'ChooseImg', title: '选图' },
                            { field: 'KeepPOPSize', title: '是否保留POP原尺寸' }
                ]],
                    singleSelect: true,
                    //toolbar: '#toolbar',
                    striped: true,
                    border: false,
                    pagination: false,
                    fitColumns: false,
                    height: 'auto',
                    fit: false,
                    iconCls: 'icon-save',
                    emptyMsg: '没有相关记录',
                    onLoadSuccess: function (data) {
                        alert("loadOk");
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
            }
        })

    }
}

function commonExpandRow(index, row, target, nextName) {

    var curName = nextName + '_' + index;
    var id = row.Id;
    $('#' + curName).datagrid({
        method: 'get',
        url: '/Subjects/Handler/SplitOrder.ashx',
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
                    { field: 'NewChooseImg', title: '新选图' },
                    { field: 'Remark', title: '备注' },
                    { field: 'IsInSet', title: '男女共一套' }
                    

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