var report = (function() {
    var grid,
        init = function () {
        },
        getSummary = function () {
            var q = {
                FromDate: $("#FromDate").val(),
                ToDate: $("#ToDate").val()
            }
            console.log(q);
            restfulSvc.post('/report/summary', q, function (res) {
                $('#total_property').text(res.data.TotalProperty);
                $('#total_customer').text(res.data.TotalCustomer);
                $('#total_saleorder').text(res.data.TotalSaleOrder);
                $('#total_user').text(res.data.TotalUser);
            });
        };
        
    return {
        grid: grid,
        init: init,
        getSummary: getSummary
    };
}());