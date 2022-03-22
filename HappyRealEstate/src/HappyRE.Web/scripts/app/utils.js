String.prototype.format = function () {
    var str = this;
    for (var i = 0; i < arguments.length; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        str = str.replace(reg, arguments[i]);
    }
    return str;
};
var $m = {
	id: function (id) { return document.getElementById(id); },
	class: function (c, d) { d = d || document; return d.getElementsByClassName(c); },
	query: function (s) { return document.querySelectorAll(s); },
	object: function (o) { return typeof o === "object"; },
	defined: function (o) { return typeof o !== "undefined"; },
	map: function (f, t) { for (var p in f) t[p] = f[p]; }
};
var $getById = function (id) { return document.getElementById(id); };
var $getByClass = function (c, d) { d = d || document; return d.getElementsByClassName(c); };
var $cache = {
	support: $m.defined(Storage),
	set: function (k, v) {
		if (this.support) {
			localStorage.setItem(k, v);
		}
	},
	setJson: function (k, v) {
		if (this.support) {
			localStorage.setItem(k, JSON.stringify(v));
		}
	},
	get: function (k) {
		if (this.support) {
			return localStorage[k];
		}
		return void 0;
	},
	getJson: function (k, d) {
		var v = this.get(k);
		if (v !== null) {
			try { return JSON.parse(v); } catch (e) { return d; }
		}
		return d;
	}
};
var $captcha = {
	id: 0,
	resp: "",
	onload: function (elid) {
		elid = elid || 'recaptcha';
		$captcha.id = grecaptcha.render(elid);
	},
	callback : function (elid) {
		$captcha.resp = grecaptcha.getResponse($captcha.id);
		if ($captcha.resp !== "") {
			document.getElementById(elid).click();
		}
	},
	exec: function (e, id) {
		if (id) $captcha.id = id;
		$captcha.resp = grecaptcha.getResponse($captcha.id);
		if ($captcha.resp === "") {
			if(e) e.preventDefault();
			grecaptcha.execute($captcha.id);
			return false;
		}
		return true;
	}
};

