var notificationDetail = {
    customerId:0,
    send: function () {
        var data = $('form#ajaxform').serializeArray();
        //let statusId = 0;
        //$.each($("#StatusIds").data("kendoMultiSelect")._values, function (n, i) {
        //    statusId += parseInt(i);
        //});

        //data.find(x => x.name === 'StatusId').value = statusId;

        restfulSvc.post('/notification/_Send', data, function (res) {
            location.href = "/notification/detail/" + res;
        });
    },
    unReadCount: function () {
        restfulSvc.get('/notification/_unReadCount', {}, function (res) {
            if (res > 0) {
                $('#noti_count').show();
                $('#noti_count').text(res);
            } else $('#noti_count').hide();
        });
    },
    read: function () {
        restfulSvc.get('/notification/_read', {}, function (res) {
        });
    },
    search: function () {
        restfulSvc.get('/notification/_search', {}, function (res) {
            var li = '';
            $(res.data).each(function (i, v) {
                var cls = v.IsRead == 0 ? "not-read" : "";
                li += '<li class="' + cls +'"><a href="/notification/detail/' + v.Id + '"><span><span>' + v.Title + '</span><span class="time">' + v.TimeDisplay +'</span></span></a></li>';
            });
            $('#noti_list').html(li);
        });
    }
}