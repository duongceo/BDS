function propertyServices($http) {
	return {
		SearchMap: SearchMap,
		RemoveFavorite: RemoveFavorite,
		AddFavorite: AddFavorite,
		GetFavorites: GetFavorites,
		SendMessageToAgent: SendMessageToAgent,
		GetMessages: GetMessages,
		SendReport: SendReport,
		TopService: TopService,
		GetNext: function GetNext(p) {
			var q = $.param(p);
			return $http({ method: 'GET', url: mogiRoutes.Property.GetNext + '?' + q });
		},
		MarketGetByDistrict: MarketGetByDistrict,
		GetBankInterestRates: GetBankInterestRates,
		GetPropertyByStreet: GetPropertyByStreet,
		GetPropertyByUniqueAddress: GetPropertyByUniqueAddress,
		GetHistoryPropertyByStreet: GetHistoryPropertyByStreet
	};

    function TopService(p) {
        return $http({ method: 'POST', url: mogiRoutes.Property.TopService, data: p });
	}

    function SendMessageToAgent(data) {
        return $http({ method: 'POST', url: mogiRoutes.Property.SendMessageToBroker, data: data });
    }
	function UpdateFavorite(v) {
        var el = $('#favorite-total');
        var o = parseInt(el.html(), 0);
        o += v; o = Math.max(0, o);
        el.html(o);
        var el2 = $('#favorite-icon');
        el2.toggleClass('favorited', (o > 0));
    }
    function SearchMap(data) {
        return $http({ method: 'POST', url: mogiRoutes.Property.MapViewData, data: data });
    }
	function GetFavorites(ids) {
		UpdateFavorite(1);
		return $http({ method: 'POST', url: mogiRoutes.Common.Favorite_GetList, data: ids });
	}
    function RemoveFavorite(propertyId) {
        UpdateFavorite(-1);
        return $http({ method: 'POST', url: mogiRoutes.Common.Favorite_Remove + "?propertyId=" + propertyId });
    }
    function AddFavorite(propertyId) {
        UpdateFavorite(1);
        return $http({ method: 'POST', url: mogiRoutes.Common.Favorite_Add + "?propertyId=" + propertyId });
    }
    function GetMessages() {
        return $http({ method: 'GET', url: mogiRoutes.Property.ReportMessages });
    }
    function SendReport(data) {
        return $http({ method: 'POST', url: mogiRoutes.Property.ReportAbuse, data: data });
    }
    function MarketGetByDistrict(districtId) {
        return $http({ method: 'GET', url: mogiRoutes.Property.MarketGetByDistrict+'?districtId='+ districtId });
    }
    function GetBankInterestRates() {
        return $http({ method: 'GET', url: mogiRoutes.Property.GetBankInterestRates });
    }
    function GetPropertyByStreet(streetId,p) {
        return $http({ method: 'GET', url: mogiRoutes.Property.GetPropertyByStreet + '?streetId=' + streetId + '&p='+ p });
    }

    function GetPropertyByUniqueAddress(uniqueAddress) {
        return $http({ method: 'GET', url: mogiRoutes.Property.GetPropertyByUniqueAddress + '?uniqueAddress=' + uniqueAddress });
    }

    function GetHistoryPropertyByStreet(streetId, fromArea, toArea) {
        return $http({ method: 'GET', url: mogiRoutes.HousePrice.GetHistoryPropertyByStreet + '?streetId=' + streetId + '&fromArea=' + fromArea + '&toArea=' + toArea });

    }
}
propertyServices.$inject = ["$http"];
mogiApp.factory('propertyServices', propertyServices);