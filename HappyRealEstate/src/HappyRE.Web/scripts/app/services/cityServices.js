function cityServices($http) {
    return {
        GetZoneByCity: GetZoneByCity,
        GetStreetById:GetStreetById
    };

    function GetZoneByCity(cityId) {
        return $http({ method: 'GET', url: mogiRoutes.City.GetZoneByCityId + '?cityId=' + cityId });
    }
    function GetStreetById(streetId) {
        return $http({ method: 'GET', url: mogiRoutes.City.GetStreetById  + '?streetId=' + streetId });
    }
}
cityServices.$inject = ["$http"];
mogiApp.factory('cityServices', cityServices);