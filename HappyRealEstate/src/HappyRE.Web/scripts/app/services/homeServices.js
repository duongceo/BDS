function homeServices($http) {
    return {
        RefreshCaptcha: RefreshCaptcha,
        SendFeedBack: SendFeedBack
    };

    function RefreshCaptcha(guid) {
        return $http({ method: 'POST', url: mogiRoutes.Home.RefreshCaptcha, data: { gid : guid} });
    };

    function SendFeedBack(data) {
        return $http({ method: 'POST', url: mogiRoutes.Home.FeedBack, data: data });
    };
}
homeServices.$inject = ["$http"];
mogiApp.factory('homeServices', homeServices);