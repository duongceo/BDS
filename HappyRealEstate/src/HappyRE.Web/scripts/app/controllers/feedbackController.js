var feedbackController = function ($scope, blockUI, commonServices) {
    $scope.Message = '';
	$scope.SendMessage = function () {
		if ($scope.myForm.$valid === false) {
			return;
		}
		SendMessage();
	};
    $scope.modalShown = false;
    $scope.ToggleModal = function () {
        $scope.modalShown = !$scope.modalShown;
    };

    function SendMessage() {
        blockUI.start();
		var feedback = {
			Email: $scope.Model.Email,
			Mobile: $scope.Model.Mobile,
			Content: $scope.Model.Content,
			Captcha: $captcha.resp
		};
		commonServices.SendFeedBack(feedback).then(function (rp) {
			rp = rp.data;
            blockUI.stop();
            $scope.Message = rp.Message;
            if (rp.Status === true) {
                $scope.ToggleModal();
            } else {
                if ($scope.Message !== null) {
                    $scope.ToggleModal();
                }
            }
        });
    }
};
feedbackController.$inject = ['$scope', 'blockUI', 'commonServices'];
mogiApp.controller('feedbackController', feedbackController);