mogiApp.directive('mgModalDialog', function modelDialog() {
    return {
        restrict: 'E',
        scope: {
            show: '='
        },
        replace: true, // Replace with the template below
        transclude: true, // we want to insert custom content inside the directive
        link: function (scope, element, attrs) {
            scope.dialogStyle = {};
            if (attrs.width)
                scope.dialogStyle.width = attrs.width;
            if (attrs.height)
                scope.dialogStyle.height = attrs.height;
            scope.hideModal = function () {
                scope.show = false;
            };
        },
        template: "<div class='mg-modal' ng-class='{visible: show}'>" +
            "<div class='mg-modal-overlay' ng-click='hideModal()'></div>" +
            "<div class='mg-modal-dialog' ng-style='dialogStyle'>" +
            "<div class='mg-modal-close' ng-click='hideModal()'>X</div>" +
            "<div class='mg-modal-dialog-content' ng-transclude></div></div></div>"
    };
});


