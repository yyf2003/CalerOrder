
var dataJsonStr = "";
$(function () {
    CheckPrimission(url, null, null, $("#btnEdit"), $("#btnDelete"), null, $("#separator1"));
    Price.getList();
    $("#btnEdit").click(function () {

        Price.edit();

    })

    $("#btnAddNewRow").click(function () {
        var tr = "<tr class='tr_bai'>";
        tr += "<td><input type='text' name='txtReceivePrice' value='0' style='text-align:center; width:90%;'/></td><td><input type='text'  name='txtPayPrice' value='0' style='text-align:center;width:90%;'/></td><td>设置默认<input type='radio' name='radioIsDefault' value='1'/>&nbsp;&nbsp;&nbsp;<span name='deleteSpan' data-id='0' style='color:red;cursor:pointer;'>删除</span></td>";
        tr += "</tr>";
        $("#editBody").append(tr);
    })

    $("#btnDelete").click(function () {
        Price.deleteItem();
    })

    $("#editBody").delegate("span[name='deleteSpan']", "click", function () {
        $(this).parent().parent().remove();

    })

    $("#editBody").delegate("input", "blur", function () {
        var val = $.trim($(this).val());

        if (val == "") {
            $(this).val("0");
        }
        else if (isNaN(val)) {
            $(this).val("0");
        }
    })
})


var Price = {
    model: function () {
        this.Id = 0;
        this.ReceivePrice = 0;
        this.PayPrice = 0;
        this.IsDefault = false;
    },
    getList: function () {
        $("#tbList").datagrid({
            method: 'get',
            url: 'List.ashx?type=getList',
            columns: [[
               { field: 'Id', hidden: true },
               { field: 'RowIndex', title: "序号" },
               { field: 'checked', checkbox: true },
               { field: 'ReceivePrice', title: "应收", width: 120 },
               { field: 'PayPrice', title: "应付", width: 200 },
               { field: 'IsDefault', title: "状态", width: 300, formatter: function (data) {
                   if (data == 1) {
                       return "默认";
                   }
               }
               }

            ]],
            toolbar: '#toolbar',
            singleSelect: false,
            striped: true,
            border: false,
            pagination: false,
            fitColumns: false,
            height: 'auto',
            fit: false,
            iconCls: 'icon-save',
            emptyMsg: '没有相关记录',
            //selectOnCheck: false,
            //checkOnSelect:false,
            onClickRow: function (rowIndex, rowData) {
                $(this).datagrid("unselectRow", rowIndex);

            }
        })
    },
    edit: function () {
        $("#editBody").html("");
        var data = $("#tbList").datagrid("getData");

        var json = data.rows;
        if (json.length > 0) {
            var tr = "";
            for (var i = 0; i < json.length; i++) {

                var checked = "";
                if (json[i].IsDefault == 1)
                    checked = "checked='checked'";
                tr += "<tr class='tr_bai'>";
                tr += "<td><input type='text' name='txtReceivePrice' value='" + json[i].ReceivePrice + "' style='text-align:center;width:90%;'/></td><td><input type='text'  name='txtPayPrice' value='" + json[i].PayPrice + "' style='text-align:center;width:90%;'/></td><td>设置默认<input type='radio' name='radioIsDefault' value='1' " + checked + "/>&nbsp;&nbsp;&nbsp;<span name='deleteSpan' data-id='" + json[i].Id + "' style='color:red;cursor:pointer;'>删除</span></td>";
                tr += "</tr>";
            }
            $("#editBody").append(tr);
        }
        $("#editDiv").show().dialog({
            modal: true,
            width: 700,
            height: 450,
            iconCls: 'icon-add',
            resizable: false,
            buttons: [{
                text: '提交',
                iconCls: 'icon-ok',
                handler: function () {
                    if (CheckSubmitVal()) {

                        if (dataJsonStr != "") {
                            $.ajax({
                                type: 'post',
                                url: 'List.ashx',
                                data: { type: "edit", jsonStr: escape(dataJsonStr) },
                                success: function (data) {
                                    if (data = "ok") {
                                        $("#editDiv").dialog('close');
                                        $("#tbList").datagrid("reload");
                                    }
                                    else
                                        alert(data);
                                }
                            })
                        }

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
    },
    deleteItem: function () {
        var rows = $("#tbList").datagrid("getSelections");
        var ids = "";
        for (var i = 0; i < rows.length; i++) {
            ids += rows[i].Id + ",";
        }
        if (ids.length == 0) {
            alert("请选择要删除的行");
            return false;
        }
        if (confirm("确定删除吗？")) {
            $.ajax({
                type: 'post',
                url: 'List.ashx',
                data: { type: "delete", ids: ids },
                success: function (data) {
                    if (data = "ok") {
                        $("#tbList").datagrid("reload");
                    }
                    else
                        alert(data);
                }
            })
        }
    }
}

function CheckSubmitVal() {
    var rows = $("#editBody tr");
    dataJsonStr = "";
    var count = 0;
    if (rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            var tr = rows[i];
            var id = $(tr).find("span[name='deleteSpan']").data("id") || 0;

            var receivePrice = $(tr).find("input[name='txtReceivePrice']").val()||0;
            var payPrice = $(tr).find("input[name='txtPayPrice']").val()||0;
            var isDefault = 0;
            isDefault = $(tr).find("input:radio[name='radioIsDefault']:checked").val() || 0;
           
            if (receivePrice > 0 && payPrice>0) {
                dataJsonStr += '{"Id":' + id + ',"ReceivePrice":' + receivePrice + ',"PayPrice":"' + payPrice + '","IsDefault":' + isDefault + '},';
                count++;
            }
        }
        dataJsonStr = "[" + dataJsonStr.substring(0, dataJsonStr.length - 1) + "]";

    }
    
    if (count==0)
    {
        alert("没有可提交的数据");
        return false;
    }
    else
        return true;
}