var mogiUtils = (function ($) {
    var
    showAlert = function (message, type) {
        if (message === null || message === '') return;
        if (type === 'warning') toastr.warning(message);
        else if (type === 'error') toastr.error(message);
        else toastr.success(message);
    },
    boxAlert = function (message, callback) {
        bootbox.alert(message, callback);
    },
    boxSucess = function (message, callback) {
        bootbox.dialog({
            message: message,
            //size: 'medium',
            buttons: {
                success: {
                    label: mogiDatas.Msg.Button_Finished,
                    className: "btn-primary",
                    callback: function () {
                        if (callback) callback();
                    }
                }
            }
        });

    },
    boxConfirm = function (message, callback, cancelCallback) {
        //bootbox.confirm(message, callback);
        bootbox.dialog({
            message: message,
            title: mogiDatas.Msg.Confirm_Arlet,
            buttons: {
                danger: {
                    label: mogiDatas.Msg.Button_OKI,
                    className: "btn-orange",
                    callback: function () {
                        if (callback) callback();
                    }
                },
                success: {
                    label: mogiDatas.Msg.Button_Reject,
                    className: "btn-default",
                    callback: function () {
                        if (cancelCallback) cancelCallback();
                    }
                },


            }
        });
    },
    boxInfo = function (message, css) {
        css = css || '';
        bootbox.dialog({
            message: message,
            className: css,
            title: mogiDatas.Msg.Confirm_Title,
            buttons: {
                success: {
                    label: mogiDatas.Msg.Button_OKI,
                    className: "btn-danger"
                }
            }
        });
    }
    ScrollToTop = function () {
        $("html, body").animate({ scrollTop: 0 }, "fast");
    },
    ScrollToBottom = function () {
        var $target = $('html,body');
        $target.animate({ scrollTop: $target.height() }, "fast");
        //$("html, body").animate({ scrollBottom: 0 }, "fast");
    },
    paramsSearch = {

        PropertyTypeId: 0,
        PropertyStyles: [],
        Transaction: 0,
        Location: '',
        FromPrice: 0,
        ToPrice: 0,
        FromArea: 0,
        ToArea: 0,
        FromBedRoom: 0,
        ToBedRoom: 0,
        LegalId: 0,
        DirectionId: 0,
        Days: 0,
        Sort: '',
        Location: '',
        q: '',
        MapId: 0

    },
    friendlyUrl = {
        removeNullParams: function (p) {
            var res = {}, v;
            for (prop in p) {
                v = p[prop]; if (v != null) res[prop] = p[prop];
            }
            return res;
        },
        getPropertySearchUrl: function (p) {
            var q = '', c = null, d = null, mapUrl = '', pt = null, fp = {};
            pt = (p.PropertyTypeId <= 0 ? null : mogiResUtils.getPropertyType(p.PropertyTypeId));
            if (p.PropertyStyles != null) {
                if (p.PropertyStyles.length == 1) {
                    pt = mogiResUtils.getPropertyType(p.PropertyStyles[0]);
                } else {
                    fp.psid = p.PropertyStyles;
                }
            }

            if (p.Location != null) {
                var loc = p.Location;
                if (loc.p == 0) {
                    d = null;
                    c = mogiResUtils.GetCityById(loc.i);
                } else {
                    d = mogiResUtils.GetCityById(loc.i);
                    c = mogiResUtils.GetCityById(loc.p);
                }
            }

            if (p.Map != null && p.Map.MapId != null) {
                if (p.Map.ProjectId > 0 || p.Map.PlaceId > 0 || p.Map.StreetId > 0 || p.Map.WardId > 0) {
                    mapUrl = (p.Map.CodeUrl || '');
                }
                d = (p.Map.DistrictId <= 0 ? null : mogiResUtils.GetCityById(p.Map.DistrictId));
                c = (p.Map.CityId <= 0 ? null : mogiResUtils.GetCityById(p.Map.CityId));
            } else if ((q.Map || '') !== '') {
                fp.q = p.Map;
            }

            q += (c != null ? "/" + c.u : ""); // city
            q += (d != null ? "/" + d.u : ""); // district
            q += "/" + (p.Rent ? p.Msg.Routing_Rent_CodeUrl : p.Msg.Routing_Buy_CodeUrl); // transtype
            q += "-" + (pt != null ? pt.Code : p.Msg.Routing_PropertyType_All);
            q += (mapUrl != "" ? "/" + mapUrl : ""); // codeurl

            // price
            if (p.FromPrice > 0) fp.fp = p.FromPrice;
            if (p.ToPrice > 0) fp.tp = p.ToPrice;
            // area
            if (p.FromArea > 0) fp.fa = p.FromArea;
            if (p.ToArea > 0) fp.ta = p.ToArea;
            // bed room
            if (p.FromBedRoom > 0) fp.fbr = p.FromBedRoom;
            if (p.ToBedRoom > 0) fp.tbr = p.ToBedRoom;
            if (p.DirectionId > 0) fp.dt = p.DirectionId;
            if (p.LegalId > 0) fp.lg = p.LegalId;
            if (p.Days > 0) fp.d = p.Days;
            if (p.Sort && p.Sort != '' && p.Sort != 'publish_desc') fp.s = p.Sort;

            var qs = $.param(fp, true);
            q += (qs == '' ? '' : '?') + qs;
            return q;
        },
        mapSearchFilter: function (item) {
            var f = ["CityId", "cid", "Days", "d", "DirectionId", "dt", "DistrictId", "did", "FromArea", "fa", "ToArea", "ta", "FromBedRoom", "fbr", "ToBedRoom", "tbr", "FromPrice", "fp", "ToPrice", "tp", "LegalId", "lg", "Map", "Map", "Location", "l", "PropertyTypeId", "pid", "ProjectId", "pjid", "PlaceId", "plid", "Radius", "r", "PropertyStyles", "psid", "WardId", "wid", "Polyenc", "poly", "StreetId", "sid", "Sort", "s", "ReferId", "rid", "MapId", "id", "ReferTypeId", "rtid", "Rent", "Rent", "q", "q"];
            var p = { Msg: mogiDatas.Msg };
            for (var i = 0; i < f.length - 1; i += 2) {
                if (!item[f[i + 1]]) continue;
                p[f[i]] = item[f[i + 1]];
            }
            return p;
        },
        getPropertyDetailUrl: function (u, id, pid, rent) {
            u = (u || "");
            if (u != "") return u;
            var o = resUtil.getPropertyType(pid);
            return (rent ? "/cho-thue-" : "/ban-") + (o ? o.Code : "nha-dat") + "/chi-tiet-pr" + id;
        },
        getBrokerSearchUrl: function (p) {
            if (p.mode == conf.Const.ReferType.PROJECT) {
                p.p = p.sp = null;
            } else {
                p.pj = null;
            }
            var q = "";
            var pid = (p.sp || p.p || 0), did = (p.d || 0);
            var oc = resUtil.getCity(did), op = resUtil.getPropertyType(pid);
            if (op != null) {
                q += "-" + op.Code + "/" + oc.u + ((p.sp || 0) > 0 ? "/spid" : "/pid") + pid;
            } else if ((p.pj || 0) > 0) {
                q += "-du-an/" + oc.u + "/pjid" + p.pj;
            } else {
                q += "/" + oc.u;
            }
            if ((p.p || p.sp || p.pj || 0) > 0) delete p.mode;
            delete p.ch; delete p.pj; delete p.sp; delete p.p; delete p.d; delete p.c;
            if ((p.cp || 0) <= 1) delete p.cp;
            if ((p.pz || 0) <= 10) delete p.pz;

            var data = $.param(p, true);
            if (data != "") q += "?" + data;
            window.location.href = conf.Routes.broker.listing + q;
        },
        getProjectSearchUrl: function (p) {
            var q = "";
            var pid = (p.p || 0), did = (p.d || 0);
            var oc = resUtil.getCity(did), op = resUtil.getPropertyType(pid);
            if (op != null) {
                q += "-" + op.Code + "/" + oc.u + "/pid" + pid;
            } else {
                q += "/" + oc.u;
            }
            delete p.ch; delete p.c; delete p.d; delete p.c;
            if ((p.cp || 0) <= 1) delete p.cp;
            if ((p.pz || 0) <= 10) delete p.pz;

            var data = $.param(p, true);
            if (data != "") q += "?" + data;
            window.location.href = conf.Routes.project.listing + q;
        },
        JumpUrl: function (ch, did) {
            var p = { ch: ch, d: did };
            if ((did || 0) < 0) {
                BoxInfo("Bạn vui lòng chọn một Quận/Huyện.");
                return;
            }
            ch = (ch || 2);
            switch (ch) {
                case conf.Const.menu.topmenu.project:
                    this.getProjectSearchUrl(p);
                    break;
                case conf.Const.menu.topmenu.broker:
                    this.getBrokerSearchUrl(p);
                    break;
                default:
                    this.getPropertySearchUrl(p).done(function (url) {
                        if (url != "") window.location.href = url;
                    });
                    break;
            }
        }
    },
    GetFullCityName = function (id) {
        id = id || 0;
        if (id == 0) return '';
        var cityName = '';
        var obj = mogiResUtils.GetCityById(id);
        if (obj != null) {
            cityName = obj.n;
            if (obj.p > 0) {
                obj = mogiResUtils.GetCityById(obj.p);
                if (obj != null) {
                    cityName = obj.n + ', ' + cityName;
                }
            }
        }
        return cityName;
    },
    TopMenuOnScroll = function () {
        window.addEventListener('scroll', function (e) {
            //var m = $("#mogi-navbar");
            ////var h = (mogiUtils.IsResponsive() ? 50 : 80);
            //var h = 50;
            //if ($(window).scrollTop() >= h) {
            //    m.addClass("smaller");
            //} else {
            //    m.removeClass("smaller");
            //}
        });
    },
    IsResponsive = function () {
        var width = $(window).width();
        return (width <= 991);
    },
    IsResponsive_Max360 = function () {
        var width = $(window).width();
        return (width <= 360);
    },
    IsResponsive_Max479 = function () {
        var width = $(window).width();
        return (width <= 479);
    },
    IsResponsive_XS = function () {
        return ($(window).width() < 768);
    },
    //màn hình máy smartphone
    IsResponsive_Max768 = function () {
        var width = $(window).width();
        return (width < 767);
    },
    IsResponsive_Max640 = function () {
        var width = $(window).width();
        return (width <= 640);
    },
    IsResponsive_Max991 = function () {
        var width = $(window).width();
        return (width <= 991);
    },
    changeLanguage = function (v) {
        var l = _.find(mogiDatas.Languages, function (o) { return o.c == v; });
        if (l === null) return true;
        $.cookie('lang', '', { expires: -1, path: '/' });
        $.cookie('lang', '', { expires: -1, path: location.pathname });
        $.cookie('lang', v, { path: '/' });
        return true;
    },
    renderLanguage = function () {
        var d = mogiDatas, s = '', o = d.Lang;
        $('#lang-title').html('<span class="micon flag-' + o.l + '"></span><span id="lang-name">' + o.n + '</span>');
        for (var i = 0; i < d.Languages.length; i++) {
            o = d.Languages[i];
            s += '<a class="dropdown-item" href="' + o.u + '" onclick="mogiUtils.changeLanguage(' + o.i + ')"><span class="micon flag-' + o.l + '"></span>' + o.n + '</a>';
        }
        $('#lang-items').html(s);
    },
    renderLanguageMobile = function () {
		var d = mogiDatas, s = '', l = '', o = null;
        for (var i = 0; i < d.Languages.length; i++) {
            o = d.Languages[i];
			if (o.i === d.Lang.i) continue;
			l = o.l.substr(0, 2);
			s += '<li><a href="' + o.u + '" onclick="mogiUtils.changeLanguage(' + o.i + ')"><svg class="mi mi-' + l + '"><use xlink:href="/content/fonts/mogi-icons.svg#mi-' + l + '"></use></svg>' + o.n + '</a></li>';
        }
        return s;
    },
    getLanguage = function (v) {
        var d = mogiDatas, u = '/';
        for (var i = 0; i < d.Languages.length; i++) {
            var o = d.Languages[i];
            if (o.i === v || o.c === v) {
                u = o.u;
                break;
            }
        }
        return u;
    };

    return {
        showAlert: showAlert,
        boxSucess: boxSucess,
        boxInfo: boxInfo,
        boxAlert: boxAlert,
        boxConfirm: boxConfirm,
        ScrollToTop: ScrollToTop,
        ScrollToBottom: ScrollToBottom,
        TopMenuOnScroll: TopMenuOnScroll,
        GetFullCityName: GetFullCityName,
        friendlyUrl: friendlyUrl,
        paramsSearch: paramsSearch,
        IsResponsive: IsResponsive,
        IsResponsive_Max360: IsResponsive_Max360,
        IsResponsive_Max479: IsResponsive_Max479,
        IsResponsive_XS: IsResponsive_XS,
        IsResponsive_Max768: IsResponsive_Max768,
        IsResponsive_Max640: IsResponsive_Max640,
        IsResponsive_Max991: IsResponsive_Max991,
        changeLanguage: changeLanguage,
        renderLanguage: renderLanguage,
        renderLanguageMobile: renderLanguageMobile,
        getLanguage: getLanguage
    };
})(jQuery);