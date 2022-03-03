var saleOrderListing = (function() {
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

        onSearch = function () {
            grid.dataSource.page(1);
        },
        onDelete = function (id) {
            restfulSvc.delete('/saleorder/_Delete', { id: id }, function () {
                var pathname = window.location.pathname.toLowerCase();
                if (pathname.indexOf('detail') > 0) location.href = '/saleorder'
                else $("#_list").data("kendoGrid").dataSource.read();
            });
        };
        
    return {
        grid: grid,
        init: init,
        onSearch: onSearch,
        delete: onDelete
    };
}());