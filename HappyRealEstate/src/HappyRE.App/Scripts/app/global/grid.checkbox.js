var gridCheckbox = (function() {
    var selectedObjects = [],
        init = function() {
            $('#mastercheckbox').click(function () {                
                $('.checkboxGroups').attr('checked', $(this).is(':checked')).change();                
            });
        },

        onChecked = function(val) {
            if ($("#checked_" + val).is(':checked'))
                updateList(val, 'add'); // checked
            else
                updateList(val, 'remove'); // unchecked
        },

        updateList = function(val, action) {
            var isExist = false;
            $.each(selectedObjects, function (i) {
                if (selectedObjects[i] == val) {

                    if (action == 'add') {
                        isExist = true;
                        return;
                    }

                    if (action == 'remove') {
                        selectedObjects.splice(i, 1);
                    }
                }
            });

            if (action == 'add' && !isExist) {
                selectedObjects.push(val);
            }
        };
    return {
        selectedObjects: selectedObjects,
        init: init,
        onChecked: onChecked,
    };
}())