var propertyDetail = {
    update: function (isTemp) {
        var data = $('form#ajaxform').serializeArray();
        let statusId = 0, typeId = 0, strongId = 0, weakId=0,contructId=0, structureId=0, potentialId=0, utilityId=0;
        $.each($("#StatusIds").data("kendoMultiSelect")._values, function (n, i) {
            statusId += parseInt(i);
        });
        $.each($("#TypeIds").data("kendoMultiSelect")._values, function (n, i) {
            typeId += parseInt(i);
        });
        $.each($("#StrongIds").data("kendoMultiSelect")._values, function (n, i) {
            strongId += parseInt(i);
        });
        $.each($("#WeakIds").data("kendoMultiSelect")._values, function (n, i) {
            weakId += parseInt(i);
        });
        $.each($("#ContructIds").data("kendoMultiSelect")._values, function (n, i) {
            contructId += parseInt(i);
        });
        $.each($("#StructureIds").data("kendoMultiSelect")._values, function (n, i) {
            structureId += parseInt(i);
        });
        $.each($("#PotentialIds").data("kendoMultiSelect")._values, function (n, i) {
            potentialId += parseInt(i);
        });
        $.each($("#UtilityIds").data("kendoMultiSelect")._values, function (n, i) {
            utilityId += parseInt(i);
        });

        data.find(x => x.name === 'StatusId').value = statusId;
        data.find(x => x.name === 'TypeId').value = typeId;
        data.find(x => x.name === 'StrongId').value = strongId;
        data.find(x => x.name === 'WeakId').value = weakId;
        data.find(x => x.name === 'ContructId').value = contructId;
        data.find(x => x.name === 'StructureId').value = structureId;
        data.find(x => x.name === 'PotentialId').value = potentialId;
        data.find(x => x.name === 'UtilityId').value = utilityId;
        data.find(x => x.name === 'IsTemp').value = isTemp || false;

        if (data.find(x => x.name === 'PostedBy') == undefined) {
            data.push({ 'name': 'PostedBy', 'value': $("#PostedBy").val() });
        }

        //data.find(x => x.name === 'Width').value = '6,5';
        restfulSvc.post('/Property/_IU', data, function (res) {
            location.href = "/property/detail/" + res;
            //location.href = "/property/";
        });
    },
    updateTemp: function () {
        this.update(true);
    },
    checkCode: function () {
        var id = $("#Id").val(), code = $("#Code").val();

        restfulSvc.get('/Property/_IsExistsCode', { id: id, code: code }, function (res) {
            if (res == true) toastr.error('Mã BĐS đã có trong hệ thống, hãy chọn mã khác!');
        });
    },
    saveImage: function () {
        var data = $('form#ajaxform').serializeArray();
        restfulSvc.post('/Property/_AddImages', data, function (res) {
            $("#_listImage").data("kendoGrid").dataSource.read();
            $("#btn_upload_img").hide();
            $("#img_list_more").html('');
        });
    },
    removeImg: function (item, e) {
        $(item).closest('li').remove();
    }
}