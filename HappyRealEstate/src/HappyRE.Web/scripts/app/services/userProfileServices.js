mogiApp.service('profileServices', ['$http', function ($http) {
	this.login = function (model) {
		return $http({ method: 'POST', url: mogiRoutes.Profile.login, data: { model: model } });
	};
	this.updateUser = function (profile) {
        return $http({ method: 'POST', url: mogiRoutes.Profile.updateUser, data: { model: profile } });
    };
    this.verifiedOTP = function (otp) {
		return $http({ method: 'POST', url: mogiRoutes.Profile.verifiedOTP, data: { model: otp } });
    };
    this.accountKitVerifySms = function (model) {
        return $http({ method: 'POST', url: mogiRoutes.Profile.accountKitVerifySms, data: model });
    };
    this.sendVerifyEmail = function () {
        return $http({ method: 'POST', url: mogiRoutes.Profile.sendVerifyEmail });
    };
    this.updateEmail = function (profile) {
        return $http({ method: 'POST', url: mogiRoutes.Profile.updateEmail, data: { model: profile } });
    };
    this.getProfile = function () {
        return $http({ method: 'POST', url: mogiRoutes.Profile.getProfile });
    };
    this.getUserData = function () {
        return $http({ method: 'POST', url: mogiRoutes.Profile.getUserData });
    };
    this.updateProfile = function (profile) {
        return $http({ method: 'POST', url: mogiRoutes.Profile.updateProfile, data: { model: profile } });
    };
    this.registerMogiPro = function (profile) {
        return $http({ method: 'POST', url: mogiRoutes.Profile.registerMogiPro, data: { model: profile } });
    };

    this.ackLoginSuccess = function (ackData) {
        return $http({ method: 'POST', url: mogiRoutes.Profile.ackLoginSuccess, data: ackData });
    };
	this.ackLoginFailed = function (mobile, command) {
		command = command || '';
		return $http({ method: 'POST', url: mogiRoutes.Profile.ackLoginFailed, data: { 'mobile': mobile, 'command': command } });
    };
	this.resetPassword = function (p) {
		return $http({ method: 'POST', url: mogiRoutes.Profile.resetPassword, data: { model: p } });
	};
    this.updatePassword = function (profile) {
        return $http({ method: 'POST', url: mogiRoutes.Profile.updatePassword, data: { model: profile } });
    };
    this.getAlertSearchs = function (query) {
        return $http({ method: 'POST', url: mogiRoutes.Profile.getAlertSearchs, data: { query: query } });
    };
    this.deleteAlertSearch = function (id) {
        return $http({ method: 'POST', url: mogiRoutes.Profile.deleteAlertSearch, data: { alertSearchId: id } });
    };
    this.updateAlertSearch = function (data) {
        return $http({ method: 'POST', url: mogiRoutes.Profile.updateAlertSearch, data: { data: data } });
    };
    //this.getReceiveEmailType = function () {
    //    return $http({ method: 'GET', url: mogiRoutes.Profile.getReceiveEmailType });
    //};
    this.getRecentSearchs = function () {
        return $http({ method: 'GET', url: mogiRoutes.Profile.getRecentSearchs });
    };
    this.getAlertSearchLatest = function () {
        return $http({ method: 'GET', url: mogiRoutes.Profile.getAlertSearchLatest });
    };
    this.saveRecentSearch = function (recentSearch, receiveEmailType) {
        return $http({ method: 'POST', url: mogiRoutes.Profile.saveRecentSearch, data: { model: recentSearch, ReceiveEmailType: receiveEmailType } });
    };
    this.clearRecentSearch = function () {
        return $http({ method: 'POST', url: mogiRoutes.Profile.clearRecentSearch });
    };

    this.UnrollAlert = function (data) {
        return $http({ method: 'POST', url: mogiRoutes.Profile.UnrollAlert, data: data });
    };

    this.GetMessages = function (p) {
        return $http({ method: 'GET', url: mogiRoutes.Profile.GetMessages + '?p=' + p });
    };

    this.validateMobile = function (mobile) {
        return $http({ method: 'POST', url: mogiRoutes.Profile.validateMobile ,data: { 'mobile': mobile } });
    };

    this.checkReferal = function (data) {
        return $http({ method: 'POST', url: mogiRoutes.Profile.checkReferal, data: data });
	};

	this.getZoneByCityId = function (cityId) {
		return $http({ method: 'GET', url: mogiRoutes.City.GetZoneByCityId + '?cityId=' + cityId });
	};

	this.sendSMS = function (data) {
		return $http({ method: 'POST', url: mogiRoutes.Profile.sendSMS, data: data });
	};
}]);