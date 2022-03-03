var restfulSvc = {
    post: function (url, data, callback) {
        var myErrorTemplate = '<span class="k-widget k-tooltip k-tooltip-validation custom_validationSection">' +
            '<span class="k-icon k-i-error"></span >Không được để trống</span>';

        var validator = $("form#ajaxform").kendoValidator({
           //validateOnBlure: true, //validate on blur event    
            //errorTemplate: myErrorTemplate //set the new error template  
            messages: {
                required: "Không được để trống",
                pattern: "Không hợp lệ"
            },
            rules: {
                custom: function (input) {
                    var vnf_regex = /^(03|05|07|08|09)[0-9]{8}$/;
                    if (input.is("[name=Phone]") || input.is("[name=OwnerPhone]") || input.is("[name=Mobile]")) {
                        return vnf_regex.test(input.val());
                    } else if (input.is("[name=CustomerPhone]") && input.val().length > 0) {
                        return vnf_regex.test(input.val());
                    } else if (input.is("[name=BudgetTo]") && input.val().length>0) {
                        var from = $('#BudgetFrom').val() || '0';
                        return parseFloat(input.val()) >= parseFloat(from);
                    } else if (input.is("[name=BudgetFrom]") && input.val().length > 0) {
                        var to = $('#BudgetTo').val() || '0';
                        return parseFloat(input.val()) <= parseFloat(to);
                    }
                return true;
            }
        }
        }).data("kendoValidator");
        if ($("form#ajaxform").length>0 && validator) {
            if (validator.validate() == false) {
                return;
            }
        }

        $.ajax({
            type: "POST",
            url: common.mapPath(url),
            data: data,
            dataType: "json",
            success: function (result) {
                if (result.message && result.message.length > 0) toastr.success(result.message);
                if (callback) callback(result)
            },
            error: function (xhr, status, error) {
                console.log(xhr);
                if (xhr != undefined && xhr != null && xhr.responseJSON && xhr.responseJSON.length>0)
                    toastr.error(xhr.responseJSON);
                else toastr.error("Lỗi! Lưu không thành công!")
            }
        });
    },
        get: function (url, params, callback) {
            $.ajax({
                type: "GET",
                url: common.mapPath(url),
                data: params,
                dataType: "json",
                success: function (result) {
                    //window.close();
                    if (callback) callback(result)
                },
                error: function (xhr, status, error) {
                    console.log(xhr);
                    //console.log(status);
                    if (xhr != undefined && xhr != null) console.log(xhr.readyState);
                    if (xhr != undefined && xhr != null && xhr.responseJSON && xhr.responseJSON.length > 0)
                        toastr.error(xhr.responseJSON);
                    //else toastr.error("Lỗi! Lưu không thành công!")
                }
            });
        },
        put: function (url, params) {
            $.ajax({
                type: "PUT",
                url: common.mapPath(url),
                data: data,
                dataType: "json",
                success: function (result) {
                    window.close();
                    if (callback) callback(result)
                },
                error: function (xhr, status, error) {
                    console.log(xhr);
                    toastr.error("Đã có lỗi xảy ra, vui lòng liên hệ quản trị");
                }
            });
        },
    delete: function (url, data, callback) {
        if (confirm("Bạn có chắc muốn xóa thông tin này không?") == false) {
            return;
        }  
            $.ajax({
                type: "DELETE",
                url: common.mapPath(url),
                data: data,
                dataType: "json",
                success: function (result) {
                    //window.close();
                    if (callback) callback(result)
                },
                error: function (xhr, status, error) {
                    console.log(xhr);
                    console.log(error);
                    if (xhr != undefined && xhr != null && xhr.responseJSON && xhr.responseJSON.length > 0)
                        toastr.error(xhr.responseJSON);
                    else toastr.error("Đã có lỗi xảy ra, vui lòng liên hệ quản trị!")
                }
            });
    },
    deleteDetail: function (url, data, callback) {
        if (confirm("Bạn có chắc muốn xóa thông tin này không?") == false) {
            return;
        }
        $.ajax({
            type: "POST",
            url: common.mapPath(url),
            data: JSON.stringify(data), 
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function () {
                callback();
            },
            error: function () {
                callback();
            }
        });
    }
}