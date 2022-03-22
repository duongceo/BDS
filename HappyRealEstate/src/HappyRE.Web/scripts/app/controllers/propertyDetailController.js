function propertyDetailController($scope, propertyServices, agentServices, $sce) {
	window.viewers = {};
	$scope.msg = pageData.Msg;
    $scope.owl = null;
    $scope.data = null;
    $scope.UserInbox = {};
    $scope.PopUpMessage = '';
    $scope.messageError = false;
	$scope.PropertyId = null;
	$scope.MarketPriceUrl = pageData.Data.MarketPriceUrl;
	$scope.SendMessageUrl = '/template/property_sendmessage';
    $scope.topMedia = {
        element: '.top-media',
        page: { index: 1, size: 5, total: 1, itemCount: 1 },
        index: 1,
		total: 1,
		counterText: '',
        options: {
            items: 1,
            rewind: true,
            nav: true,
            navText: ['&#x2039;', '&#x203a;'],
            video: true,
            dots: true,
            dotData: false,
            onResize: function (e) {

            },
            onChanged: function (e) {
				if (e.page.index < 0) return;
				$scope.topMedia.counter(e,true);
            },
            onInitialized: function (e) {
                var o = $scope.topMedia.page;
                var c = e.item.count;
                o.total = (c - c % o.size) / o.size + (c % o.size > 0 ? 1 : 0);
				o.itemCount = c;
				$scope.topMedia.counter(e);
            }
        },
        init: function () {
            $scope.owl = $("#top-media").owlCarousel($scope.topMedia.options);
        },
		counter: function (e, a) {
			if (e.item.index < 0) {
				$scope.topMedia.counterText = '';
				return;
			}
			$scope.topMedia.total = e.item.count;
			$scope.topMedia.counterText = '' + (1 + e.item.index) + '/' + e.item.count;
			if (a) $scope.$digest();
		}
	};

    $scope.SimilarPropOptions = {
        items: 1,
        nav: true,
        navText: ['&#x2039;', '&#x203a;'],
        margin: 20,
        responsive: {
			0: { items: 1, stagePadding: 20, margin: 10, nav: true },
			375: { items: 1, stagePadding: 50, margin: 10, nav: true },
			425: { items: 1, stagePadding: 70, margin: 10, nav: true },
            640: { items: 2, stagePadding: 50, nav: true },
            768: { items: 3 },
            1024: { items: 4 }
        }
    };

    $scope.Message = {
        Send: function (form) {
			if (form.$valid === false) return;

			if (captchaValidate('sendmessage') === false) {
				return false;
			}

			$scope.UserInbox.Captcha = captcha_resp;
            agentServices.SendMessageToAgent($scope.UserInbox).then(function (rp) {
                form.$setUntouched();
			});
			$('#message').modal('hide');
            mogiUtils.showAlert($scope.msg.ThankYou);
		},
		Open: function () {
			captchaInit();
		}
    };

    $scope.PhoneFormat = function (num) {
        var phone = num.replace(/(\d{1,100})(\d{3})(\d{3})/, '$1 $2 $3');
        return phone;
    };

    $scope.MarketPrice = (function () {
        return {
            PriceToText: function (x) {
                return mogiResUtils.currencyUtils.toShortTextv2(x);
            },
            IsViewMore: false,
            ViewMore: function () { this.IsViewMore = true; }
        };
    }());

    $scope.UserType = function (typeid) { return mogiResUtils.getUserType(typeid); };

    $scope.Report = (function () {
        var _errors = 0;
        return {
            ErrorList: [],
            Open: function () {
				var self = this;
				captchaInit();
                if (this.ErrorList.length === 0) {
					propertyServices.GetMessages().then(function (rp) {
						rp = rp.data;
                        if (rp.Data !== null) {
                            self.ErrorList = rp.Data;
                        }
                    });
                }
            },
            SendReport: function (form) {
                if (form.report.$valid === false) return;

				if (captchaValidate('sendreport') === false) {
					return false;
				}

                var report = {
                    PropertyId: $scope.PropertyId,
                    FeedbackTypes: _errors,
                    Captcha: captcha_resp
                };
				propertyServices.SendReport(report).then(function (rp) {
                    _errors = 0;
				});
				$('#report').modal('hide');
				mogiUtils.showAlert($scope.msg.ThankYou);
            },
            OnChecking: function (index, value) {
                if ($scope.Report.IsChecked(index) !== true) {
                    _errors = (_errors | value);
                } else {
                    _errors = (_errors ^ value);
                }
            },
            IsChecked: function (index) {
                return ((_errors & $scope.Report.ErrorList[index].Value) == $scope.Report.ErrorList[index].Value);
            },
            ItemCss: function (index) {
                return $scope.Report.IsChecked(index) ? { 'fa-check-square': true } : { 'fa-square-o': true };
            }
        };
    })();

    // Calc
    $scope.CalcChartData = {
        datasets: [{
            data: [1, 2],
            backgroundColor: ["#262637", "#00C8A4"]
        }],
        labels: [$scope.msg.principal, $scope.msg.interest]
    };
    function UpdateCalcChartData(principal, interest) {
        $scope.CalcChartData = {
            datasets: [{
                data: [principal, interest],
                backgroundColor: ["#262637", "#00C8A4"],
                borderWidth: [1, 1]
            }],
            labels: [
                $scope.msg.principal,
                $scope.msg.interest
            ]
        };
    }
    $scope.CalcChartOptions = {
        responsive: true,
        maintainAspectRatio: false,
        legend: {
            display: false
        },
        tooltips: {
            enabled: false
        },
        cutoutPercentage: 75,
        animation: { duration: 0 },
        innerText: 0,
        legendCallback: function (chart) {
            var text = [];
            text.push('<ul>');
            for (var i = chart.data.datasets[0].data.length - 1; i >= 0; i--) {
                text.push('<li>');
                text.push('<div class="square" style="background-color:' + chart.data.datasets[0].backgroundColor[i] + '"></div>');
                if (chart.data.labels[i]) {
                    text.push(chart.data.labels[i] + ': ');
                }
                text.push('<span>' + chart.data.datasets[0].data[i] + ' ' + $scope.msg.million + '</span>');

                text.push('</li>');
            }
            text.push('</ul>');
            return text.join("");
        }
    };
    $scope.CalcLegend = "";
    $scope.chartext = [{
        beforeDraw: function (chart, options) {
            var width = chart.chart.width,
                height = chart.chart.height,
                ctx = chart.chart.ctx;

            ctx.restore();
            var fontSize = (height / (114 * 1.5)).toFixed(2);
            ctx.font = fontSize + "em sans-serif";
            ctx.textBaseline = "middle";

            var text = chart.options.innerText + " " + $scope.msg.million + "/" + $scope.msg.month,
                textX = Math.round((width - ctx.measureText(text).width) / 2),
                textY = height / 2;

            ctx.fillText(text, textX, textY);
            ctx.save();
        }
    }];

    $scope.TrustedHtml = function (html) {
        return $sce.trustAsHtml(html);
    };

	$scope.HistoryPrice = {
		Items: [],
		Init: function (sid, area) {
			var from = Math.round(Math.max(area - area * 0.1, 0));
			var to = Math.round(area + area * 0.1);
			propertyServices.GetHistoryPropertyByStreet(sid, from, to).then(function (rp) {
				rp = rp.data;
				if (rp.Status === true) {
					$scope.HistoryPrice.Items = rp.Data;
				}
			});
		},
		IsShow: function () { return this.Items.length > 0; }
	};

	$scope.SearchNav = {
		show: true,
		url: '',
		next: true,
		nextUrl: '',
		pid: 0,
		init: function (id) {
			this.pid = id;
			var data = $cache.getJson('mogi:searchresult', null);
			if (data) {
				this.url = data.Url;
				var i = data.Ids.findIndex(function (v) { return v === id; });
				//this.show = i >= 0;
				if (i < data.Ids.length - 1) {
					//this.next = true;
					this.nextUrl = '/property/go/' + data.Ids[i + 1];
				}
			}
			if (this.url === '') {
				this.url = pageData.SearchUrl;
			}
		},
		getNextUrl: function () {
			if (this.nextUrl !== '') return true;
			var pindex = 1, pid = this.pid;
			var pd = pageData.Data;
			var data = $cache.getJson('mogi:searchnext', null);
			if (data) {
				if (data.rent === pd.rent && data.did === pd.did && data.psid === pd.psid) {
					pindex = data.pindex + 1;
				}
			}
			data = { rent: pd.rent, did: pd.did, psid: pd.psid, pindex: pindex };
			$cache.setJson('mogi:searchnext', data);
			
			propertyServices.GetNext({ rent: pd.rent, did: pd.did, psid: pd.psid, pindex: pindex }).then(function (resp) {
				var url = pageData.SearchUrl;
				if (resp.data) {
					for (var i = 0; i < resp.data.length; i++){
						if (resp.data[i].PropertyId !== pid) {
							url = resp.data[i].Url;
						}
					}
				}
				document.location.assign(url);
			});
			return false;
		}
	};
	$scope.SimilarUrl = null;
	$scope.loadSimilar = function () {
		$scope.SimilarUrl = pageData.SimilarUrl;
	};
	$scope.loadSimilarComplete = function () {
		var root = document.getElementById('similarProperty');
		var observer = lozad('.lozad', { threshold: 0.1, root: root });
		observer.observe();
	};

	$scope.Init = function () {
		var data = pageData.Data;
		$scope.data = data;
		$scope.SearchNav.init(data.propertyId);
        $scope.topMedia.init();
        $scope.UserInbox = data.userInbox;
        $scope.PropertyId = data.propertyId;
		$scope.Favorite.init([data.propertyId]); // app.js
        $scope.BankLoanCalc.Init();
        $scope.HistoryPrice.Init(data.streetId, data.Area);
    };

    $scope.BankLoanCalc = (function () {
        var defaultBank = "BIDV";
        var defaultPrePayPercents = 30;
        var banks = [];

        var getPrePayPercents = function (maxLoan) {
            var min = 100 - maxLoan;
            var ranges = [];
            for (var i = min; i <= 95; i += 5) {
                ranges.push({
                    v: i,
                    n: i + '%'
                });
            }
            return ranges;
        };

        var getYears = function (min, max) {
            var ranges = [];
            for (var i = min; i <= max; i++) {
                ranges.push({
                    v: i
                });
            }
            return ranges;
        };

        var ProcessData = function () {
            $scope.BankLoanCalc.Bank = $scope.BankLoanCalc.Banks()[0];
            for (var i = 0; i < banks.length; i++) {
                banks[i].PrePayPercents = getPrePayPercents(banks[i].MaxLoan);
                banks[i].Years = getYears(banks[i].MinYear, banks[i].MaxYear);
                if (banks[i].InterestRateFormula !== '') {
                    banks[i].Rate = '(' + banks[i].InterestRateFormula + ')';
                }
                if (banks[i].Name === "BIDV") {
                    $scope.BankLoanCalc.Bank = $scope.BankLoanCalc.Banks()[i];

                }
            }

            var minLoan = 100 - $scope.BankLoanCalc.Bank.MaxLoan;
            if (defaultPrePayPercents >= minLoan) {
                for (var j = 0; j < $scope.BankLoanCalc.Bank.PrePayPercents.length; j++) {
                    if ($scope.BankLoanCalc.Bank.PrePayPercents[j].v == defaultPrePayPercents) {
                        $scope.BankLoanCalc.PrePayPercent = $scope.BankLoanCalc.Bank.PrePayPercents[j];
                    }
                }
            } else {
                $scope.BankLoanCalc.PrePayPercent = $scope.BankLoanCalc.Bank.PrePayPercents[0];
            }

        };
        var GetBankData = function () {
            banks = mogiDatas.Loan;
            ProcessData();
            $scope.BankLoanCalc.Year = $scope.BankLoanCalc.Bank.Years[$scope.BankLoanCalc.Bank.Years.length - 1];
            //Set default bank
            $scope.BankLoanCalc.PrePayPercentChange();
            $scope.BankLoanCalc.YearChange();
        };
        var calc = function () {
            var prePay = $scope.Price * $scope.BankLoanCalc.PrePayPercent.v / 100;
            var year = $scope.BankLoanCalc.Year.v;
            var rate = $scope.BankLoanCalc.Bank.InterestRate;

            var loanAmount = $scope.Price - prePay;
            var monthlyPay = (loanAmount + loanAmount * year * (rate / 100)) / (year * 12);
            var monthlyPrincipalPay = Math.round((loanAmount / (year * 12)) / 1000000);
            if (isNaN(monthlyPay)) return;

            $scope.MonthlyPay = Math.round(monthlyPay / 1000000);

            $scope.CalcChartOptions.innerText = $scope.MonthlyPay;

            UpdateCalcChartData(monthlyPrincipalPay, Math.round($scope.MonthlyPay - monthlyPrincipalPay));
        };

        return {
            Banks: function () { return banks; },
            Bank: null,
            PrePayPercent: 0,
			Year: 0,
			PriceText: '',
            BankChange: function () {
				if (this.Bank === null) return;
				var arr = this.Bank.PrePayPercents;
				for (var i = 0, l = arr.length; i < l; i++) {
					if (arr[i].v >= defaultPrePayPercents) {
						$scope.BankLoanCalc.PrePayPercent = arr[i];
						break;
					}
				}
				//$scope.BankLoanCalc.PrePayPercent = this.Bank.PrePayPercents[0];
                $scope.BankLoanCalc.Year = this.Bank.Years[this.Bank.Years.length - 1];
                this.PrePayPercentChange();
                calc();
            },
            PrePayPercentChange: function () {
                if (this.Bank === null) return;
                this.Rate = this.Bank.InterestRate + '%' + this.Bank.Rate;
                calc();
            },
            YearChange: function () {
                if (this.Bank === null) return;
                calc();
            },
            Rate: '',
            Init: function () {
				$scope.Price = pageData.Data.price;
				$scope.BankLoanCalc.PriceText = pageData.Data.PriceText;
                GetBankData();
            }
        };
    }());

	if (w.IsMobile) {
		$scope.headerUrl2 = "detail-header.html";
	} else {
		$scope.headerUrl = "detail-header.html";
	}
}
propertyDetailController.$inject = ['$scope', 'propertyServices', 'agentServices', '$sce'];
mogiApp.controller('propertyDetailController', propertyDetailController);


// google recaptcha
//var grecaptcha_id = 0;
//var reCaptchaOnload = function () {
//    try {
//        grecaptcha_id = grecaptcha.render("g-recaptcha");
//        grecaptcha.reset(grecaptcha_id);
//    }
//    catch (e) { console.log(e); }
//};
//reCaptchaReportCallBack = function () { $('#sendreport').click(); };
//reCaptchaMessageCallBack = function () { $('#sendmessage').click(); };