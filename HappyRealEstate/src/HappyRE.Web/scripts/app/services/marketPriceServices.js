function marketPrice($http) {
    return {
        GetHousePriceSummary_ByDistrict: GetHousePriceSummary_ByDistrict,
        GetHousePriceSummary_ByStreet: GetHousePriceSummary_ByStreet,
        GetHousePriceMonthly_ByStreet: GetHousePriceMonthly_ByStreet,
        GetHousePrice_TopBy_AvgPrice: GetHousePrice_TopBy_AvgPrice,
        GetHousePrice_TopBy_Total: GetHousePrice_TopBy_Total,
        GetHousePrice_Street_vs_District: GetHousePrice_Street_vs_District,
        PropertySummaryByPriceRange: PropertySummaryByPriceRange
    };

    function GetHousePriceSummary_ByDistrict(districtId, month) {
        return $http({ method: 'GET', url: mogiRoutes.HousePrice.GetHousePriceSummary_ByDistrict + '?districtId=' + districtId + '&month=' + month });
    };
    function GetHousePriceSummary_ByStreet(districtId, streetId, month) {
        return $http({ method: 'GET', url: mogiRoutes.HousePrice.GetHousePriceSummary_ByStreet + '?districtId=' + districtId + '&streetId=' + streetId + '&month=' + month });
    };
    function GetHousePriceMonthly_ByStreet(districtId, streetId, month) {
        return $http({ method: 'GET', url: mogiRoutes.HousePrice.GetHousePriceMonthly_ByStreet + '?districtId=' + districtId + '&streetId=' + streetId + '&month=' + month });
    };
    function GetHousePrice_TopBy_AvgPrice(districtId) {
        return $http({ method: 'GET', url: mogiRoutes.HousePrice.GetHousePrice_TopBy_AvgPrice + '?districtId=' + districtId });
    };
    function GetHousePrice_TopBy_Total(districtId) {
        return $http({ method: 'GET', url: mogiRoutes.HousePrice.GetHousePrice_TopBy_Total + '?districtId=' + districtId });
    };
    function GetHousePrice_Street_vs_District(districtId, streetId, month) {
        return $http({ method: 'GET', url: mogiRoutes.HousePrice.GetHousePrice_Street_vs_District + '?districtId=' + districtId + '&streetId=' + streetId });
    };
    function PropertySummaryByPriceRange(data) {
        return $http({ method: 'POST', url: mogiRoutes.HousePrice.PropertySummaryByPriceRange, params: data });
    }
};
marketPrice.$inject = ["$http"];
mogiApp.factory('marketPriceServices', marketPrice);