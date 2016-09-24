(function () {
    'use strict';

    angular
        .module('detailsApp')
        .factory('progressFactory', progressFactory);

    progressFactory.$inject = ['$http'];

    function progressFactory($http) {
        var service = {
            getProgress: getProgress,
            start: start,
            stop: stop
        };

        return service;

        function getProgress(projectId) {
            return $http.get('/api/progress/current/' + projectId);
        }

        function start(projectId) {
            return $http.get('/api/timers/start/' + projectId);
        }

        function stop(projectId) {
            return $http.get('/api/timers/stop/' + projectId);
        }
    }
})();