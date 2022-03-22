function suggest($http) {
    var cache = {};

    return {
        Map: Map,
        Map_MapObject: Map_MapObject,
        MapField: MapField,
        Agent: Agent,
        Project: Project,
        GetSuggestion: GetSuggestion,
        SuggestionField: SuggestionField,
        Street: Street
    };

    function MapField() {
        return {
            MapId: 0, Name: '', ReferTypeId: 0, ReferId: 0, CityId: 0, DistrictId: 0, WardId: 0, StreetId: 0, ProjectId: 0, PlaceId: 0, SEOId: 0, CodeUrl: "",
            FullName: function (v) {
                return v.replace(/<.?b>/g, '');
            },
            Url: function () {
                return this.CodeUrl + '-mid' + this.MapId;
            }
        };
    }

    function Map_MapObject(q, resp) {
        var res = [];
        var d1 = resp.data.suggest.startwith_suggester[q].suggestions;
        var d2 = resp.data.suggest.contain_suggester[q].suggestions;
        var f = ["MapId", "ReferTypeId", "ReferId", "CityId", "DistrictId", "WardId", "StreetId", "ProjectId", "PlaceId", "SEOId", "CodeUrl"];
        var h = {};
        angular.forEach(d1, function (v) {
            var ids = JSON.parse(v.payload);
            var o = new MapField();
            o.Name = v.term;
            for (var i = 0; i < f.length; i++) {
                o[f[i]] = ids[i];
            }
            if (h[o.MapId]) return;
            h[o.MapId] = o;
            res.push(o);
        });
        angular.forEach(d2, function (v) {
            if (res.length >= 10) return;
            var ids = JSON.parse(v.payload);
            var o = new MapField();
            o.Name = v.term;
            for (var i = 0; i < f.length; i++) {
                o[f[i]] = ids[i];
            }
            if (h[o.MapId]) return;
            h[o.MapId] = o;
            res.push(o);
        });

        return res;
    }

    function Map(q, callback) {
        q = q.toLowerCase();
        var res = cache[q];
        if (res) {
            return res; // cache
        }
        return $http({ method: 'GET', url: mogiRoutes.Common.Suggest, params: { q: q }, responseType: 'json' }).then(
            function (resp) {
                res = Map_MapObject(q, resp);
                cache[q] = res;
                if (callback) callback(res[0]);
                return res;
            },
            function (resp) {
                cache[q] = [];
                return [];
            });
    }

    function Street(q, callback) {
        q = q.toLowerCase();
        var res = cache[q];
        if (res) {
            return res; // cache
        }
        return $http({ method: 'GET', url: mogiRoutes.Common.Suggest_Street, params: { q: q }, responseType: 'json' }).then(
            function (resp) {
                res = Map_MapObject(q, resp);
                cache[q] = res;
                if (callback) callback(res[0]);
                return res;
            },
            function (resp) {
                cache[q] = [];
                return [];
            });
    }


    function Agent(q, callback) {
        q = q.toLowerCase();
        var res = cache[q];
        if (res) {
            return res; // cache
        }
        return $http({ method: 'GET', url: mogiRoutes.Common.Suggest_Agent, params: { q: q }, responseType: 'json' }).then(
            function (resp) {
                res = Agent_MapObject(q, resp);
                cache[q] = res;
                if (callback) callback(res[0]);
                return res;
            },
            function (resp) {
                cache[q] = [];
                return [];
            });
    }

    function AgentField() {
        return {
            Id: '', Name: '',
            FullName: function (v) {
                return v.replace(/<.?b>/g, '');
            }
        };
    }
    function Agent_MapObject(q, resp) {
        var res = [];
        var d1 = resp.data.suggest.startwith_suggester[q].suggestions;
        var d2 = resp.data.suggest.contain_suggester[q].suggestions;
        var h = {};
        angular.forEach(d1, function (v) {
            var o = new AgentField();
            o.Id = v.payload;
            o.Name = v.term;
            if (h[o.Id]) return;
            h[o.Id] = o;
            res.push(o);
        });
        angular.forEach(d2, function (v) {
            if (res.length >= 10) return;
            var o = new AgentField();
            o.Id = v.payload;
            o.Name = v.term;
            if (h[o.Id]) return;
            h[o.Id] = o;
            res.push(o);
        });
        return res;
    }

    function Project(q, callback) {
        q = q.toLowerCase();
        var res = cache[q];
        if (res) {
            return res; // cache
        }
        return $http({ method: 'GET', url: mogiRoutes.Common.Suggest_Project, params: { q: q }, responseType: 'json' }).then(
            function (resp) {
                res = Project_MapObject(q, resp);
                cache[q] = res;
                if (callback) callback(res[0]);
                return res;
            },
            function (resp) {
                cache[q] = [];
                return [];
            });
    }

    function ProjectField() {
        return {
            Id: '', Name: '',
            FullName: function (v) {
                return v.replace(/<.?b>/g, '');
            }
        };
    }
    function Project_MapObject(q, resp) {
        var res = [];
        var d1 = resp.data.suggest.startwith_suggester[q].suggestions;
        var d2 = resp.data.suggest.contain_suggester[q].suggestions;
        var h = {};
        angular.forEach(d1, function (v) {
            var o = new ProjectField();
            o.Id = v.payload;
            o.Name = v.term;
            if (h[o.Id]) return;
            h[o.Id] = o;
            res.push(o);
        });
        angular.forEach(d2, function (v) {
            if (res.length >= 10) return;
            var o = new ProjectField();
            o.Id = v.payload;
            o.Name = v.term;
            if (h[o.Id]) return;
            h[o.Id] = o;
            res.push(o);
        });
        return res;
    }

    function SuggestionField() {
		return {
			Name: '',
			Type: 0,
			Id: 0,
			Url: '',
			FullName: function (v) {
				return v.replace(/<.?b>/g, '');
			}
		};
    }

    function SuggestionMapping(q, resp) {
        var res = [];
        var d1 = resp.data.suggest.startwith_suggester[q].suggestions;
        var d2 = resp.data.suggest.contain_suggester[q].suggestions;
		var f = ["Type", "Id", "Url"];

        var h = {};
        angular.forEach(d1, function (v) {
            var ids = JSON.parse(v.payload);
            var o = new SuggestionField();
            o.Name = v.term;
            for (var i = 0; i < f.length; i++) {
                o[f[i]] = ids[i];
            }
            if (h[o.Id]) return;
            h[o.Id] = o;
            res.push(o);
        });
        angular.forEach(d2, function (v) {
            if (res.length >= 10) return;
            var ids = JSON.parse(v.payload);
            var o = new SuggestionField();
            o.Name = v.term;
            for (var i = 0; i < f.length; i++) {
                o[f[i]] = ids[i];
            }
            if (h[o.Id]) return;
            h[o.Id] = o;
            res.push(o);
        });
        console.log(res);
        return res;
    }

    function GetSuggestion(q, url) {
        q = q.toLowerCase();
        var res = cache[q];
        if (res) {
            return res; // cache
        }
        return $http({ method: 'GET', url: url, params: { q: q }, responseType: 'json' }).then(
            function (resp) {
                console.log(resp.data);
                res = SuggestionMapping(q, resp);
                cache[q] = res;
                return res;
            },
            function (resp) {
                cache[q] = [];
                return [];
            });
    }
}
suggest.$inject = ["$http"];
mogiApp.factory('suggest', suggest);