function imageServices(Upload) {
    return service = {
        UploadImage: UploadImage
    };

    function UploadImage(file, referCode, callback, err_callback) {
        if (file.type.indexOf('image/') != 0) {
            err_callback(mogiDatas.Msg.Msg_ProfileImage_Type_Error);
            return;
        }
        if (file.size >= 1024 * 1024 * 5) {
            err_callback(mogiDatas.Msg.Msg_ProfileImage_Size_Error);
            return;
        }
        file.upload = Upload.upload({
            url: mogiRoutes.Upload.UploadImage,
            headers: {
                'optional-header': 'header-value'
            },
            data: { referCode: referCode, file: file }
        });
        file.upload.then(function (response) {
            file.result = response.data;

            if (response.data!=null && response.data.success) {                
                var uploadFile = {
                    Url: response.data.PublishUrl,
                    MediaId: response.data.MediaId,
                    isUsed: false,
                    isChecked: false,
                    MediaTypeId: response.data.MediaTypeId,
                    ServerId: response.data.ServerId,
                    MediaUrl: response.data.MediaUrl,
                    style: {},
                    SmallUrl: function () {
                        return this.Url.replace(this.MediaUrl, "thumb-small/" + this.MediaUrl);
                    },
                    AvatarUrl: function () {
                        return this.Url.replace(this.MediaUrl, "thumb-avatar/" + this.MediaUrl);
                    }
                };
                if (callback) callback(uploadFile);
            }
            else {
                err_callback(response.data.error);
            }
        },
        function (response) {
            if (response.status > 0 && err_callback) err_callback(mogiDatas.Msg.Msg_ProfileImage_Type_Error)
        }
        , function (evt) {
            // Math.min is to fix IE which reports 200% sometimes
            file.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
        });

        file.upload.xhr(function (xhr) {
            // xhr.upload.addEventListener('abort', function(){console.log('abort complete')}, false);
        });
    }
}
imageServices.$inject = ["Upload"];
mogiApp.factory('imageServices', imageServices);