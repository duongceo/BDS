var baseDetailPopup = (function () {
    show= function (name,id) {
        var dialog = $("#_detail");
        dialog.data("kendoWindow").refresh({
            url: common.mapPath('/' + name + '/Detail/' + id)
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
        onDelete = function (type,id) {
        restfulSvc.deleteDetail('/' + type + '/Delete', { id: id }, function () {
            location.href = "/" + type;
            });
        },
    close= function () {
        $("#_detail").data("kendoWindow").close();
    },
    filterDistricts = function () {
        return {
            CityId: $("#CityId").val()
        }
        },
        filterWardOrStreets = function () {
            return {
                DistrictId: $("#DistrictId").val()
            }
        }
    return {
        show: show,
        update: update,
        delete: onDelete,
        close: close,
        filterDistricts: filterDistricts,
        filterWardOrStreets: filterWardOrStreets
    };
}())