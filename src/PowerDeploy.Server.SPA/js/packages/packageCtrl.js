angular.module('powerdeploy')
    .controller('packageCtrl', ['$scope', 'packages',
        function ($scope, packages) {

            $scope.packages = packages.query({}, function () {}, function () {
                toastr.error('server communication failed');
            });
        }]);