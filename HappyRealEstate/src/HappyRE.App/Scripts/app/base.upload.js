var baseUpload = (function () {   
    onSelect = function(e) {
        console.log("Select :: " + getFileInfo(e));
    }

    onUpload = function(e) {
        var files = e.files;
        $.each(files, function () {
            if (this.extension.toLowerCase() != ".jpg" && this.extension.toLowerCase() != ".png") {
                alert("Chỉ chấp nhận file hình ảnh JPG, PNG!")
                e.preventDefault();
            }

            if (this.size / 1024 / 1024 > 5) {
                alert("Dung lượng file phải nhỏ hơn 5MB!")
                e.preventDefault();
            }
        });
    }

    onSuccess = function(e) {
        if (e.operation == 'upload') {
            $("#Avatar").val(e.response.data);
            $("#avatar_img").attr("src", e.response.data);
        }
    }

    onSuccessMutils = function (e) {
        if (e.operation == 'upload') {
            $("#img_list").append("<li><img src='" + e.response.data + "' /><input type='hidden' name='PropertyImages' value='" + e.response.data + "' /><a onclick='propertyDetail.removeImg(this)'><i class='glyphicon glyphicon-remove'></i></a></li>");
        }
    }

    onSuccessMutilsMore = function (e) {
        console.log(e.operation);
        if (e.operation == 'upload') {
            console.log('ok');
            $("#_listImage").data("kendoGrid").dataSource.read();
            //$("#img_list_more").append("<li><img src='" + e.response.data + "' /><input type='hidden' name='PropertyImages' value='" + e.response.data + "' /><a onclick='propertyDetail.removeImg(this)'><i class='glyphicon glyphicon-remove'></i></a></li>");
        }
    }

    onSuccessList = function (e) {
        if (e.operation == 'upload') {
            $("#img_list").append("<li><img src='" + e.response.data + "' /><input type='hidden' name='Images' value='" + e.response.data + "' /><a onclick='baseUpload.removeImg(this)'><i class='glyphicon glyphicon-remove'></i></a></li>");
        }
    }

    onSuccessListOwner = function (e) {
        if (e.operation == 'upload') {
            $("#img_list_owner").append("<li><img src='" + e.response.data + "' /><input type='hidden' name='OwnerImages' value='" + e.response.data + "' /><a onclick='baseUpload.removeImg(this)'><i class='glyphicon glyphicon-remove'></i></a></li>");
        }
    }

    onSuccessListCustomer = function (e) {
        if (e.operation == 'upload') {
            $("#img_list_customer").append("<li><img src='" + e.response.data + "' /><input type='hidden' name='CustomerImages' value='" + e.response.data + "' /><a onclick='baseUpload.removeImg(this)'><i class='glyphicon glyphicon-remove'></i></a></li>");
        }
    }

    onError = function(e) {
        console.log("Error (" + e.operation + ") :: " + getFileInfo(e));
    }

    onComplete = function (e) {
        console.log("");
    }

    onCancel = function(e) {
        console.log("Cancel :: " + getFileInfo(e));
    }

    onRemove = function(e) {
        console.log("Remove :: " + getFileInfo(e));
    }

    onProgress = function(e) {
        console.log("Upload progress :: " + e.percentComplete + "% :: " + getFileInfo(e));
    }

    removeImg= function (item, e) {
        $(item).closest('li').remove();
    }

    function getFileInfo(e) {
        return $.map(e.files, function (file) {
            var info = file.name;

            // File size is not available in all browsers.
            if (file.size > 0) {
                info += " (" + Math.ceil(file.size / 1024) + " KB)";
            }
            return info;
        }).join(", ");
    }

    return {
        onSuccess: onSuccess,
        onSuccessMutils: onSuccessMutils,
        onSuccessMutilsMore: onSuccessMutilsMore,
        onSuccessList: onSuccessList,
        onSuccessListOwner: onSuccessListOwner,
        onSuccessListCustomer: onSuccessListCustomer,
        onComplete: onComplete,
        removeImg: removeImg
    };
}())