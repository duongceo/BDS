mogiApp.controller('menuController', ['$scope', function ($scope) {
    $scope.Data = {
        Items: mogiDatas.Menu,
        SelectedId: 0,
        IsSelected: function (o) {
            return (o.Id == this.SelectedId);
        },
        Active: function (v) {
            var f = _.find(this.Items, function (o) { return o.Code == v; });
            if (f) this.SelectedId = f.Id;
        }
    };
}]);