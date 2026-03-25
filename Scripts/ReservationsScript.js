var resApp = angular.module("resApp", ["sharedDirectives"])
    .config(function ($interpolateProvider) {
        $interpolateProvider.startSymbol('[[');
        $interpolateProvider.endSymbol(']]');
    });

resApp.controller("ReservationsController", function ($scope, $http) {

    $scope.reservations = [];
    $scope.clients = [];

    $scope.selectedClientId = "";
    $scope.selectedPhoneNumberId = "";

    function loadClients() {
        return $http.get("/api/clients").then(function (res) {
            $scope.clients = res.data || [];
        });
    }

    function loadAll() {
        return $http.get("/api/phonenumberreservations").then(function (res) {
            $scope.reservations = res.data || [];
        });
    }

    loadClients().then(loadAll);

    $scope.search = function () {
        var cid = $scope.selectedClientId ? parseInt($scope.selectedClientId, 10) : null;
        var pid = $scope.selectedPhoneNumberId ? parseInt($scope.selectedPhoneNumberId, 10) : null;

        $http.get("/api/phonenumberreservations", { params: { clientId: cid, phoneNumberId: pid } })
            .then(function (res) { $scope.reservations = res.data || []; });
    };

    $scope.reset = function () {
        $scope.selectedClientId = "";
        $scope.selectedPhoneNumberId = "";
        loadAll();
    };
});
