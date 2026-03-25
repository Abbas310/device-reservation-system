var clientsApp = angular.module("clientsApp", ["ui.bootstrap", "sharedDirectives"])
    .config(function ($interpolateProvider) {
        $interpolateProvider.startSymbol('[[');
        $interpolateProvider.endSymbol(']]');
    });

clientsApp.controller("ClientsController", function ($scope, $http, $modal, $timeout) {

    $scope.successMessage = "";
    $scope.hasAvailablePhones = false;

    $scope.searchName = "";
    $scope.selectedType = "";
    $scope.clients = [];

    function refreshHasAvailablePhones() {
        return $http.get("/api/phonenumbers/available")
            .then(function (res) {
                $scope.hasAvailablePhones = (res.data && res.data.length > 0);
            })
            .catch(function () {
                $scope.hasAvailablePhones = false;
            });
    }

    function refreshClientReservationFlags(list) {
        angular.forEach(list, function (c) {
            c.HasActiveReservation = false;

            $http.get("/api/phonenumberreservations/active", { params: { clientId: c.Id } })
                .then(function (r) {
                    c.HasActiveReservation = (r.data && r.data.length > 0);
                })
                .catch(function () {
                    c.HasActiveReservation = false;
                });
        });
    }

    function loadAll() {
        return $http.get("/api/clients").then(function (res) {
            $scope.clients = res.data || [];
            refreshClientReservationFlags($scope.clients);
            return refreshHasAvailablePhones();
        });
    }

    loadAll();

    $scope.searchClients = function () {
        var s = ($scope.searchName || "").trim();
        var t = $scope.selectedType ? parseInt($scope.selectedType, 10) : null;

        $http.get("/api/clients", { params: { search: s, type: t } })
            .then(function (res) {
                $scope.clients = res.data || [];
                refreshClientReservationFlags($scope.clients);
                refreshHasAvailablePhones();
            });
    };

    $scope.openAddModal = function () {
        openClientModal({ Id: null, Name: "", Type: "", BirthDate: null, BirthDateText: null }, false);
    };

    $scope.openEditModal = function (c) {
        var copy = angular.copy(c);
        copy.Type = copy.Type ? parseInt(copy.Type, 10) : "";
        openClientModal(copy, true);
    };

    function showSuccess(msg) {
        $scope.successMessage = msg;
        $timeout(function () { $scope.successMessage = ""; }, 3000);
    }

    $scope.openReserveModal = function (c) {
        var modalInstance = $modal.open({
            templateUrl: "reservePhoneModal.html",
            controller: "ReservePhoneController",
            resolve: {
                client: function () { return angular.copy(c); }
            }
        });

        modalInstance.result.then(function (phoneNumberId) {

            $http.post("/api/phonenumberreservations/reserve", {
                ClientId: c.Id,
                PhoneNumberId: phoneNumberId
            }).then(function () {

                showSuccess("Phone number reserved successfully.");
                loadAll();

            }).catch(function (err) {
                console.error("Reserve failed:", err);
                alert("Reserve failed. Check console/network.");
            });

        });
    };

    $scope.openUnreserveModal = function (c) {
        var modalInstance = $modal.open({
            templateUrl: "unreservePhoneModal.html",
            controller: "UnreservePhoneController",
            resolve: {
                client: function () { return angular.copy(c); }
            }
        });

        modalInstance.result.then(function (reservationId) {

            $http.post("/api/phonenumberreservations/unreserve", {
                ReservationId: reservationId
            }).then(function () {

                showSuccess("Phone number unreserved successfully.");
                loadAll();

            }).catch(function (err) {
                console.error("Unreserve failed:", err);
                alert("Unreserve failed. Check console/network.");
            });

        });
    };

    function openClientModal(model, isEdit) {
        var modalInstance = $modal.open({
            templateUrl: "clientModal.html",
            controller: "ClientModalController",
            resolve: {
                client: function () { return model; },
                isEdit: function () { return isEdit; }
            }
        });

        modalInstance.result.then(function (result) {

            if (!isEdit) {
                $http.post("/api/clients", result).then(function () { loadAll(); });
            } else {
                $http.put("/api/clients/" + result.Id, result).then(function () { loadAll(); });
            }

        });
    }

    $scope.openDeleteModal = function (c) {
        var modalInstance = $modal.open({
            templateUrl: "deleteClientModal.html",
            controller: "DeleteClientController",
            resolve: {
                client: function () { return angular.copy(c); }
            }
        });

        modalInstance.result.then(function (id) {
            $http.delete("/api/clients/" + id)
                .then(function () { loadAll(); })
                .catch(function (err) {
                    var msg = "Delete failed.";

                    if (err && err.data) {
                        if (err.data.Message) msg = err.data.Message;
                        else if (typeof err.data === "string") {
                            msg = (err.data.trim().charAt(0) === "<")
                                ? "Cannot delete because it is in use."
                                : err.data;
                        }
                    }

                    alert(msg);
                    console.error("Delete failed:", err);
                });
        });
    };
});

/* ===== Add/Edit Client Modal ===== */
clientsApp.controller("ClientModalController", function ($scope, $modalInstance, client, isEdit) {

    $scope.client = client;
    $scope.isEdit = isEdit;

    $scope.form = {
        birthDateModel: client.BirthDate ? new Date(client.BirthDate) : null
    };

    $scope.$watch("client.Type", function (t) {
        var typeNum = t ? parseInt(t, 10) : null;
        if (typeNum === 2) {
            $scope.form.birthDateModel = null;
        }
    });

    $scope.save = function () {
        $scope.client.Name = ($scope.client.Name || "").trim();
        if (!$scope.client.Name) return;

        if (!$scope.client.Type) return; // mandatory

        $scope.client.Type = parseInt($scope.client.Type, 10);

        if ($scope.client.Type === 1) {
            if (!$scope.form.birthDateModel) return; // mandatory

            var d = ($scope.form.birthDateModel instanceof Date)
                ? $scope.form.birthDateModel
                : new Date($scope.form.birthDateModel);

            $scope.client.BirthDateText = d.toISOString().substring(0, 10);
            $scope.client.BirthDate = null;
        } else {
            $scope.client.BirthDateText = null;
            $scope.client.BirthDate = null;
        }

        $modalInstance.close($scope.client);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
});

/* ===== Delete Client Modal ===== */
clientsApp.controller("DeleteClientController", function ($scope, $modalInstance, client) {
    $scope.client = client;

    $scope.confirm = function () { $modalInstance.close(client.Id); };
    $scope.cancel = function () { $modalInstance.dismiss("cancel"); };
});

/* ===== Reserve Modal ===== */
clientsApp.controller("ReservePhoneController", function ($scope, $modalInstance, client) {
    $scope.client = client;
    $scope.reservation = { phoneNumberId: null };

    $scope.save = function () { $modalInstance.close($scope.reservation.phoneNumberId); };
    $scope.cancel = function () { $modalInstance.dismiss("cancel"); };
});

/* ===== Unreserve Modal ===== */
clientsApp.controller("UnreservePhoneController", function ($scope, $modalInstance, $http, client) {
    $scope.client = client;
    $scope.reservations = [];
    $scope.selectedReservationId = null;

    $http.get("/api/phonenumberreservations/active", { params: { clientId: client.Id } })
        .then(function (res) { $scope.reservations = res.data || []; });

    $scope.confirm = function () { $modalInstance.close($scope.selectedReservationId); };
    $scope.cancel = function () { $modalInstance.dismiss("cancel"); };
});