var pdApp = angular.module('pdApp', ['ngRoute', 'ngResource']);

pdApp.config(['$routeProvider',
    function($routeProvider) {
        $routeProvider.
            when('/settings', {
                templateUrl: 'js/settings/settings.html',
                controller: 'settingsCtrl'
            }).
            when('/dashboard', {
                templateUrl: 'js/dashboard/dashboard.html',
                controller: 'dashboardCtrl'
            }).
            otherwise({
                redirectTo: '/dashboard'
            });
    }]);

pdApp.controller('SettingsCtrl', ['$scope', '$routeParams',
    function($scope, $routeParams) {
    }
]);

pdApp.controller('dashboardCtrl', ['$scope', '$routeParams',
    function($scope, $routeParams) {
    }
]);

pdApp.filter("StripRavenDbIdPrefix", [
    function () {
      var result = function (id) {
        var index = id.indexOf("/") + 1;
        return id.substr(index, id.length - index);
      };

      return result;
    }
]);

pdApp.filter("boolConverter", [
    function () {
      var result = function (valueToCheck, trueValue, falseValue) {
        if (valueToCheck) {
          return trueValue;
        }

        return falseValue;
      };

      return result;
    }
]);

pdApp.filter("formatdate", [function () {
  var result = function (date, formatstring) {
    if (formatstring === null) {
      formatstring = "DD MMM YYYY";
    }
    return moment(date).format(formatstring);
  };

  return result;
}]);

pdApp.filter("convertdate", [function () {
  var result = function (date) {
    return moment(date).toDate();
  };

  return result;
}]);
pdApp.factory('Packages', [
  '$resource', function ($resource) {
    return $resource('/packages/:Id', null, {
      'update': { method: 'POST' }
    });
  }
]);