pdApp.factory('settings', [
  '$resource', function ($resource) {
    return $resource('http://localhost:81/settings/:Id', null, {
      'save': { method: 'PUT' }
    });
  }
]);