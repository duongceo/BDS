var userListing = (function () {
    var grid, refId,
        init = function () {
            grid = $("#_list").data("kendoGrid");
            $('#btnSearch').click(function (e) {
                onSearch();
            });
            $('#btnSearchModal').click(function (e) {
                onSearchModal();
            });
            $('#btnExport').click(function (e) {
                onExport();
            });
        },
        additionalDataModal = function (){
            return {
                'LevelId_Filter': $("#_cmbLevel").val(),
                'RoleGroupId_Filter': $("#_cmbRoleGroup").val(),
                'DepartmentId_Filter': $("#_cmbDepartment").val(),
                'UserStatus_Filter': $("#cmbUserStatus").val(),
                'Keyword': $("#_Keyword").val()
            };
        },
        additionalData = function () {
            return {
                'LevelId_Filter': $("#cmbLevel").val(),
                'RoleGroupId_Filter': $("#cmbRoleGroup").val(),
                'DepartmentId_Filter': $("#cmbDepartment").val(),
                'UserStatus_Filter': $("#cmbUserStatus").val(),
                'Keyword': $("#Keyword").val()
            };
        },
        onExport = function () {
            window.location.href = '/City/DownloadCSV';
        },
        onSearch = function () {
            $("#_list").data("kendoGrid").dataSource.page(1);
        },
        onSearchModal = function () {
            $("#_listModal").data("kendoGrid").dataSource.page(1);
        },
        showUserListModal = function (_refId) {
            refId = _refId;
            var dialog = $("#_userListModal");
            dialog.data("kendoWindow").refresh({
                url: common.mapPath('/User/ListModal')
            });
            dialog.data("kendoWindow").center();
            dialog.data("kendoWindow").open();
        },
        choose = function (userId) {
            var pathname = window.location.pathname.toLowerCase();
            console.log(pathname);
            console.log(refId);
            if (pathname == '/department') {
                var data = {
                    managerId: userId,
                    id: refId
                }

                restfulSvc.post('/Department/_UpdateManager', data, function () {
                    $("#_userListModal").data("kendoWindow").close();
                    $("#_list").data("kendoGrid").dataSource.read();
                });
            } else if (pathname == '/user') {
                var data = {
                    toUser: userId,
                    fromUser: refId
                }

                restfulSvc.post('/User/_TranferAccount', data, function () {
                    $("#_userListModal").data("kendoWindow").close();
                    $("#_list").data("kendoGrid").dataSource.read();
                });
            }
        },
        show = function (id) {
            var dialog = $("#_detail");
            dialog.data("kendoWindow").refresh({
                url: common.mapPath('/User/Detail/' + id)
            });
            dialog.data("kendoWindow").center();
            dialog.data("kendoWindow").open();
        },
        resetPass = function (id, newPass) {
            newPass = newPass || 'Pw000@111';
            restfulSvc.post('/User/_ResetPass', { id: id, newPassword: newPass }, function () {
                $("#_list").data("kendoGrid").dataSource.read();
            });
        },
        onDelete = function (id) {
            restfulSvc.delete('/user/_Delete', { id: id }, function () {
                $("#_list").data("kendoGrid").dataSource.read();
            });
        },
        upload = function (file) {
            console.log(file)
        }

    return {
        init: init,
        onSearch: onSearch,
        onSearchModal: onSearchModal,
        additionalData: additionalData,
        additionalDataModal: additionalDataModal,
        showUserListModal: showUserListModal,
        choose: choose,
        show: show,
        resetPass: resetPass,
        upload: upload,
        delete: onDelete
    };
}())