mogiApp.directive('dropdownSearch', function () {
    return {
        restrict: 'AE',
        scope: {
            BindingModel: '=bindingModel',
            BindingItems: '=bindingItems',
            BindingItemSelected: '=bindingItemSelected'
            //SetItemSelected: '&setItemSelected',
            //GetItems:'&setItems'
        },
        link: function ($scope, $element, $attrs, controllers) {
            $scope.Items = $scope.BindingItems;
            $scope.ElementId = "drop_" + Math.random();
            $scope.SelectedItem = function (obj) {
                $scope.BindingModel.Changed(obj);
            };
			$scope.AutoClose = 'always';
        },
        template:
            '<div class="dropdown dropdown-more-filter" uib-dropdown>' +
                '<a href="#" class="btn btn-default btn-search dropdown-toggle" role="button" uib-dropdown-toggle>' +
                    '<span><span class="more-filter-label">{{BindingModel.Label}}</span><span class="more-filter-value">{{BindingModel.Selected.Name}}</span></span><span class="caret"></span>' +
                '</a>' +
                '<ul class="dropdown-menu" uib-dropdown-menu>' +
                        '<li ng-repeat="item in Items track by $index">' +
                            '<a ng-click="SelectedItem(item)" ng-class="{selected: BindingModel.IsSelected(item)}"><span><i class="icon icon-check" ng-show="BindingModel.IsSelected(item)"></i><span ng-bind-html="item.Name"></span></span></a>' +
                        '</li>' +
                '</ul>' +
            '</div>'
    };
});


