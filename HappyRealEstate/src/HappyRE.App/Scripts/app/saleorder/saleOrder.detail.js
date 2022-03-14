var saleOrderDetail = {
    update: function () {
        var data = $('form#ajaxform').serializeArray();
        let ownerTargetId=0, customerTargetId=0;
        $.each($("#OwnerTargetIds").data("kendoMultiSelect")._values, function (n, i) {
            ownerTargetId += parseInt(i);
        });
        $.each($("#CustomerTargetIds").data("kendoMultiSelect")._values, function (n, i) {
            customerTargetId += parseInt(i);
        });
        
        data.find(x => x.name === 'OwnerTargetId').value = ownerTargetId;
        data.find(x => x.name === 'CustomerTargetId').value = customerTargetId;

        if (data.find(x => x.name === 'PostedBy') == undefined) {
            data.push({ 'name': 'PostedBy', 'value': $("#PostedBy").val()});
        }

        var onp = (data.find(x => x.name === 'OwnerPhone').value || '');
        if (onp.length>0 && onp == data.find(x => x.name === 'CustomerPhone').value) {
            alert('SĐT chủ nhà và khách hàng trùng nhau, vui lòng kiểm tra lại!');
            return;
        }

        restfulSvc.post('/SaleOrder/_IU', data, function (res) {
            location.href = "/SaleOrder/detail/" + res;
            //location.href = "/SaleOrder/";
        });
    },
    updateCustomer: function () {
        var data = $('form#ajaxform').serializeArray();
        restfulSvc.post('/saleorder/_UpdateCustomer', data, function () {
            $("#_detail").data("kendoWindow").close();
            toastr.success("Thông tin sẽ được gửi đến tổng giám đốc và hoàn toàn bảo mật","Thành công!")
            $("#_list").data("kendoGrid").dataSource.read();
        });
    },
    showCustomerUpdateModal: function (id) {
        var dialog = $("#_detail");
        dialog.data("kendoWindow").refresh({
            url: common.mapPath('/saleorder/UpdateCustomer/' + id)
        });
        dialog.data("kendoWindow").center();
        dialog.data("kendoWindow").open();
    },
}