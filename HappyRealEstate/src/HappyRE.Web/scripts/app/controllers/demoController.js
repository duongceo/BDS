
mogiApp.controller('demoController', function ($scope, $http, suggest) {
    $scope.name = 'Vũ Văn Quý';
    $scope.city = { CityId: 1, Name: "TP.HCM" };
    $scope.alert = function () { alert($scope.name); }
    $scope.regex = "[0-9]*";
    $scope.msg01 = "message 01";
    $scope.changeMsg = function () {
        $scope.msg01 = "Thay doi msg";
        suggest.Map($scope.name, $scope.alert);
    }
    $scope.alert = function (res) {
        alert(JSON.stringify(res));
    }
    $scope.submitForm = function () {

        

    };

    $scope.Map = {
        MapId: 0,
        CityId: null,
        DistrictId: null,
        StreetNameId: 0,
        Name: '',
        Enabled: false
    };

    $scope.MapData = [];
    $scope.SetMapData = function (data) {
        $scope.MapData = data;
    }
    $scope.GetMap = function (q) {
        return suggest.Map(q, $scope.SetMapData);

        return suggest.Map(q, $scope.SetMapData).then(function (resp) {
            return suggest.Map_MapObject(q,resp);
        });

    };

    $scope.GetMap('quan');
});