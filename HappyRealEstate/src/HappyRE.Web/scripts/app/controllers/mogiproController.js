mogiApp.controller('mogiproController', ['$scope', 'profileServices', 'cmsServices', 'cityServices', 'blockUI', function ($scope, profileServices, cmsServices, cityServices, blockUI) {
    $scope.step = 'register';
    $scope.Profile = {
        CityId: 0,
        DistrictId: 0,
        FistName: '',
        LastName: '',
        Mobile: '',
        Email: '',
        Password: '',
        ConfirmPassword: '',
        RegisterMogiPro: false,
        UserType: 0,
        ZoneId: null,
        Code: ''
    };

	$scope.OTP = { Mobile: '', Code: '' };

    $scope.ReturnUrl = '/';
    $scope.OtpMsg = "";
    $scope.RegisterResult = {
        Title: '',
        Msg: '',
        ReturnUrl: '/'
    };

    $scope.alert = {
        msg: '',
        type: ''
    };
    $scope.reset = function () {
        $scope.myForm.$setPristine();
        $scope.myForm.$setUntouched();
    };

    // login callback
    function loginCallback(response) {
        var mobile = $('#Mobile').val();
        if (response.status === "PARTIALLY_AUTHENTICATED") {
			$scope.AckResponse = {
				Code: response.code,
				Csrf_Nonce: response.state,
				Mobile: mobile,
				Profile: $scope.Profile
			};
            blockUI.start();
			profileServices.ackLoginSuccess($scope.AckResponse).then(function (rp) {
				rp = rp.data;
				blockUI.stop();
				if (rp.Status == false) {
					$scope.alert.type = 'msg-error';
					$scope.alert.msg = rp.Message;
					mogiUtils.showAlert(rp.Message, 'error');
				}
				else {
					$scope.RegisterResult = rp.Data.result;
					$scope.MogiPro.Step = 'mogipro';
				}
			});
        }
        else if (response.status === "NOT_AUTHENTICATED") {   
            blockUI.start();
			profileServices.ackLoginFailed(mobile).then(function (rp) {
				rp = rp.data;
                blockUI.stop();
                if (rp.Status) {                         
                    $scope.OTP.Mobile = mobile;
                    $scope.OTP.Code = '';
                    $scope.OtpMsg = rp.Message;
                    $scope.MogiPro.Step = 'otp';
                    $scope.MogiPro.OTP = true;
                    window.setTimeout(function () { $('#Code').focus(); }, 1000);
                } else {
                    mogiUtils.showAlert(rp.Message, 'error');                            
                }
            });
        }
        else if (response.status === "BAD_PARAMS") {
            // handle bad parameters
        }
    }

    // phone form submission handler
    function smsFacebookLogin() {
        var countryCode = mogiConst.SmsCountryCode.Vn;
        var phoneNumber = document.getElementById("Mobile").value;
        if (phoneNumber == '' || phoneNumber == null) return;
        AccountKit.login(
          'PHONE',
          { countryCode: countryCode, phoneNumber: phoneNumber },
          loginCallback
        );
    }

    // Đăng ký Mogi Pro
    $scope.MogiPro = {
        Step: 'register',
        OTP: false,
        Agreement: '',
        CityOptions: [],
        UserZoneOptions: [],
        initProfile: function () {
			$scope.Profile = pageData.Profile;
			$scope.OTP.Mobile = $scope.Profile.Mobile;
        },
        initCity: function () {
            $scope.MogiPro.CityOptions = mogiResUtils.GetCities();
        },
        changeCity: function () {
			$scope.MogiPro.UserZoneOptions = [];
			cityServices.GetZoneByCity(0).then(function (rp) {
				rp = rp.data;
                if (rp.Data !== null) {
                    $scope.MogiPro.UserZoneOptions = rp.Data;
                }
            });
        },
		getMobile: function () { return $scope.Profile.Mobile || '';},
		confirm: function () {
			$scope.MogiPro.Step = this.getMobile() === '' ? 'info' : 'mogipro';
        },
		ignore: function () { document.location = '/'; },
        update: function () {
            if ($scope.myForm.$valid === false) {
                return;
            }
            if (this.OTP === false) {
                smsFacebookLogin();
                return;
            }
        },
        register: function () {
            if ($scope.myForm2.$valid == false) {
                return;
            }
            blockUI.start();
			profileServices.registerMogiPro($scope.Profile).then(function (rp) {
				rp = rp.data;
                blockUI.stop();
                if (rp.Status) {
                    var msg = '<div class="popup-dialog"><p class="icon_verify"><span class="icon icon-verified"></span></p><div class="content"><h4 class="">' + rp.Data.Title + '</h4><p style="padding-top:10px;">' + rp.Data.Msg + '</p></p></div>';
                    mogiUtils.boxSucess(msg, function () {
                        window.location = rp.Data.ReturnUrl;
                    });
                } else {
                    mogiUtils.showAlert(rp.Message, 'error');
                }
            });
        },
        verifyMobile: function () {
            if ($scope.myForm1.$valid === false) {
                return;
            }
            captcha_resp = grecaptcha.getResponse(captcha_id);
            if (captcha_resp === '') {
                grecaptcha.execute(captcha_id);
                return false;
            }
            grecaptcha.reset(captcha_id);

            $scope.OTP.captchaGuid = $scope.Captcha.GId;
            $scope.OTP.captchaConfirm = $scope.Captcha.Value;
            $scope.OTP.Code = $scope.Profile.Code;
            $scope.OTP.GoogleCaptchaResponse = captcha_resp;
            $scope.OTP.Profile = $scope.Profile;

            blockUI.start();
			profileServices.verifiedOTP($scope.OTP).then(function (rp) {
				rp = rp.data;
                blockUI.stop();
                if (!rp.Status) {
                    mogiUtils.showAlert(rp.Message, 'error');
                }

                if (rp.Status) {
                    $scope.RegisterResult = rp.Data.result;
                    $scope.MogiPro.Step = 'mogipro';
                }
            });
        }
	};
}]);