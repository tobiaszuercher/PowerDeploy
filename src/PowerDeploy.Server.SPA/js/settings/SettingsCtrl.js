angular.module('powerdeploy')
    .controller('settingsCtrl', ['$scope', 'settings',
        function ($scope, settings) {

            $scope.settingsBackup = {};
            $scope.versionControlSystems = ['Git', 'Tfs']

            $scope.settings = settings.get({}, function () {
                    $scope.settingsBackup = angular.copy($scope.settings);
                }, function () {
                    toastr.error('server communication failed');
                });

            $scope.submit = function () {
                $scope.settings.$save($scope.onSaveSuccess, $scope.onSaveError);
            };

            $scope.reset = function () {
                $scope.settings = angular.copy($scope.settingsBackup);
            }

            $scope.onSaveSuccess = function (settings, responseHeaders) {
                toastr.info('Settings saved!');
            };

            $scope.onSaveError = function () {
                toastr.error('Save failed!');
            }
}]);