var baseListing = (function () {
    var grid, counter = 1,
        init = function () {          
            grid = $("#_list").data("kendoGrid");

            $('#btnSearch').click(function (e) {
                onSearch();
            });

            //$("#Keyword").keyup(function (e) {
            //    if (e.key === 'Enter' || e.keyCode === 13) {
            //        onSearch();
            //    }
            //});

            $(window).keydown(function (event) {
                if (event.key === 'Enter' || event.keyCode == 13) {
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
            counter = 1;
            grid.dataSource.page(1);

        },
        onPressSearch = function () {
            var key = window.event.keyCode;
            if (key == 13) {
                onSearch();
            }
        },
        onDataBoundHandler = function (e) {
            //var pz = $("#_list").data("kendoGrid").dataSource.pageSize();
            //var p = $("#_list").data("kendoGrid").dataSource.page();
           
            counter = 1;
            if ($('.pagerTop').length == 0) {
                var grid = $('#_list').data('kendoGrid');
                var wrapper = $('<div class="k-pager-wrap k-grid-pager pagerTop"/>').insertBefore(grid.element.children("table"));
                grid.pagerTop = new kendo.ui.Pager(wrapper, $.extend({}, grid.options.pageable, { dataSource: grid.dataSource }));
                grid.element.height("").find(".pagerTop").css("border-width", "0 0 1px 0");
            }
        },
        renderNumber = function (data) {
            return counter++;
        }

    return {
        init: init,
        additionalData: additionalData,
        onPressSearch: onPressSearch,
        onSearch: onSearch,
        renderNumber: renderNumber,
        onDataBoundHandler: onDataBoundHandler
    };
}())