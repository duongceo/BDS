mogiApp.directive('mgSlider', function () {
    return {
        require: 'ngModel',
        scope: {
            options: '='
        },
        link: function (scope, el, atts, ngModel) {
            el.slider(scope.options);
            console.log(scope.options);
            ngModel.$render = function () {
                el.slider('setValue', ngModel.$viewValue, true, true);
            }
        }
    }
});
