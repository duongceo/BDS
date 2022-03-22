function commonServices($http, $uibModal, profileServices) {
    this.AlertSearch = function (data, callback) {
		var modalInstance = $uibModal.open({
			templateUrl: '/scripts/app/templates/alertsearchstatus.html',
			controller: ['$scope', '$filter', '$uibModalInstance', 'profileServices', 'obj',
				function ($scope, $filter, $uibModalInstance, profileServices, obj) {
					$scope.Msg = mogiDatas.Msg;
					$scope.Data = { AlertSearchId: obj.AlertSearchId || 0, Title: obj.Title, ReceiveEmailType: obj.ReceiveEmailType || 136, HashKey: obj.HashKey, Rent: obj.ForRent, json: obj["json"] || null };
					$scope.List = mogiResUtils.syscodeUtils.getAlertType();
					$scope.close = function () { $uibModalInstance.dismiss('cancel'); };
					$scope.update = function () {
						profileServices.updateAlertSearch($scope.Data).then(function (rp) {
							rp = rp.data;
							if (rp.Status) {
								obj.ReceiveEmailType = rp.Data.ReceiveEmailType;
								obj.ReceiveEmailTypeName = mogiResUtils.getSysCode(obj.ReceiveEmailType);
							}
							$uibModalInstance.close($scope.Data);
						});
					};
				}],
			backdrop: 'static',
			resolve: { obj: function () { return data; } }
		});

        modalInstance.result.then(function (data) {
			profileServices.getUserData().then(function (rp) {
				rp = rp.data;
                if (rp.Status) {
                    // Nếu chưa xác thực email => chuyển trang xác thực email
					if (!rp.Data.IsVerifiedEmail && data.ReceiveEmailType !== mogiConst.ReceiveEmailType.None) {
						window.location.assign("/profile/verifyemail");
                        return;
                    }
                }
                return callback(true);
            });
        }, function () { return callback(false); });
	};

	this.SendFeedBack = function (data) {
		return $http({ method: 'POST', url: mogiRoutes.Common.FeedBack, data: data });
	};
}
commonServices.$inject = ['$http', '$uibModal', 'profileServices'];
mogiApp.service('commonServices', commonServices);
