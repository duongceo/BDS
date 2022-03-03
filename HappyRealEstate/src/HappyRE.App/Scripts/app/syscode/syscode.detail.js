var sysCodeDetail = (function () {
    show = function (name,tableId,id) {
        var dialog = $("#_detail");
        dialog.data("kendoWindow").refresh({
            url: common.mapPath('/' + name + '/Detail/' + id + '?tableId=' + tableId)
        });
        dialog.data("kendoWindow").center();
        dialog.data("kendoWindow").open();
    },
        update = function (name) {
            var data = $('form#ajaxform').serializeArray();
            restfulSvc.post('/' + name + '/_IU', data, function () {
                close();
                $("#_list").data("kendoGrid").dataSource.read();
            });
        },
        close = function () {
            $("#_detail").data("kendoWindow").close();
        },
        filterDistricts = function () {
            return {
                CityId: $("#CityId").val()
            }
        }
    return {
        show: show,
        update: update,
        close: close,
        filterDistricts: filterDistricts
    };
}())