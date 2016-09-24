(function () {
    'use strict';

    angular
        .module('detailsApp')
        .controller('progressController', progressController);

    progressController.$inject = ['$scope', '$interval', 'progressFactory']; 

    function progressController($scope, $interval, progressFactory) {
        $scope.projectId = undefined;
        $scope.realtimeInfo = undefined;

        var lastSynch = moment('1990-01-01');
        var lastLocalUpdate = moment('1990-01-01');

        var synchPeriod = moment.duration(20, 'seconds');
        var updatePeriod = moment.duration(1, 'seconds');

        var updaterPromise;

        $scope.init = function (projectId) {
            $scope.projectId = projectId;
            synchronizeRealtimeInfo();
        };

        // Synchronize realtime info with server
        function synchronizeRealtimeInfo() {
            progressFactory.getProgress($scope.projectId).then(function (data) {
                $scope.realtimeInfo = data.data;
                if ($scope.realtimeInfo.isRunning)
                {
                    if (!updaterPromise)
                        startUpdate();
                }
                else
                {
                    if (updaterPromise)
                        stopUpdate();
                }
                lastLocalUpdate = lastSynch = moment();
            });
        };

        // Update realtime info without synchronization with server 
        function updateRealtimeInfoLocal() {
            if ($scope.realtimeInfo.isRunning) {
                var curExcecuted = moment.duration($scope.realtimeInfo.progress.excecuted);
                $scope.realtimeInfo.progress.excecuted = curExcecuted.add(moment() - lastLocalUpdate);
            }
            lastLocalUpdate = moment();
        };

        function updateRealtimeInfo() {
            var now = moment();
            if (now - lastSynch > synchPeriod)
                synchronizeRealtimeInfo();
            else
                updateRealtimeInfoLocal();
        };

        function startUpdate() {
            if (updaterPromise)
                stopUpdate();
            updaterPromise = $interval(updateRealtimeInfo, updatePeriod);
        };

        function stopUpdate() {
            if (updaterPromise) {
                $interval.cancel(updaterPromise);
            }
        };

        $scope.start = function () {
            startUpdate();
            progressFactory.start($scope.projectId).then(function () {
                synchronizeRealtimeInfo();
            });
        };

        $scope.stop = function () {
            stopUpdate();
            progressFactory.stop($scope.projectId).then(function () {
                synchronizeRealtimeInfo();
            });
        };

        $scope.formatDuration = function (duration) {
            return moment.duration(duration).format("hh:mm:ss", { trim: false });
        };
    }
})();
