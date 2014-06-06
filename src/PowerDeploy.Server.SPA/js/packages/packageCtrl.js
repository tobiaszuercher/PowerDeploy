angular.module('powerdeploy')
angular.module('powerdeploy')
    .controller('packageCtrl', ['$routeParams', '$scope', 'packages',
        function ($routeParams, $scope, packages) {

            $scope.package = packages.get({ id: $routeParams.nugetId }, function () {
                toastr.success('yay');
            }, function () {
                toastr.error('server communication failed');
            });
        }]);