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
            when('/package', {
               templateUrl: 'js/packages/package.html',
                controller: 'packageCtrl'
            }).
            when('/package-overview', {
                templateUrl: 'js/packages/packageOverview.html',
                controller: 'packageCtrl'
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