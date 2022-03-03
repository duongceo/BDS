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
        onComplete: onComplete
    };
}())