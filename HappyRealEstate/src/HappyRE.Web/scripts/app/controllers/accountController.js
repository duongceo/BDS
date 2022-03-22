mogiApp.controller('accountController', ['$scope', '$location', 'profileServices', 'blockUI', function ($scope, $location, profileServices, blockUI) {
	$scope.step = 'login';
	$scope.isLoginOrRegister = function () { return $scope.step === 'login' || $scope.step === 'register'; };
	$scope.isLogin = function () { return $scope.step === 'login'; };
	$scope.isRegister = function () { return $scope.step === 'register'; };
	$scope.isUpdate = function () { return $scope.step === 'update'; };
	$scope.isFacebook = function () { return $scope.step === 'facebook'; };
	$scope.isOTP = function () { return $scope.step === 'otp'; };
	$scope.Profile = {
		CityId: -1,
		FirstName: '',
		LastName: '',
		Mobile: '',
		Email: '',
		Password: '',
		ConfirmPassword: '',
		RegisterMogiPro: false,
		LevelId: 1,
		ZoneId: 0,
		Avatar: '',
		IDCard1: '',
		IDCard2: '',
		Code: '',
		Birthday: null,
		IDCard: null,
		UserTypeId: 0,
		Referal: {
			lid: null,
			lg: null
		}
	};
	$scope.OTP = function () {
		var p = $scope.Profile;
		return { Mobile: p.Mobile, FirstName: p.FirstName, LastName: p.LastName, Password: p.Password, ZoneId: 0, LevelId: 1, UserTypeId: 0, Code: p.Code, GoogleCaptchaResponse: captcha_resp, Command: '' };
	};
	$scope.ReturnUrl = '/';
	$scope.OtpMsg = "";
	$scope.RegisterResult = { Title: '', Msg: '', ReturnUrl: '/' };
	$scope.showError = function (msg) { mogiUtils.showAlert(msg, 'error'); };
	$scope.focusOTP = function () {
		try { document.getElementById("Code").focus(); } catch (e) { console.log(e); }
	};

	$scope.Facebook = {
		counter: 0,
		max: 1,
		command: '',
		loginCallback: null,
		login: function (cb) {
			var m = $scope.Profile.Mobile;
			if (m === null || m === '') return;
			this.loginCallback = cb;
			this.counter += 1;
			AccountKit.login('PHONE', { countryCode: mogiConst.SmsCountryCode.Vn, phoneNumber: m }, this.callback);
		},
		callback: function (resp) {
			var mobile = $scope.Profile.Mobile;
			if (resp.status === "PARTIALLY_AUTHENTICATED") {
				if ($scope.Facebook.command === 'resetpassword') {
					$scope.Facebook.loginCallback.facebookSuccess(resp);
					return;
				}
				var p = $scope.Profile;
				$scope.AckResponse = { Code: resp.code, Csrf_Nonce: resp.state, Mobile: mobile, FirstName: p.FirstName, LastName: p.LastName, Password: p.Password };
				blockUI.start();
				profileServices.ackLoginSuccess($scope.AckResponse).then(function (resp) {
					blockUI.stop();
					resp.Message = mogiDatas.Msg.GeneralError;
					if (resp.status === 200) {
						resp = resp.data;
						if (resp.Status) {
							$scope.Facebook.loginCallback.facebookSuccess(resp);
							return;
						}
					}
					$scope.showError(resp.Message);
				});
			} else if (resp.status === "NOT_AUTHENTICATED") {
				blockUI.start();
				profileServices.ackLoginFailed(mobile,$scope.Facebook.command).then(function (resp) {
					blockUI.stop();
					resp.Message = mogiDatas.Msg.GeneralError;
					if (resp.status === 200) {
						resp = resp.data;
						if (resp.Status) {
							$scope.Facebook.loginCallback.facebookError(resp);
							return;
						}
					}
					$scope.showError(resp.Message);
				});
			}
			// else if (resp.status === "BAD_PARAMS") { }
		},
		allow: function () { return this.counter < this.max; }
	};
	window.Facebook = $scope.Facebook;

	$scope.login = function () {
		var p = $scope.Profile;
		if (p.Mobile === '' || p.Password === '') {
			return;
		}
		if (captchaValidate('sign-in') === false) {
			return false;
		}
		var model = { Mobile: p.Mobile, Password: p.Password, GoogleCaptchaResponse: captcha_resp, ReturnUrl: pageData.ReturnUrl };
		blockUI.start();
		profileServices.login(model).then(function (resp) {
			blockUI.stop();
			resp.Message = mogiDatas.Msg.GeneralError;
			if (resp.status === 200) {
				resp = resp.data;
				if (resp.Status) {
					window.location = resp.Data;
					return;
				}
			}
			$scope.showError(resp.Message);
		});
	};
	$scope.changeStep = function (step) {
		$scope.step = step;
		$scope.myForm.$submitted = false;
		setTimeout(function () { document.getElementById("Mobile").focus(); }, 100);
	};

    $scope.Register = {
		OTP: false,
		timeoutId: null,
		lastMobile: '',
		eye: false,
		initModel: function (model) {
			$m.map(model, $scope.Profile);
			$scope.Profile.RegisterMogiPro = true;
        },
		checkReferal: function () {
			var p = $location.search();
			p.leadid = p.leadid || '';
			p.leadkey = p.leadkey || '';
			if (p.leadkey && p.leadkey.length > 0 && p.leadid && p.leadid.length > 0) {
				profileServices.checkReferal(p).then(function (resp) {
					blockUI.stop();
					resp.Message = mogiDatas.Msg.GeneralError;
					if (resp.status === 200) {
						resp = resp.data;
						if (resp.Status) {
							$scope.Profile.Mobile = resp.Mobile;
							$scope.Profile.LastName = resp.FullName.split(' ').slice(0, -1).join(' ');
							$scope.Profile.FirstName = resp.FullName.split(' ').slice(-1).join(' ');
							$scope.Profile.Referal.lid = lid;
							$scope.Profile.Referal.lg = lg;
							return;
						}
					}
					$scope.showError(resp.Message);
				});
			}
        },
		submit: function () {
			// sign-in
			if ($scope.isLogin() === true) {
				$scope.login();
				return;
			}

			// sign-up
            if ($scope.myForm.$valid === false) {
                return;
			}
			if (captchaValidate('sign-up') === false) {
				return false;
			}

			var p = $scope.Profile;
			var model = { Mobile: p.Mobile, GoogleCaptchaResponse: captcha_resp, command: 'register' };
			blockUI.start();
			profileServices.sendSMS(model).then(function (resp) {
				blockUI.stop();
				resp.Message = mogiDatas.Msg.GeneralError;
				if (resp.status === 200) {
					resp = resp.data;
					if (resp.Status) {
						$scope.step = 'otp';
						$scope.OtpMsg = resp.Message;
						$scope.Register.OTP = true;
						$scope.focusOTP();
						return;
					}
				}
				$scope.showError(resp.Message);
			});
		},
		updateMobile: function (fid) {
			fid = fid || 'updateForm';
			if ($scope[fid].$valid === false) {
				return;
			}
			this.facebookLogin();
		},
		verifyMobile: function (fid) {
			fid = fid || 'otpForm';
            if ($scope[fid].$valid === false) {
                return;
			}
			if (captchaValidate('verified-otp') === false) {
                return false;
            }

            blockUI.start();
            profileServices.verifiedOTP($scope.OTP()).then(function (resp) {
				blockUI.stop();
				resp.Message = mogiDatas.Msg.GeneralError;
				if (resp.status === 200) {
					resp = resp.data;
					if (resp.Status) {
						window.location = mogiRoutes.Profile.MogiProRegister;
						return;
					}
				}
				$scope.showError(resp.Message);
				$scope.focusOTP();
            });
		},
		validateMobile: function () {
			mobile = $scope.Profile.Mobile || '';
			if (mobile === '' || this.lastMobile === mobile) return;
			if (this.timeoutId) {
				clearTimeout(this.timeoutId);
				this.timeoutId = null;
			}
			this.timeoutId = setTimeout(function () {
				if ($scope.isRegister() === false) return;
				profileServices.validateMobile(mobile).then(function (resp) {
					if (resp.status === 200 && resp.data.Status === false) {
						if ($scope.isRegister()) {
							$scope.Register.lastMobile = mobile;
							$scope.showError(resp.data.Message);
						}
					}
				});
			}, 100);
		},
		togglePass: function () {
			this.eye = !this.eye;
			var t = this.eye ? 'text' : 'password';
			document.getElementById('Password').setAttribute('type',t);
		},
		openId: function (l) {
			return ($scope.isLogin() ? pageData.openId.SignIn : pageData.openId.SignUp) + ' ' + l;
		}
	};

	$scope.MogiPro = {
		step: 'register',
		Zones: [],
		isWelcome: function () { return this.step === 'welcome'; },
		getZones: function () {
			profileServices.getZoneByCityId(0).then(function (rp) {
				$scope.MogiPro.Zones = rp.data.Data || [];
			});
		},
		confirm: function () {
			$scope.MogiPro.step = 'mogipro';
		},
		ignore: function () { document.location = '/'; },
		register: function () {
			var f = $scope.myForm2;
			if (f.$valid === false) return;
			if ($scope.Profile.ZoneId <= 0) {
				f.ZoneId.$touched = true;
				f.ZoneId.$error.required = true;
				return;
			}

			blockUI.start();
			profileServices.registerMogiPro($scope.Profile).then(function (rp) {
				rp = rp.data;
				blockUI.stop();
				if (rp.Status) {
					$m.map(rp.Data, $scope.RegisterResult);
					$scope.MogiPro.step = 'welcome';
					window.setTimeout(function () { $scope.MogiPro.done(); }, 10000);
				} else {
					$scope.showError(rp.Message);
				}
			});
		},
		done: function () {
			window.location = $scope.RegisterResult.ReturnUrl || '/';
		},
		init: function () {
			this.getZones();
		}
	};

	$scope.Forgot = {
		step: 'input',
		eye: false,
		message: '',
		loginUrl: mogiRoutes.loginUrl,
		stepInput: function () { return this.step === 'input'; },
		stepOTP: function () { return this.step === 'otp'; },
		stepSuccess: function () { return this.step === 'success'; },
		submit: function () {
			if ($scope.myForm.$valid === false) {
				return;
			}

			if (captchaValidate('btn-submit') === false) {
				return false;
			}

			var p = $scope.Profile;
			var model = { Mobile: p.Mobile, GoogleCaptchaResponse: captcha_resp, command: 'resetpassword' };
			blockUI.start();
			profileServices.sendSMS(model).then(function (resp) {
				blockUI.stop();
				resp.Message = mogiDatas.Msg.GeneralError;
				if (resp.status === 200) {
					resp = resp.data;
					if (resp.Status) {
						$scope.Forgot.step = 'otp';
						$scope.OtpMsg = resp.Message;
						$scope.focusOTP();
						return;
					}
				}
				$scope.showError(resp.Message);
			});
		},
		validateOTP: function () {
			if ($scope.otpForm.$valid === false) {
				return;
			}
			if (captchaValidate('verified-otp') === false) {
				return false;
			}

			var data = $scope.OTP();
			data.Facebook = false;
			this.resetPassword(data);
		},
		resetPassword: function (data) {
			blockUI.start();
			profileServices.resetPassword(data).then(function (resp) {
				blockUI.stop();
				resp.Message = mogiDatas.Msg.GeneralError;
				if (resp.status === 200) {
					resp = resp.data;
					if (resp.Status) {
						$scope.Forgot.step = 'success';
						$scope.Forgot.message = resp.Message;
						setTimeout(function () { window.location = mogiRoutes.loginUrl; }, 5000);
						return;
					}
				}
				$scope.showError(resp.Message);
				$scope.focusOTP();
			});
		},
		togglePass: function () {
			this.eye = !this.eye;
			var t = this.eye ? 'text' : 'password';
			document.getElementById('Password').setAttribute('type', t);
		}
	};
	
	function initData() {
		pageData = window.pageData || {};
		if ($m.defined(pageData.Profile)) {
			$m.map(pageData.Profile, $scope.Profile);
		}
		if (pageData.step) $scope.step = pageData.step;
		$scope.Profile.Mobile = pageData.Mobile;

		var po = document.createElement('script'); po.type = 'text/javascript'; po.async = true;
		po.src = 'https://zjs.zdn.vn/zalo/sdk.js';
		var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);
	}
	initData();
}]);