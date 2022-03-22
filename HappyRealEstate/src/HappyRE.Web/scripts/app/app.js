angular_modules = window.angular_modules || ['ngMessages', 'ngAnimate', 'ngSanitize', 'ui.bootstrap', 'blockUI', 'ngFileUpload', 'localytics.directives', '720kb.socialshare'];
var mogiApp = angular.module('mogiApp', angular_modules);

if (_.indexOf(angular_modules, 'blockUI') !== -1) {
	mogiApp.config(['blockUIConfig', function (conf) {
		conf.autoBlock = false;
	}]);
}
if (_.indexOf(angular_modules, 'ui.bootstrap.pagination') !== -1) {
	mogiApp.config(['uibPaginationConfig', function (conf) {
		//conf.itemsPerPage = 20;
		conf.firstText = '«',
			conf.nextText = '›';
		conf.lastText = '»';
		conf.previousText = '‹';
		//conf.boundaryLinks = true;
		//conf.maxSize = 10;
	}]);
}
function rootService($rootScope, $http, $templateCache) {
	var $rs = $rootScope;
	$rs.ngMsg = function (o) { return (o.$touched || (o.$submitted || false)) && o.$error; };
	// SlideMenu
	$rs.isOpenNav = false;
	$rs.openNav = function () {
		if ($rs.isOpenNav) {
			$rs.closeNav();
			return;
		}
		$rs.isOpenNav = true;
		$getById("slidemenu").style.left = "0px";
		$getById("mogi-overlay").style.display = "block";
		document.body.style.overflow = 'hidden';
	};
	$rs.closeNav = function () {
		$getById("slidemenu").style.left = "-250px";
		$getById("mogi-overlay").style.display = "none";
		document.body.style.overflow = 'auto';
		$rs.isOpenNav = false;
	};

	// Profile
	$rs.Profile = {
		Data: { ClientId: null, ProfileId: 0, FirstName: "", Avatar: "", TotalFavorite: 0, IsAuth: false, Alert: null },
		getInfo: function () {
			var o1 = $rs.Profile.Data;
			var o2 = window.MOGI;
			for (var p in o1) {
				o1[p] = o2[p];
			}
			if (o2.Token !== '') {
				$http.defaults.headers.common.Authorization = o2.Token;
			}
		},
		isAuth: function () { return this.Data.IsAuth; },
		getAvatar: function () {
			var u = this.Data.Avatar; u = '';
			return u === '' ? '<i class="fa fa-user-circle"></i>' : '<img ng-src="' + u + '" width="35" height="35" />';
		},
		getFavorite: function () {
			var v = this.Data.TotalFavorite;
			if (v < 10) return v;
			return '9+';
		},
		savedAlert: function () {
			var d = $rs.Profile.Data.Alert;
			if (!d) return false;
			return d.AlertSearchId > 0;
		},
		Message: {
			Total: 0,
			Items: [], Inboxs: [],
			HasValue: false,
			HasMsg: false,
			HasInbox: false,
			KeyMsg: 'mogi:msg',
			KeyInbox: 'mogi:inbox',
			IsRead: function (o, m, oc, nc) {
				var k = m.MessageId;
				o.push(m);
				m.Read = m.Read || false;
				if (oc[k]) {
					m.Read = true;
					nc[k] = true;
				}
			},
			GetMessage: function () {
				var nc = {msg: {}, inbox: {}}, oc = {};
				oc.msg = $cache.getJson(this.KeyMsg, {});
				oc.inbox = $cache.getJson(this.KeyInbox, {});
				$http({ method: 'GET', url: mogiRoutes.Common.GetMessage + '/' + MOGI.Member }).then(function (res) {
					var d = res.data;
					var o = $rs.Profile.Message;
					o.Total += d.length;
					o.HasMsg = d.length > 0;
					o.HasValue |= o.HasMsg;
					for (var i = 0, t = d.length; i < t; i++) {
						o.IsRead(o.Items, d[i], oc.msg, nc.msg);
					}
					$cache.setJson(o.KeyMsg, nc.msg);
				});
				if (MOGI.IsAuth && MOGI.Member === 'mogipro') {
					$http({ method: 'GET', url: mogiRoutes.Common.Profile_GetInbox }).then(function (res) {
						var d = res.data;
						var o = $rs.Profile.Message;
						o.Total += d.length;
						o.HasInbox = d.length > 0;
						o.HasValue |= o.HasInbox;
						for (var i = 0, t = d.length; i < t; i++) {
							o.IsRead(o.Inboxs, d[i], oc.inbox, nc.inbox);
						}
						$cache.setJson(o.KeyInbox, nc.inbox);
					});
				}
			},
			Read: function (id,f) {
				if (f && MOGI.IsAuth) {
					$http({ method: 'GET', url: mogiRoutes.Profile.ReadMessage, params: { id: id } });
				}
				var k = f ? this.KeyInbox : this.KeyMsg;
				var d = $cache.getJson(k, {});
				d[id] = true;
				$cache.setJson(k, d);
			}
		}
	};

	$rs.Favorite = {
		auth: false,
		loaded: false,
		max: 200,
		key: 'mogi:favorite',
		items: {},
		init: function (ids) {
			if ($m.defined(window.MOGI) === false) {
				setTimeout(function () { $rs.Favorite.init(ids); }, 10);
				return;
			}
			var self = this;
			self.auth = MOGI.IsAuth;
			if (self.auth) {
				if (ids.length > 0) {
					self.get(ids).then(function (resp) {
						resp.data.forEach(function (v) { if (v !== null) { self.items[parseInt(v, 10)] = true; } });
					});
				}
			} else {
				this.loadCache();
			}
		},
		loadCache: function () {
			var self = this;
			if (self.loaded) return;
			self.loaded = true;
			var l = $cache.getJson(this.key, []);
			$rs.Profile.Data.TotalFavorite = l.length;
			l.forEach(function (v) { self.items[v] = true; });
		},
		get: function (ids) {
			return $http({ method: 'POST', url: mogiRoutes.Common.Favorite_GetList, data: ids });
		},
		remove: function (id) {
			return $http({ method: 'POST', url: mogiRoutes.Common.Favorite_Remove + "?propertyId=" + id });
		},
		add: function (id) {
			return $http({ method: 'POST', url: mogiRoutes.Common.Favorite_Add + "?propertyId=" + id });
		},
		update: function (v) {
			var d = $rs.Profile.Data;
			v = d.TotalFavorite + v;
			d.TotalFavorite = Math.max(0, v);
		},
		addRemove: function (id, e) {
			var f = this.items[id] || false;
			this.cache(id, f);
			if (f === false) {
				this.items[id] = true;
				this.add(id).then(function () { });
				this.update(1);
			}
			else
			{
				delete this.items[id];
				this.remove(id).then(function () { });
				this.update(-1);
			}
			if (e) { e.currentTarget.classList.toggle('favorited'); }
		},
		cache: function (id, v) {
			var l = $cache.getJson(this.key, []);
			var f = l.indexOf(id);
			if (v) {
				if (f !== -1) l.splice(f, 1);
			} else {
				if (f === -1) l.push(id);
			}
			if (l.length > this.max) {
				l = l.slice(l.length - this.max);
			}
			$cache.setJson(this.key, l);
		}
	};

	$rs.TemplateTopMenuUrl = window.TemplateTopMenuUrl;
	var template = window.template_layout_topmenu;
	if ($m.object(template) === true && $m.defined($templateCache.get(template.url)) === false) {
		$templateCache.put(template.url, template.data);
	}

	function loadInfo() {
		if ($m.defined(window.MOGI) === false) {
			setTimeout(loadInfo, 5);
			return;
		}
		$rs.Profile.getInfo();
		if (MOGI.IsAuth) $rs.Profile.Message.GetMessage();
		$rs.Favorite.loadCache();
	}
	loadInfo();
}

