var customerListing = (function() {
    var grid,

        init = function () {
            grid = $("#_list").data("kendoGrid");

            $('#FilterString').keyup(function (e) {
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
            grid.dataSource.page(1);
        },
        showMobile = function (id) {
            var isAdmin = $('#canShowAllMobile').val() || "0";
            restfulSvc.get('/Customer/_TotalViewedMobileToday', {}, function (res) {
                if (isAdmin == "0") {
                    if (confirm("Bạn đã xem " + res + " /10 SĐT được xem mỗi ngày. Bạn muốn xem thêm SĐT khách này?") == false) {
                        return;
                    }
                } else {
                    if (confirm("Bạn đã xem " + res + " SĐT trong hôm nay. Bạn muốn xem thêm SĐT khách này?") == false) {
                        return;
                    }
                }
                restfulSvc.post('/Customer/_ShowMobile', { id: id }, function (res) {
                    if (res.data && res.data.length > 0) {
                        console.log(res.data);
                        $(".info-" + id + " .txt_phone").text(res.data);
                        $(".info-" + id).show();
                        $(".btn-show-mobile-" + id).hide();
                        //detail
                        $("#txt_phone").text(res.data).show();                        
                        $("#btn_phone").hide();
                    }
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
            restfulSvc.post('/Customer/_HideMobile', { id: id, isForced: isForced }, function () {               
                $("#_list").data("kendoGrid").dataSource.read();
            });
        },
        onDelete = function (id) {
            restfulSvc.delete('/customer/_Delete', { id: id }, function () {
                var pathname = window.location.pathname.toLowerCase();
                if (pathname.indexOf('detail') > 0) location.href = '/customer'
                else $("#_list").data("kendoGrid").dataSource.read();
            });
        };
        
    return {
        grid: grid,
        init: init,
        additionalData: additionalData,
        onSearch: onSearch,
        hideMobile: hideMobile,
        showMobile: showMobile,
        delete: onDelete
    };
}());