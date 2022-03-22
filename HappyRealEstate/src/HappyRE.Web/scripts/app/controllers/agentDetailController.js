var agentDetailController = function ($scope, propertyServices) {
    $scope.IsRent = null;

    $scope.ToggleTab = function (value) {
        $scope.IsRent = value;
    };

    $scope.Init = function () {
        let saleIds = pageData.SaleIds || [];
        let rentIds = pageData.RentIds || [];
        let ids = saleIds.concat(rentIds);
        $scope.Favorite.init(ids);
    };



    $scope.PhoneFormat = function (num) {
        let phone = num.replace(/(\d{1,100})(\d{3})(\d{3})/, '$1 $2 $3');
        return phone;
    };

    $scope.UserType = function (typeid) {
        var typeName = mogiResUtils.getUserType(typeid);
        return typeName;
    };


};
agentDetailController.$inject = ['$scope', 'agentServices', 'propertyServices', 'suggest'];
mogiApp.controller('agentDetailController', agentDetailController);

// google recaptcha
