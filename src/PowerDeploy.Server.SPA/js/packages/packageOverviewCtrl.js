angular.module('powerdeploy')
    .controller('packageOverviewCtrl', ['$scope', 'packages',
        function ($scope, packages) {

            $scope.packages = packages.query({}, function () {}, function () {
                toastr.error('server communication failed');
            });
        }]);