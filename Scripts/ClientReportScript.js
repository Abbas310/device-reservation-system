var clientsReportApp = angular.module("clientsReportApp", ["ui.bootstrap", "sharedDirectives"])
    .config(function ($interpolateProvider) {
        $interpolateProvider.startSymbol('[[');
        $interpolateProvider.endSymbol(']]');
    });

clientsReportApp.controller("ClientsReportController", function ($scope, $http) {

    $scope.selectedType = ""; 
    $scope.rows = [];

    function load(type) {
        var t = type ? parseInt(type, 10) : null;

        $http.get("/api/clientreports", { params: { type: t } })
            .then(function (res) {
                $scope.rows = res.data || [];
            });
    }

    
    load(null);

    $scope.search = function () {
        load($scope.selectedType);
    };
});