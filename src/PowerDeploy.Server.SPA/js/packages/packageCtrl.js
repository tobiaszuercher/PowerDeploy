angular.module('powerdeploy')
    .controller('packageCtrl', ['$routeParams', '$scope', 'packages',
        function ($routeParams, $scope, packages) {

            $scope.package = packages.get({ id: $routeParams.nugetId }, angular.noop, function () {
                toastr.error('server communication failed');
            });
        }]);