var historyLog = (function() {
    var sendComment = function (tableName, tableKeyId) {
            send(tableName, tableKeyId, $('#commentText').val());
        },

        quickComment = function (tableName, tableKeyId, s)
        {
            send(tableName, tableKeyId, s);            
        },

        send = function(tableName, tableKeyId, comment, phone) {
            $.ajax({
                type: "POST",
                url: common.mapPath('/Customer/_SendHistoryLog'),
                data: { TableName: tableName, TableKeyId: tableKeyId, text: comment},
                dataType: "json",
                success: function () {
                    $("#girdHistoryLog").data("kendoGrid").dataSource.read();
                    if (comment =="Gửi tin nhắn nhắc bắt máy") toastr.success("Đã gửi!");
                    $("#commentText").val("");
                    $("#commentText").focus();
                },
            });
        },

        sendCommentEnter = function(tableName, tableKeyId) {
            var key = window.event.keyCode;

            // If the user has pressed enter
            if (key == 13) {
                sendComment(tableName, tableKeyId);
                return false;
            } else {
                return true;
            }
        },

        onRemove = function(id) {
            if (confirm("Bạn có chắc muốn xóa thông này không?") == true) {
                $.ajax({
                    type: "POST",
                    url: common.mapPath('/Customer/_DeleteHistory'),
                    data: { Id: id },
                    dataType: "json",
                    success: function() {
                        $("#girdHistoryLog").data("kendoGrid").dataSource.read();
                    },                    
                });
            }
        };
    return {
        sendComment: sendComment,
        quickComment: quickComment,
        sendCommentEnter: sendCommentEnter,
        onRemove: onRemove
    };
}())