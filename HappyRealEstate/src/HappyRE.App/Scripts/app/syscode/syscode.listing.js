var sysCodeListing = (function () {
    var grid,
        init = function () {
            grid = $("#_list").data("kendoGrid");
            $('#btnSearch').click(function (e) {
                onSearch();
            });
        },

        additionalData = function () {
            return {
                'TableId': $("#cmbCity").val(),
                'Keyword': $("#Keyword").val()
            };
        },

        onSearch = function () {
            grid.dataSource.page(1);
        }

    return {
        init: init,
        additionalData: additionalData
    };
}())