rootService.$inject = ['$rootScope', '$http', '$templateCache', '$compile'];
mogiApp.run(rootService);

//var surveymonkey_total = 0;
//function surveymonkey_init() {
//    surveymonkey_total += 1;
//    if (surveymonkey_total >= 1000) return;
//    if ($('.smcx-modal').html() === undefined) {
//        setTimeout(surveymonkey_init, 100);
//        return;
//    }
//    $('.smcx-modal').attr('style', "width: 100% !important;max-width: initial;left: 0;margin-left: 0px !important;margin: 0 !important;top: 0;height: 100%;background-color: rgba(0,0,0,0.7);border-radius:0!important;");
//    $('.smcx-modal-content').attr('style', 'max-width: 505px;height: 255px;margin: auto;margin-top:100px;');
//    $('.smcx-widget-footer').attr('style', 'display:none');
//}
//surveymonkey_init();
lazyLoadCounter = 0;
function lazyLoad(root) {
	if (lazy.length === 0 || lazyLoadCounter > 3000) {
		var observer = lozad('.lozad', {threshold: 0.1, root: root});
		observer.observe();
	} else {
		lazyLoadCounter += 20;
		setTimeout(lazyLoad, 20);
	}
}

$(document).ready(function () {
	setTimeout(lazyLoad, 20);

	if (mogiUtils.IsResponsive_Max768()) {
		$(".mogi-collapse").on("click", function () {
			$($(this).data("target")).collapse("toggle");
		});
		var l = mogiUtils.renderLanguageMobile();
		var p = $('#profile-menu').html();
		$("#mogi-menu").append(l).append(p);
	}

	if (typeof toastr !== "undefined") {
		toastr.options = {
			"closeButton": true,
			"debug": false,
			"newestOnTop": false,
			"progressBar": false,
			"positionClass": "toast-top-center",
			"preventDuplicates": false,
			"onclick": null,
			"showDuration": "300",
			"hideDuration": "1000",
			"timeOut": "5000",
			"extendedTimeOut": "1000",
			"showEasing": "swing",
			"hideEasing": "linear",
			"showMethod": "fadeIn",
			"hideMethod": "fadeOut"
		};
	}
});
