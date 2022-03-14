var imageFile = (function() {
    var onDelete = function(item,id) {
            if (confirm("Bạn có chắc muốn xóa hình này không?") == true) {
                $.ajax({
                    type: "POST",
                    url: common.mapPath('/ImageFile/_Delete'),
                    data: { id: id },
                    dataType: "json",
                    success: function() {
                        //$(item).closest('li').remove();
                        $("#_listImage").data("kendoGrid").dataSource.read();
                    },                    
                });
            }
        };
    return {
        delete: onDelete
    };
}())