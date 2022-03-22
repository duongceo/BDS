function messageService($rootScope, $http) {
	var $rs = $rootScope;
	this.Messages = $rs.Profile.Messages;
	this.Data = [];
	this.Key = 'mogimsg';
	this.IsAuth = false;
	this.Init = function (model, auth) {
		data = model;
		isAuth = auth;

		this.Messages.Total = this.Data.length;
		this.IsReadAll();
	};
	this.GetData = function (c) {
		$http({ method: 'GET', url: mogiRoutes.Profile.getMessage, data: { code: c } }).then(function (res) {
			$rs.Profile.Messages = res.data;
			this.Messages = res.data;
		});
	};
	this.IsReadAll = function () {
		var v = $cache.get(this.Key);
		if (v === null) {
			this.Messages.ShowBadge = this.Data.length > 0;
			return false;
		}
		v = JSON.parse(v);
		var readItems = this.Data.filter(function (item) {
			return v.msg[item.MessageId] === true;
		});

		this.Messages.ShowBadge = readItems.length !== this.Data.length;
	};
	this.OnRead = function (id) {
		if (this.IsAuth) {
			var u = mogiRoutes.Profile.ReadMessage + '?id=' + id;
			$http({ method: 'GET', url: u }).then(function (res) {});
		}
		var d = $cache.get(this.Key);
		if (d !== null) {
			d = JSON.parse(d);
			if (d.msg[id]) {
				return;
			}
			d.msg[id] = true;
		} else {
			d = { msg: {} };
			d.msg[id] = true;
		}
		$cache.set(this.Key, JSON.stringify(d));

		this.IsReadAll();
	};
}
messageService.$inject = ['$rootScope','$http'];
mogiApp.service('messageService', messageService);

//(function () {
//    if (MOGI != null && MOGI.Messages !=null) {
//        messageService.Init(MOGI.Messages, MOGI.Auth);
//    }
//}())
