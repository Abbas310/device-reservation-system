var phoneApp = angular.module("phoneApp", ["ui.bootstrap", "sharedDirectives"])
    .config(function ($interpolateProvider) {
        $interpolateProvider.startSymbol('[[');
        $interpolateProvider.endSymbol(']]');
    });

phoneApp.controller("PhoneController", function ($scope, $http, $modal) {

    $scope.phones = [];
    $scope.searchNumber = "";
    $scope.selectedDeviceId = "";

    function loadAll() {
        return $http.get("/api/phonenumbers").then(function (res) {
            $scope.phones = res.data;
        });
    }

    loadAll();

    $scope.search = function () {
        var num = ($scope.searchNumber || "").trim();

        // if empty => null => API returns all
        var dev = $scope.selectedDeviceId ? parseInt($scope.selectedDeviceId, 10) : null;

        $http.get("/api/phonenumbers", { params: { number: num, deviceId: dev } })
            .then(function (res) { $scope.phones = res.data; });
    };

    $scope.openAdd = function () {
        openModal({ Id: null, Number: "", DeviceId: "" }, false);
    };

    $scope.openEdit = function (p) {
        // ensure edit mode has numeric id
        var copy = angular.copy(p);
        copy.DeviceId = copy.DeviceId ? parseInt(copy.DeviceId, 10) : "";
        openModal(copy, true);
    };

    function openModal(model, isEdit) {
        var modalInstance = $modal.open({
            templateUrl: "phoneModal.html",
            controller: "PhoneModalController",
            resolve: {
                phone: function () { return model; },
                isEdit: function () { return isEdit; }
            }
        });

        modalInstance.result.then(function (result) {
            var payload = {
                Number: (result.Number || "").trim(),
                DeviceId: parseInt(result.DeviceId, 10)
            };

            if (!payload.Number) return;

            if (!isEdit) {
                $http.post("/api/phonenumbers", payload)
                    .then(function () { loadAll(); });
            } else {
                $http.put("/api/phonenumbers/" + result.Id, payload)
                    .then(function () { loadAll(); });
            }
        });
    }

    $scope.openDelete = function (p) {
        var modalInstance = $modal.open({
            templateUrl: "deletePhoneModal.html",
            controller: "DeletePhoneController",
            resolve: { phone: function () { return angular.copy(p); } }
        });

        modalInstance.result.then(function (id) {
            $http.delete("...")
                .then(function () { loadAll(); })
                .catch(function (err) {
                    var msg = "Delete failed.";

                    if (err && err.data) {
                        if (err.data.Message) {
                            msg = err.data.Message;                 // JSON { Message: "..." }
                        } else if (typeof err.data === "string") {
                            // If it's HTML, don't show it
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

phoneApp.controller("PhoneModalController", function ($scope, $modalInstance, phone, isEdit) {
    $scope.phone = phone;
    $scope.isEdit = isEdit;

    // IMPORTANT: do NOT default to first device.
    // For Add, DeviceId stays empty until user selects.
    if (!$scope.phone.DeviceId) {
        $scope.phone.DeviceId = "";
    }

    $scope.save = function () {
        $scope.phone.Number = ($scope.phone.Number || "").trim();
        if (!$scope.phone.Number) return;

        // mandatory device selection
        if (!$scope.phone.DeviceId) return;

        $modalInstance.close($scope.phone);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
});

phoneApp.controller("DeletePhoneController", function ($scope, $modalInstance, phone) {
    $scope.phone = phone;

    $scope.confirm = function () {
        $modalInstance.close(phone.Id);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
});
