var clientsReportApp = angular.module("clientsReportApp", ["ui.bootstrap", "sharedDirectives"])
    .config(function ($interpolateProvider) {
        $interpolateProvider.startSymbol('[[');
        $interpolateProvider.endSymbol(']]');
    });

clientsReportApp.controller("ClientsReportController", function ($scope, $http) {

    $scope.selectedType = ""; // "" => no filter (show all)
    $scope.rows = [];

    function load(type) {
        var t = type ? parseInt(type, 10) : null;

        $http.get("/api/clientreports", { params: { type: t } })
            .then(function (res) {
                $scope.rows = res.data || [];
            });
    }

    // initial load => show all
    load(null);

    $scope.search = function () {
        load($scope.selectedType);
    };
});