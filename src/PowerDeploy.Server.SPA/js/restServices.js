pdApp.factory('Packages', [
  '$resource', function ($resource) {
    return $resource('/packages/:Id', null, {
      'update': { method: 'POST' }
    });
  }
]);