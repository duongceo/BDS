var mogiResUtils = (function ($, _) {
    var getSysTables = function () {
        return mogiDatas.SysTable;
    },
    getSysCode = function (id) {
        return mogiSysCode.hash['c' + id];
    },
    getHash = function (key, arr, id) {
        var res = this[key];
        if (res == null) {
            var h = {};
            if (!id) id = 'Id';
            for (var i = 0; i < arr.length; i++) {
                h['c' + arr[i][id]] = arr[i];
            }
            this[key] = res = h;
        }
        return res;
    },
    filterUtils = {
        getFilterArea: function () {
            return mogiDatas.Filters.hash["AREA"];
        },
        getFilterPriceRent: function () {
            return mogiDatas.Filters.hash["PRICE_RENT"];
        },
        getFilterPriceSale: function () {
            return mogiDatas.Filters.hash["PRICE_SALES"];
        },
        getFilterRoadwidth: function () {
            return mogiDatas.Filters.hash["ROADWIDTH"];
        },
        getFilterRoom: function () {
            return mogiDatas.Filters.hash["ROOM"];
        },
        getFilterTime: function () {
            return mogiDatas.Filters.hash["TIME"];
        },
        getDirections: function () {
            return mogiDatas.Filters.hash["DIRECTION"];
        },
        getLegals: function () {
            return mogiDatas.Filters.hash["LEGAL"];
        },
        getTransTypes: function () {
            return mogiDatas.Filters.hash["TRANSTYPE"];
        },
        getPropertyTypes: function () {
            return mogiDatas.Filters.hash["PROPERTYTYPE"];
        },
        getCities: function () {
            return mogiDatas.Filters.hash["CITY"];
        }
    },
    syscodeUtils = {
        getByBitMask: function (t, b) {
            var q = mogiSysCode[t];
            if (q == null) return null;
            return _.find(q, function (item) { return item.b == b; });
        },
        getByCode: function (t, c) {
            var q = mogiSysCode[t];
            if (q == null) return null;
            return _.find(q, function (item) { return item.c == c; });
        },
        getServiceBase: function (b) {
            return mogiSysCode.SERVICEBASE;
        },
        getLevelFeatures: function () {
            return mogiSysCode.LEVEL_FEATURE;

        },
        GetReferType: function () {
            return mogiSysCode.REFERTYPE;
        },
        getGenders: function () {
            return mogiSysCode.GENDER_TITLE;
        },
        getUserTypes: function () {
            return mogiSysCode.USERTYPE;
        },
        getIdCardTypes: function () {
            return mogiSysCode.URL;
        },
        getProjectUtitlities: function () {
            return mogiSysCode.PROJECT_UTILITY;
        },
        getProjectFeatures: function () {
            return mogiSysCode.PROJECT_FEATURE;
        },
        getProjectStatus: function () {
            return mogiSysCode.PROJECT_STATUS;
        },
        getCurrentCy: function () {
            return mogiSysCode.RE_CURRENCY_FORMAT;
        },
        getOutdoorUtility: function () {
            return mogiSysCode.RE_OUTDOOR_UTILITY;
        },
        getIntdoorUtility: function () {
            return mogiSysCode.RE_INDOOR_UTILITY;
        },
        getIntdoorUtility: function () {
            return mogiSysCode.RE_INDOOR_UTILITY;
        },
        getRefilterSort: function () {
            return mogiSysCode.RE_FILTER_SORT;
        },
        getAlertType: function () {
            return mogiSysCode.RE_FILTER_ALERT;
        },
        getGoogleNearBy: function () {
            return mogiDatas.NearBy;
        }
    },
    getRewardTypes = function () {
        return mogiDatas.RewardType;
    },
    getLevels = function () {
        return mogiDatas.Level;
    },
    getLevelById = function (id) {
        var obj = getLevels();
        return _.find(obj, function (item) { return item.i == id; });
    },
     getLevelByCode = function (code) {
         var obj = getLevels();
         return _.find(obj, function (item) { return item.c == code; });
     },
    currencyUtils = {
        toVND: function (currencyId, price, area) {
            var q = _.find(bdsCurrencies, function (e) { return e.Id == currencyId; });
            if (q == null) return 0;
            if (q.IsTotal == true) return (price * q.Price);
            return (price * q.Price) * area;
        },
        priceToText: function (v) {
            var arr = mogiSysCode.RE_CURRENCY_FORMAT;
            var r = "", d = 0, f = 1000000000;
            for (var i = 0; i < 3; i++) {
                if (v >= f) {
                    d = (v - (v % f)) / f;
                    v = v % f;
                    r += d + arr[i].n;
                }
                f = (f / 1000);
            }
            return r;
        },
        toShortText: function (v) {
            var arr = mogiSysCode.RE_CURRENCY_FORMAT;
            var r = "", d = "", f = 1000000000;
            for (var i = 0; i < 3; i++) {
                if (f > v) { f = (f / 1000); continue; }
                v = v / f; v = v.toFixed(3);
                d = v.substring(v.length - 3, v.length - 1);
                if (d == '00') d = '';
                else if (d.charAt(1) == '0') d = d.substring(0, 1);
                r = v.substring(0, v.length - 4) + (d == '' ? '' : ',' + d) + arr[i].n;
                break;
            }
            return r;
        },
        toShortTextv2: function (v) {

            var arr = mogiSysCode.RE_CURRENCY_FORMAT;
            var million = Math.pow(10, 6);
            var billion = Math.pow(10, 9);
            var p = '';
            var t = '';
            if (v >= billion) {
                p = (v / billion).toFixed(2);
                t = arr[0].n;
            } else if (v >= million) {
                p = (v / million).toFixed(2);

                t = arr[1].n;

            } else {
                p = v;
                return p.toFixed(0).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
            }
            p = (Math.floor(p) + (p - Math.floor(p))) + t;
            return p;
        },
        getById: function (id) {
            return _.find(bdsCurrencies, function (e) { return e.Id == id; });
        }
    };
    GetCities = function () {
        return mogiCities.data;
    },
    GetCityById = function (id) {
        return mogiCities.hash['c' + id];
    },
    GetCityByCode = function (c) {
        if (c == null || c == '') return null;
        var d = mogiCities.data;
        if (c.length == 4) {
            var o = _.findWhere(d, { co: c.substring(0, 2) });
            if (o != null) d = o.c;
        }
        if (d == null) return null;
        return _.findWhere(d, { co: c });
    },
    getPropertyType = function (id) {
        return mogiPropertyTypes.hash['c' + id];
    },
    getPropertyTypes = function (parentId) {
        var q = null;
        if (parentId == 0) { q = mogiPropertyTypes.data; }
        else { q = getPropertyType(parentId); if (q != null) q = q.c; }
        return (q == null ? null : _.filter(q, function (item) { return (item.p == parentId && item.Sale && item.Rent); }));
    },
    getPropertyTypeForSales = function (parentId) {
        var q = null;
        if (parentId == 0) { q = mogiPropertyTypes.data; }
        else { q = getPropertyType(parentId); if (q != null) q = q.c; }
        return (q == null ? null : _.where(q, { p: parentId, Sale: true }));
    },
    getPropertyTypeForRents = function (parentId) {
        var q = null;
        if (parentId == 0) { q = mogiPropertyTypes.data; }
        else { q = getPropertyType(parentId); if (q != null) q = q.c; }
        return (q == null ? null : _.where(q, { p: parentId, Rent: true }));
    },
     getPropertyTypeForProjects = function (parentId) {
         var q = null;
         if (parentId == 0) { q = mogiPropertyTypes.data; }
         else { q = getPropertyType(parentId); if (q != null) q = q.c; }
         return (q == null ? null : _.where(q, { p: parentId, Proj: true }));
     },
     getLegals = function () {
         var r = mogiDatas.Legal;
         if (r[0]['Id'] == null) {
             r.map(function (o) { o.Id = o.i; o.Name = o.n; });
         }
         return r;
     },
     getLegal = function (id) {
         var obj = getLegals();
         return _.find(obj, function (item) { return item.i == id; });
     },
     getCurrenCyForRent = function () {
         return _.where(mogiDatas.Currency, { r: true });
     },
     getCurrenCyForSales = function () {
         return _.where(mogiDatas.Currency, { s: true });
     },
     getCurrenCy = function (id) {
         var obj = mogiDatas.Currency;
         return _.find(obj, function (item) { return item.i == id; });
     },
    getDirections = function () {
        var r = mogiDatas.Direction;
        if (r[0]['Id'] == null) {
            r.map(function (o) { o.Id = o.i; o.Name = o.n; });
        }
        return r;
    },
    getDirection = function (id) {
        var obj = getDirections();
        return _.find(obj, function (item) { return item.i == id; });
    },
    getUserType = function (id) {
        var types = syscodeUtils.getUserTypes();

        var type = _.find(types, function (item) { return item.i == id; });
        if (type != void (0))
            return type.n;
        else {
            id = 68;
            type = _.find(types, function (item) { return item.i == id; });
            return type.n;
        }
        // return "";
    };
    return {
        getSysTables: getSysTables,
        getSysCode: getSysCode,
        syscodeUtils: syscodeUtils,
        getRewardTypes: getRewardTypes,
        GetCities: GetCities,
        GetCityById: GetCityById,
        GetCityByCode: GetCityByCode,
        getLevels: getLevels,
        getLevelById: getLevelById,
        getLevelByCode: getLevelByCode,
        getPropertyType: getPropertyType,
        getPropertyTypes: getPropertyTypes,
        getPropertyTypeForSales: getPropertyTypeForSales,
        getPropertyTypeForRents: getPropertyTypeForRents,
        getPropertyTypeForProjects: getPropertyTypeForProjects,
        currencyUtils: currencyUtils,
        getLegals: getLegals,
        getLegal: getLegal,
        getCurrenCyForRent: getCurrenCyForRent,
        getCurrenCyForSales: getCurrenCyForSales,
        getCurrenCy: getCurrenCy,
        getDirections: getDirections,
        getDirection: getDirection,
        filterUtils: filterUtils,
        getUserType: getUserType
    };
})(jQuery, _);