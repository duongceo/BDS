var propertyListing = (function () {
    var grid,

        init = function () {
            grid = $("#_list").data("kendoGrid");

            $('#Keyword').keyup(function (e) {
                if (e.keyCode == 13) {
                    onSearch();
                }
            });

            $('#Search').click(function (e) {
                onSearch();
            });
        },

        additionalData = function () {
            return {
                'FilterString': $("#FilterString").val(),
                'IsFullSearch': $("#IsFullSearch").is(":checked"),
            };
        },

        onSearch = function () {
            //grid = $("#_list").data("kendoGrid");
            $("#_list").data("kendoGrid").dataSource.page(1);
        },
        showMobile = function (id) {
            restfulSvc.get('/Property/_TotalViewedMobileToday', {}, function (res) {
                if (confirm("Bạn đã xem " + res + " /10 SĐT được xem mỗi ngày. Bạn muốn xem thêm SĐT khách này?") == false) {
                    return;
                }
                restfulSvc.post('/Property/_ShowMobile', { id: id }, function () {
                    //$("#_list").data("kendoGrid").dataSource.read();
                    $(".info-" + id).show();
                    $(".btn-show-mobile-" + id).hide();
                    $("#txt_phone").show();
                    $("#btn_phone").hide();
                });
            });
        },
        hideMobile = function (id, isForced) {
            if (isForced == false) {
                if (confirm("Ẩn SĐT khách hàng này?") == false) {
                    return;
                }
            } else {
                if (confirm("Bỏ ẩn SĐT khách hàng này?") == false) {
                    return;
                }
            }
            restfulSvc.post('/Property/_HideMobile', { id: id, isForced: isForced }, function () {
                $("#_list").data("kendoGrid").dataSource.read();
            });
        },
        onDelete = function (id) {
            restfulSvc.delete('/property/_Delete', { id: id }, function () {
                var pathname = window.location.pathname.toLowerCase();
                if (pathname.indexOf('detail') > 0) location.href ='/property'
                else $("#_list").data("kendoGrid").dataSource.read();
            });
        },
        showListModal = function () {
            var dialog = $("#_propertyListModal");
            dialog.data("kendoWindow").refresh({
                url: common.mapPath('/Property/ListModal')
            });
            dialog.data("kendoWindow").center();
            dialog.data("kendoWindow").open();
        };
        
    return {
        grid: grid,
        init: init,
        additionalData: additionalData,
        onSearch: onSearch,
        hideMobile: hideMobile,
        showMobile: showMobile,
        delete: onDelete,
        showListModal: showListModal
    };
}());