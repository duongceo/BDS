mogiApp.service('popupAlert', ['$uibModal', '$http', function($uibModal, $http) {
    this.open = function(data,isRecent, callback) {
        var modalInstance = $uibModal.open({
            templateUrl: '/scripts/app/templates/AlertSearchStatus.html',
            controller: ['$scope','$filter', '$uibModalInstance','profileServices' ,'obj','isRecent', function($scope,$filter, $uibModalInstance,profileServices, obj,isRecent) {
                profileServices.getReceiveEmailType().success(function (rp) {
                    if (rp.Status) {
                        $scope.receiveEmailTypes = rp.Data;
                    }
                });

                if (isRecent) {
                    $scope.alertSearch = obj;
                    $scope.alertSearch.ReceiveEmailType = mogiConst.ReceiveEmailType.None;
                } else {
                    $scope.alertSearch = { AlertSearchId: obj.AlertSearchId, Title: obj.Title, ReceiveEmailType: obj.ReceiveEmailType };
                }
                    
                $scope.close = function () {
                    $uibModalInstance.dismiss('cancel');
                };
                $scope.update = function () {
                    if (isRecent) {
                        profileServices.saveRecentSearch($scope.alertSearch, $scope.alertSearch.ReceiveEmailType).success(function (rp) {
                            if (!rp.Status) {
                            } else {
                                $uibModalInstance.close();
                            }
                        });
                       
                    } else {
                        profileServices.updateAlertSearch($scope.alertSearch)
                        .success(function (rp) {
                            if (rp.Status) {
                                obj.ReceiveEmailType = rp.Data.ReceiveEmailType;
                                obj.ReceiveEmailTypeName = $filter('filter')($scope.receiveEmailTypes, function (d) { return d.SysCodeId == $scope.alertSearch.ReceiveEmailType })[0].Name;
                                $uibModalInstance.close();
                            }
                        });
                    }                    
                };
            }],
            backdrop: 'static',
            resolve: {
                obj: function() {
                    return data;
                },
                isRecent: function() {
                    return isRecent;
                }
            }
        });
 
        modalInstance.result.then(function() {
            return callback(true);
        }, function() {
            return callback(false);
        });
    };
    }]);
 