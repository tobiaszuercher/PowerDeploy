'use strict';

angular.module('powerdeploy.rest', ['powerdeploy.config', 'ngResource']);

angular.module('powerdeploy', ['powerdeploy.config', 'powerdeploy.rest', 'ngRoute'])

.config(['$routeProvider',
    function ($routeProvider) {
        $routeProvider.
        when('/settings', {
            templateUrl: 'js/settings/settings.html',
            controller: 'settingsCtrl'
        }).
        when('/dashboard', {
            templateUrl: 'js/dashboard/dashboard.html',
            controller: 'dashboardCtrl'
        }).
        when('/package', {
            templateUrl: 'js/packages/package.html',
            controller: 'packageCtrl'
        }).
        otherwise({
            redirectTo: '/dashboard'
        });
    }]);

//pdApp.controller('dashboardCtrl', ['$scope', '$routeParams',
//    function ($scope, $routeParams) {}
//]);