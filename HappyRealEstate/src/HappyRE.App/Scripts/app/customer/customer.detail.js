var customerDetail = {
    customerId: 0,
    regionTarget:[],
    init: function () {
        var dataSource = new kendo.data.DataSource({
            data: customerDetail.regionTarget
        });
        var grid = $("#_list").data("kendoGrid");
        grid.setDataSource(dataSource);
    },
    update: function () {
        var data = $('form#ajaxform').serializeArray();
        let statusId = 0, demandId = 0, targetId = 0, directionId=0;
        $.each($("#StatusIds").data("kendoMultiSelect")._values, function (n, i) {
            statusId += parseInt(i);
        });
        $.each($("#DemandIds").data("kendoMultiSelect")._values, function (n, i) {
            demandId += parseInt(i);
        });
        $.each($("#TargetIds").data("kendoMultiSelect")._values, function (n, i) {
            targetId += parseInt(i);
        });
        $.each($("#DirectionIds").data("kendoMultiSelect")._values, function (n, i) {
            directionId += parseInt(i);
        });

        data.find(x => x.name === 'StatusId').value = statusId;
        data.find(x => x.name === 'DemandId').value = demandId;
        data.find(x => x.name === 'TargetId').value = targetId;
        data.find(x => x.name === 'DirectionId').value = directionId;

        var id = data.find(x => x.name === 'Id').value;

        if (id == 0 && customerDetail.regionTarget.length == 0) {
            alert("Vui lòng chọn khu vực mong muốn!");
            return;
        }
        if (id == 0) data.find(x => x.name === 'RegionTargets').value = JSON.stringify(customerDetail.regionTarget);


        restfulSvc.post('/Customer/_IU', data, function (res) {         
            //location.href = "/customer/edit/" + res;
            //toastr.success("Đã lưu!");
                location.href = "/customer/detail/" + res;
        });
    },
    showUserRegionModal: function (_refId) {
        customerDetail.customerId = _refId;
        var dialog = $("#_regionModal");
        dialog.data("kendoWindow").refresh({
            url: common.mapPath('/Customer/RegionModal')
        });
        dialog.data("kendoWindow").center();
        dialog.data("kendoWindow").open();
    },
    regionAdditionalData: function () {
        return {
            'CityId': $("#cmbCity").val(),
            'DistrictId': $("#cmbDistrict").val(),
            'Keyword': $("#Keyword").val()
        }
    },
    filterDistricts: function () {
        return {
            cityId: $("#cmbCity").val()
        }
    },
    regionSearch : function () {
        $("#_listModal").data("kendoGrid").dataSource.page(1);
    },
    chooseRegion: function (cityId, districtId, wardId, streetId, cityName, districtName, wardName, streetName) {
        var data = {
            customerId: customerDetail.customerId,
            cityId: cityId,
            districtId: districtId,
            wardId: wardId,
            streetId: streetId
        }
        if (customerDetail.customerId > 0) {
            restfulSvc.post('/CustomerRegionTarget/_IU', data, function () {
                $("#_regionModal").data("kendoWindow").close();
                $("#_list").data("kendoGrid").dataSource.read();
            });
        } else {
            var m = {
                "Id": 0,
                "CustomerId": 0,
                "CityId": cityId,
                "DistrictId": districtId,
                "WardId": wardId,
                "StreetId": streetId,
                "StreetName": streetName,
                "DistrictName": districtName,
                "CityName": cityName,
                "WardName": wardName
            }
            var h = $.grep(customerDetail.regionTarget, function (v, i) {
                return v.CityId == m.CityId && v.DistrictId == m.DistrictId && v.WardId== m.WardId && v.streetId == m.StreetId;
            });
            if (h.length == 0) {
                customerDetail.regionTarget.push(m);

                var dataSource = new kendo.data.DataSource({
                    data: customerDetail.regionTarget
                });
                var grid = $("#_list").data("kendoGrid");
                grid.setDataSource(dataSource);
            }
        }
    },
    checkPhoneExists: function () {
        var val = $('#Phone').val() || '';
        if (val.length < 8) {
            console.log(val);
            return;
        }
        restfulSvc.get('/Customer/_CheckPhoneExists', { phone: val }, function (res) {
            if (res == true) alert('Khách hàng với số điện thoại này đã có trong hệ thống!');
        });
    }
}