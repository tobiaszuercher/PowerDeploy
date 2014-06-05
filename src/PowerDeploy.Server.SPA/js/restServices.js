angular.module('powerdeploy.rest')
    .factory('settings', ['REST_API_URL', '$resource',
        function (REST_API_URL, $resource) {
            return $resource(REST_API_URL + 'settings/:Id', null, {
                'save': {
                    method: 'PUT'
                }
            });
  }
]);