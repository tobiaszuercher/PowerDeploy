var powerdeployApp = angular.module('powerdeployApp', [
    'ngRoute',
    'powerdeployControllers'
]);

powerdeployApp.config(['$routeProvider',
    function($routeProvider) {
        $routeProvider.
            when('/settings', {
                templateUrl: 'js/settings/settings.html',
                controller: 'SettingsCtrl'
            }).
            otherwise({
                redirectTo: '/'
            });
    }]);