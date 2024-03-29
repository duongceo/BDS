﻿mogiApp.directive('mgInputFormater', ['$filter', function ($filter) {
    return {
        require: '?ngModel',
        link: function (scope, elem, attrs, ctrl) {
            if (!ctrl) return;


            ctrl.$formatters.unshift(function (a) {
                return $filter(attrs.mgInputFormater)(ctrl.$modelValue)
            });


            ctrl.$parsers.unshift(function (viewValue) {
                var plainNumber = viewValue.replace(/[^\d|\-+|\,+]/g, '');
                elem.val($filter(attrs.mgInputFormater)(plainNumber));
                return plainNumber;
            });
        }
    };
}]);