angular.module('powerdeploy')
    .controller('environmentCtrl', ['$routeParams', '$scope', 'environments',
        function ($routeParams, $scope, environments) {

            $scope.environment = environments.get({ id: $routeParams.name }, angular.noop, function () {
                toastr.error('server communication failed');
            });
        }]);