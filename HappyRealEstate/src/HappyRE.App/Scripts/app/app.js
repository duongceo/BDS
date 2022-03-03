
//var commonModule = angular.module('common', ['ngRoute','ui.bootstrap']);

// non-SPA views will use Angular controllers created on the appMainModule

//var appMainModule = angular.module('appMain', []);
var appMainModule = angular.module('appMain', ['ngSanitize', 'ngStorage'])
     .directive('ngCtrlEnter', function () {
         return function (scope, element, attrs) {
             element.bind("keydown keypress", function (event) {
                 if (event.which === 13 && event.ctrlKey) {
                     scope.$apply(function () {
                         scope.$eval(attrs.ngCtrlEnter);
                     });

                     event.preventDefault();
                 }
             });
         }
     })
    .directive('ngEnter', function() {
        return function(scope, element, attrs) {
            element.bind("keydown keypress", function(event) {
                if (event.which === 13) {
                    scope.$apply(function() {
                        scope.$eval(attrs.ngEnter);
                    });

                    event.preventDefault();
                }
            });
        }
    })   
    .directive('repeatDone', function() {
        return function(scope, element, attrs) {
            if (scope.$last) { // all are rendered            
                scope.$emit('LastElem');
                scope.$eval(attrs.repeatDone);
            }
        }
    })
    .directive('renderDone', function() {
        return function(scope, element, attrs) {
            scope.$on('LastElem', function(event) {
                scope.$eval(attrs.renderDone);
            });
        };
    });

//
appMainModule.factory('viewModelHelper', viewController);
viewController.$inject = ["$http"];

function viewController($http) {    
    return ChildFashion.viewModelHelper($http);
};

appMainModule.factory('cookieFactory', cookieFactoryController);

function cookieFactoryController() {
    $.cookie.json = true;

    return {        
        getCookie: function (name) {
            return $.cookie(name);
        },

        getAllCookies: function() {
            return $.cookie();
        },

        setCookie: function(name, value) {
            return $.cookie(name, value, { expires: 365, path: '/' });
        },

        deleteCookie: function(name) {
            return $.removeCookie(name);
        }
    };
};


//angular.module('yourModuleName').directive('ngEnter', function () {
//    return function (scope, element, attrs) {
//        element.bind("keydown keypress", function (event) {
//            if (event.which === 13) {
//                scope.$apply(function () {
//                    scope.$eval(attrs.ngEnter, { 'event': event });
//                });
//
//                event.preventDefault();
//            }
//        });
//    };
//});

//
//// services attached to the commonModule will be available to all other Angular modules (above)
//
//commonModule.directive('jqdatepicker', function () {
//    return {
//        restrict: 'A',
//        link: function (scope, element, attrs) {
//            element.datepicker({
//                changeYear: "true",
//                changeMonth: true,
//                dateFormat: 'mm-dd-yyyy',
//                showOtherMonths: true,
//                showButtonPanel: true,
//                onClose: function (selectedDate) {
//                    scope.dateTtime = selectedDate + "T" + scope.time;
//                    scope.$apply();
//                }
//            });
//        }
//    };
//})
//
//commonModule.factory('viewModelHelper', function ($http, $q) {
//    return ChildFashion.viewModelHelper($http, $q);
//});

//(function (cr) {
//    var initialId;
//    cr.initialId = initialId;
//}(window.ChildFashion));
//
//(function (cr) {
//    var initialState;
//    cr.initialState = initialState;
//}(window.ChildFashion));

