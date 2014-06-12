angular.module('powerdeploy.rest')
    .factory('settings', ['REST_API_URL', '$resource',
        function (REST_API_URL, $resource) {
            return $resource(REST_API_URL + 'settings/:Id', null, {
                'save': {
                    method: 'PUT'
                }
            });

    }])
    .factory('packages', ['REST_API_URL', '$resource',
        function (REST_API_URL, $resource) {
            return $resource(REST_API_URL + 'packages/:id', null, {
                'save': {
                    method: 'PUT'
                }
            });
        }])
    .factory('environments', ['REST_API_URL', '$resource',
        function (REST_API_URL, $resource) {
            return $resource(REST_API_URL + 'environments/:id/variables', null, {
                'save': {
                    method: 'PUT'
                }
            });
        }]
);