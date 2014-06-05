angular.module('powerdeploy')
    .controller('settingsCtrl', ['$scope', 'settings',
        function ($scope, settings) {

            $scope.versionControlSystems = ['Git', 'Tfs']

            $scope.settings = settings.get();

            $scope.submit = function() {
                $scope.settings.$save(function (u, putResponseHeaders) {
                    // success callback
                });
            };

}]);