//(function (cr) {
//    var mustEqual = function (val, other) {
//        return val == other();
//    }
//    cr.mustEqual = mustEqual;
//}(window.ChildFashion));
//
(function (cf) {   
    var viewModelHelper = function($http) {

        var self = this;

        self.modelIsValid = true;
        self.modelErrors = [];
        self.isLoading = false;
//        self.modelIsValid = ko.observable(true);
//        self.modelErrors = ko.observableArray();
//        self.isLoading = ko.observable(false);

        self.statePopped = false;
        self.stateInfo = {};

        self.apiGet = function(uri, data, success, failure, always) {
            self.isLoading = true;
            self.modelIsValid = true;
            $http.get(ChildFashion.rootPath + uri, data)
                .then(function(result) {
                    success(result);
                    if (always != null)
                        always();
                    self.isLoading = false;
                }, function(result) {
                    if (failure == null) {
                        if (result.status != 400)
                            self.modelErrors = [result.status + ':' + result.statusText + ' - ' + result.data.Message];
                        else
                            self.modelErrors = [result.data.Message];
                        self.modelIsValid = false;
                    } else
                        failure(result);
                    if (always != null)
                        always();
                    self.isLoading = false;
                });
        };
        self.apiPost = function(uri, data, success, failure, always) {
            self.isLoading = true;
            self.modelIsValid = true;
            $http.post(ChildFashion.rootPath + uri, data)
                .then(function(result) {
                    success(result);
                    if (always != null)
                        always();
                    self.isLoading = false;
                }, function(result) {
                    if (failure == null) {
                        if (result.status != 400)
                            self.modelErrors = [result.status + ':' + result.statusText + ' - ' + result.data.Message];
                        else
                            self.modelErrors = [result.data.Message];
                        self.modelIsValid = false;
                    } else
                        failure(result);
                    if (always != null)
                        always();
                    isLoading = false;
                });
        };
        self.isUndefinedOrNull = function(val) {
            return angular.isUndefined(val) || val == null;
        };
        self.defaultValueIfNull = function(val, d) {
            if (self.isUndefinedOrNull(val)) return d;

            return val;
        };

        self.PropertyRule = function(propertyName, rules) {
            var self = this;
            self.PropertyName = propertyName;
            self.Rules = rules;
        };

        self.validateModel = function (model, propertyRules) {            
            var errors = [];
            var props = Object.keys(model);
            for (var i = 0; i < props.length; i++) {
                var prop = props[i];
                for (var j = 0; j < propertyRules.length; j++) {
                    var propertyRule = propertyRules[j];
                    if (prop == propertyRule.PropertyName) {
                        var rules = propertyRule.Rules;
                        if (rules.hasOwnProperty('required')) {
                            if (model[prop].trim() == '') {
                                errors.push(getMessage(rules.required));
                            }
                        }
                        if (rules.hasOwnProperty('pattern')) {
                            var regExp = new RegExp(rules.pattern.value);
                            if (regExp.exec(model[prop].trim()) == null) {
                                errors.push(getMessage(rules.pattern));
                            }
                        }
                        if (rules.hasOwnProperty('minLength')) {
                            var minLength = rules.minLength.value;
                            if (model[prop].trim().length < minLength) {
                                errors.push(getMessage(rules.minLength));
                            }
                        }
                    }
                }
            }

            model['errors'] = errors;
            model['isValid'] = (errors.length == 0);
        };
        var getMessage = function(rule) {
            var message = '';
            if (rule.hasOwnProperty('message'))
                message = rule.message;
            else
                message = prop + ' is invalid.';
            return message;
        };

        //self.pushUrlState = function (code, title, id, url) {
        //    self.stateInfo = { State: { Code: code, Id: id }, Title: title, Url: ChildFashion.rootPath + url };
        //}

        //self.handleUrlState = function (initialState) {
        //    if (!self.statePopped) {
        //        if (initialState != '') {
        //            history.replaceState(self.stateInfo.State, self.stateInfo.Title, self.stateInfo.Url);
        //            // we're past the initial nav state so from here on everything should push
        //            initialState = '';
        //        }
        //        else {
        //            history.pushState(self.stateInfo.State, self.stateInfo.Title, self.stateInfo.Url);
        //        }
        //    }
        //    else
        //        self.statePopped = false; // only actual popping of state should set this to true

        //    return initialState;
        //}

        return this;
    };
    cf.viewModelHelper = viewModelHelper;
}(window.ChildFashion));