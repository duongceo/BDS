var historyLog = (function() {
    var sendComment = function (tableName, tableKeyId) {
            send(tableName, tableKeyId, $('#commentText').val());
        },

        quickComment = function (tableName, tableKeyId, s)
        {
            send(tableName, tableKeyId, s);            
        },

        send = function (tableName, tableKeyId, comment) {
            if (comment == null || (comment || '') == '') {
                alert('Vui lòng nhập nội dung bình luận!');
                return;
            }
            $.ajax({
                type: "POST",
                url: common.mapPath('/HistoryLog/_IU'),
                data: { tableName: tableName, tableKeyId: tableKeyId, contents: comment},
                dataType: "json",
                success: function () {
                    $("#_list").data("kendoGrid").dataSource.read();
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
            if (confirm("Bạn có chắc muốn xóa thông tin này không?") == true) {
                $.ajax({
                    type: "POST",
                    url: common.mapPath('/HistoryLog/_Delete'),
                    data: { id: id },
                    dataType: "json",
                    success: function() {
                        $("#_list").data("kendoGrid").dataSource.read();
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