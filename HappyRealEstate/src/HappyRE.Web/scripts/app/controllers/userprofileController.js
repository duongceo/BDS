mogiApp.controller("userProfileController", ['$scope', 'commonServices', 'profileServices', 'imageServices', 'blockUI', 'propertyServices',
	function ($scope, commonServices, profileServices, imageServices, blockUI, propertyServices) {
    var path = angular.lowercase(window.location.pathname);
    $scope.menu = {
        mogipro: { active: false, visible: true },
        lertsearch: { active: false, visible: true },
        favorite: { active: false, visible: true },
        info: { active: false, visible: true },
        changepassword: { active: false, visible: true },
    };

    $scope.Email = {
        HasEmail: false,
        IsVerifiedEmail: false
    };

    $scope.alert = { msg: '', type: '' };
    $scope.Avatar = '/content/Images/no-avatar.jpg';

		$scope.initProfile = function () {
			profileServices.getProfile().then(function (rp) {
				rp = rp.data;
				if (rp.Status) {
					var d = rp.Data;
					$scope.Profile = d;
					$scope.Email.HasEmail = (d.Email != '' && d.Email != null);
					$scope.Email.IsVerifiedEmail = $scope.Profile.IsVerifiedEmail;
					if ((d.LogoSquare || '') != '') $scope.Avatar = d.LogoSquare;
					if (d.CityId == 0) d.CityId = null;
					if (d.LoginByOpenId) $scope.menu.changepassword.visible = false;
				} else {
					mogiUtils.showAlert(rp.Message, 'error');
				}
			});
		};

		$scope.updateProfile = function () {
			if ($scope.myForm.$valid == false) {
				return;
			}
			blockUI.start();
			profileServices.updateProfile($scope.Profile).then(function (rp) {
				rp = rp.data;
				blockUI.stop();
				mogiUtils.showAlert(rp.Message, rp.Status ? '' : 'error');
				if (rp.Data.returnUrl != '' && rp.Data.returnUrl != null) window.location = rp.Data.returnUrl;
			});
		};

    $scope.ChangePassword = {
        msg_err: '',
        initData: function (s) {
            $scope.ChangePassword.msg_err = s;
        },
        updatePassword: function () {
            if ($scope.myForm.$valid == false) {
                return;
            }
            if ($scope.Profile.Password != $scope.Profile.ConfirmPassword) {
                mogiUtils.showAlert($scope.ChangePassword.msg_err, 'error');
                return;
            }
            blockUI.start();
			profileServices.updatePassword($scope.Profile).then(function (rp) {
				rp = rp.data;
                blockUI.stop();
                if (!rp.Status) {
                    mogiUtils.showAlert(rp.Message, 'error');
                }
                else {
                    mogiUtils.boxAlert(rp.Message, function () {
                        setTimeout(function () { window.location = '/account/logoff'; }, 5000);
                    });
                }
            });
        }
    };

		$scope.sendVerifyEmail = function () {
			if ($scope.myForm.$valid == false) {
				return;
			}
			profileServices.sendVerifyEmail().then(function (rp) {
				rp = rp.data;
				$scope.alert.msg = rp.Message;
				if (!rp.Status) mogiUtils.showAlert(rp.Message, 'error');
				else mogiUtils.showAlert(rp.Message, '');
			});
		};
		$scope.updateEmail = function () {
			if ($scope.myForm.$valid == false) {
				return;
			}
			profileServices.updateEmail($scope.Profile).then(function (rp) {
				rp = rp.data;
				if (!rp.Status) mogiUtils.showAlert(rp.Message, 'error');
				else {
					mogiUtils.showAlert(rp.Message, '');
					window.location = '/trang-ca-nhan/tim-kiem-da-luu';
				}
			});
		};

    //-----Avartar--------------
    $scope.$watch('FileAvatar', function (files) {
        $scope.formUpload = false;
        if ((files || null) != null) {
            $scope.UploadAvatarImage(files, true);
        }
    });

    $scope.UploadAvatarImage = function (file) {
        $scope.alert.msg = '';
        imageServices.UploadImage(file, mogiConst.ReferType.account_square_logo, function (res) {
            $scope.Avatar = res;
            $scope.Profile.LogoSquare = $scope.Avatar.AvatarUrl();
            $scope.Profile.AvatarMediumUrl = $scope.Avatar.AvatarUrl();
        }, function (err) {
            mogiUtils.showAlert(err, 'error');
        });
    };

    // Đăng ký Mogi Pro
    $scope.MogiPro = {
        CityOptions: [],
        DistrictOptions: [],
        initCity: function () {
            $scope.MogiPro.CityOptions = mogiResUtils.GetCities();
        },
        changeCity: function (cityId) {
            $scope.MogiPro.DistrictOptions = [];
            var objCity = mogiResUtils.GetCityById(cityId);
            if (objCity != null) {
                $scope.MogiPro.DistrictOptions = objCity.c;
            }
        },
        registerMogiPro: function () {
            if ($scope.myForm.$valid == false) {
                return;
            }
            blockUI.start();
			profileServices.registerMogiPro($scope.Profile).then(function (rp) {
				rp = rp.data;
                blockUI.stop();
                mogiUtils.showAlert(rp.Message, rp.Status ? '' : 'error');
                if (rp.Data.returnUrl != '' && rp.Data.returnUrl != null) window.location = rp.Data.returnUrl;
            });
        }
    };


    $scope.Search = {
        Recents: [],
        Data: [],
        IsRent: false,
        Counter: { buy: 0, forent: 0 },
        Query: { ForRent: false, PageIndex: 1, PageSize: 10 },
        IsAlert: function (item) {
            return (item.ReceiveEmailType != mogiConst.ReceiveEmailType.None);
        },
        ShowRecent: function () { return (this.Recents && this.Recents.length > 0); },
        ShowAlertPopup: function (data, isRecent, index) {
            commonServices.AlertSearch(data, function (resp) {
                if (resp) {
                    $scope.Search.GetAlertSearchs($scope.Search.IsRent);
                    if (isRecent) {
                        $scope.Search.Recents.splice(index, 1);
                    }
                }
            });
        },
        GetAlertSearchs: function (v) {
            $scope.Search.IsRent = v;
            $scope.Search.Query.ForRent = v;
			profileServices.getAlertSearchs($scope.Search.Query).then(function (rp) {
				rp = rp.data;
                if (rp.Status) {
                    $scope.Search.Data = rp.Data.Data;
                    $scope.Search.Counter = { buy: rp.Data.TotalBuy, forent: rp.Data.TotalForent }
                } else {
                    mogiUtils.showAlert(rp.Message, 'error');
                }
            });
        },
        GetRecents: function () {
			profileServices.getRecentSearchs().then(function (rp) {
				rp = rp.data;
                if (rp.Status) { $scope.Search.Recents = rp.Data; }
            });
        },
        ClearRecent: function () {
			profileServices.clearRecentSearch().then(function (rp) {
				rp = rp.data;
                if (!rp.Status) {
                    mogiUtils.showAlert(rp.Message, 'error');
                } else {
                    $scope.Search.Recents = [];
                }
            });
        },
        SaveRecent: function (model, index) {
			profileServices.saveRecentSearch(model, mogiConst.ReceiveEmailType.None).then(function (rp) {
				rp = rp.data;
                if (!rp.Status) {
                    mogiUtils.showAlert(rp.Message, 'error');
                } else {
                    $scope.Search.GetAlertSearchs($scope.Search.IsRent);
                    $scope.Search.Recents.splice(index, 1);
                }
            });
        },
        Delete: function (id, index) {
			profileServices.deleteAlertSearch(id).then(function (rp) {
				rp = rp.data;
                if (rp.Status) {
                    $scope.Search.GetAlertSearchs($scope.Search.IsRent);
                } else {
                    mogiUtils.showAlert(rp.Message, 'error');
                }
            });
        },
        CallSearch: function (f) {
            f = angular.fromJson(f);
            var p = mogiUtils.friendlyUrl.mapSearchFilter(f);
            window.location.href = mogiUtils.friendlyUrl.getPropertySearchUrl(p);
        }
    };

    // Favorite List
    $scope.BuyItems = 0;
    $scope.RentItems = 0;
    $scope.IsRent = null;
    $scope.ToggleTab = function (v) {
        $scope.IsRent = v;
    };

    $scope.RemoveFavorite = function (id, isRent) {
		propertyServices.RemoveFavorite(id).then(function () {
			angular.element('#' + id).fadeOut(200, function () { angular.element(this).remove(); });
			if (isRent) {
				$scope.RentItems--;
			} else {
				$scope.BuyItems--;
			}
		});
    };

    $scope.activeTab = function (buyItems, rentItems) {
        var url = $.url();
        var isTabActive = (Object.keys(url.fparam()).length > 0);

        $scope.IsRent = (buyItems == 0 && rentItems > 0);
        if ($scope.IsRent) {
            $('.summary-tab a[href="#rent-list"]').trigger('click');
        }
        $scope.initProfile();
    };

    // Un-roll search alert
		$scope.UnrollAlert = (function () {

			return {
				Confirm: function () {
					var u = $.url();
					//pid, string aid, string sig
					var payload = {
						pid: u.hparam('pid'),
						aid: u.hparam('aid'),
						sig: u.hparam('sig'),
						t: $scope.UnrollAlert.ReceiveEmailType
					};
					profileServices.UnrollAlert(payload).then(function (rp) {
						location.href = "http://batdongsanhanhphuc.vn";
					});
				},
				Cancel: function () { location.href = "http://batdongsanhanhphuc.vn"; },
				Data: mogiResUtils.syscodeUtils.getAlertType(),
				ReceiveEmailType: (typeof alertType === "undefined") ? "" : alertType,

			};
		}());

    $scope.UserMessage = (function () {
        var isAuth = false;
        var data = [];
        var key = 'mogimsg';
        var messages = [];
        var saveLocalData = function (key, data) {
            if (typeof (Storage) !== "undefined") {
                localStorage.setItem(key, data);
            } else {
                console.log('Not support local storare');
            }
        };

        var getLocalData = function (key) {
            if (typeof (Storage) !== "undefined") {
                return localStorage[key];

            } else {
                console.log('Not support local storare');
                return void (0);
            }
        };

		var getMessages = function (p) {
			profileServices.GetMessages(p).then(function (rp) {
				rp = rp.data;
				$scope.UserMessage.Messages = rp.Data.Items;
				$scope.UserMessage.Total = rp.Data.Total;
			});

		};
        return {
            Init: function (model, isAuth) {
                data = model;
                isAuth = isAuth;
                getMessages(1);
            },
            Data: function () {
                return data;
            },
            IsReadAll: function () {
                var localData = getLocalData(key);
                if (localData == null) {
                    return false;
                }
                localData = JSON.parse(localData);
                var readItems = data.filter(function (item) {
                    return localData.msg[item.MessageId] === true;
                });
                debugger;
                return readItems.length === data.length;
            },
            OnRead: function (id) {
                console.log(data)

                var localData = getLocalData(key);
                if (localData != null) {
                    localData = JSON.parse(localData);
                    if (localData.msg[id]) {
                        return;
                    }
                    localData.msg[id] = true;
                    localData = JSON.stringify(localData);
                    saveLocalData(key, localData);
                } else {
                    localData = {};
                    localData.msg = {};
                    localData.msg[id] = true;
                    localData = JSON.stringify(localData);

                    saveLocalData(key, localData);
                }
            },
            GetMessages: function (p) {
                getMessages($scope.UserMessage.PageIndex);
            },
            Total: 0,
            Messages: [],
            PageIndex:1
        }
    }());

}]);