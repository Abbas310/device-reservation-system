var devicesReportApp = angular.module("devicesReportApp", ["ui.bootstrap", "sharedDirectives"])
    .config(function ($interpolateProvider) {
        $interpolateProvider.startSymbol('[[');
        $interpolateProvider.endSymbol(']]');
    });

devicesReportApp.controller("DevicesReportController", function ($scope, $http) {

    $scope.selectedDeviceId = "";
    $scope.selectedStatus = ""; // "reserved" | "unreserved" | ""
    $scope.rows = [];

    $scope.search = function () {
        var devId = $scope.selectedDeviceId ? parseInt($scope.selectedDeviceId, 10) : null;
        var status = ($scope.selectedStatus || "").trim();

        $http.get("/api/devicereports", { params: { deviceId: devId, status: status } })
            .then(function (res) {
                $scope.rows = res.data || [];
            });
    };

    // optional initial load: show nothing until Search is pressed
    $scope.rows = [];
});