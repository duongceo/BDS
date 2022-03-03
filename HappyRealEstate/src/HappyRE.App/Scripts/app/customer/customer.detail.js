var customerDetail = {
    customerId:0,
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
    chooseRegion: function (cityId, districtId, wardId, streetId) {
        var data = {
            customerId: customerDetail.customerId,
            cityId: cityId,
            districtId: districtId,
            wardId: wardId,
            streetId: streetId
        }

        restfulSvc.post('/CustomerRegionTarget/_IU', data, function () {
            $("#_regionModal").data("kendoWindow").close();
            $("#_list").data("kendoGrid").dataSource.read();
        });
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