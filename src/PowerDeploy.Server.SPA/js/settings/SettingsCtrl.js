pdApp.controller('settingsCtrl', ['$scope', 'settings', function ($scope, settings) {
  $scope.versionControlSystems = [ { name: 'Git' }, { name: 'Tfs'} ]
    
  $scope.settings = settings.get({}, function (currentSettings) {
      console.log('work dir: ' + currentSettings.WorkDir);
      console.log('vcs: ' + currentSettings.VersionControlSystem);
  });
}]);
