var roleGroup = {
    detail: {
        show: function (id) {
            var dialog = $("#_detail");
            dialog.data("kendoWindow").refresh({
                url: common.mapPath('/RoleGroup/Detail/' + id)
            });
            dialog.data("kendoWindow").center();
            dialog.data("kendoWindow").open();
        },
        update: function () {
            var data = $('form#ajaxform').serializeArray();
            let roles = 0;
            $.each($("#lckRoles").data("kendoMultiSelect")._values, function (n, i) {
                roles += parseInt(i);
            });
            data.find(x => x.name === 'Roles').value = roles;
            restfulSvc.post('/RoleGroup/_IU', data, function () {
                close();
                $("#_list").data("kendoGrid").dataSource.read();
            });
        },
        close: function () {
            $("#_detail").data("kendoWindow").close();
        },
        onClose: function () {
            var grid = $("#_list").data("kendoGrid");
            grid.dataSource.read();
        }
    }
}