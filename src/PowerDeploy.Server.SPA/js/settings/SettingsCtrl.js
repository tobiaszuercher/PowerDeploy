angular.module('powerdeploy')
       .controller('settingsCtrl', ['$scope', 'settings', function ($scope, settings) {
           
  $scope.versionControlSystems = [ 'Git', 'Tfs' ]
    
  $scope.settings = settings.get({}, function (currentSettings) {
      console.log('work dir: ' + currentSettings.WorkDir);
      console.log('vcs: ' + currentSettings.VersionControlSystem);
  });
           
}]);
