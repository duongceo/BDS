var baseListing = (function () {
    var grid,
        init = function () {
            console.log('init');
            grid = $("#_list").data("kendoGrid");

            $('#btnSearch').click(function (e) {
                onSearch();
            });

            $("#Keyword").keyup(function (e) {
                if (e.key === 'Enter' || e.keyCode === 13) {
                    onSearch();
                }
            });

            $(window).keydown(function (event) {
                if (event.keyCode == 13) {
                    console.log('catch');
                    event.preventDefault();
                    onSearch();
                    return false;
                }
            });
        },

        additionalData = function () {
            return {
                'Keyword': $("#Keyword").val()
            };
        },

        onSearch = function () {
            grid.dataSource.page(1);
        },
        onPressSearch = function () {
            var key = window.event.keyCode;
            if (key == 13) {
                onSearch();
            }
        }

    return {
        init: init,
        additionalData: additionalData,
        onPressSearch: onPressSearch,
        onSearch: onSearch
    };
}())