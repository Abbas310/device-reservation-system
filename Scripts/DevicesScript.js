var app = angular.module("deviceApp", ["ui.bootstrap"])
    .config(function ($interpolateProvider) {
        $interpolateProvider.startSymbol('[[');
        $interpolateProvider.endSymbol(']]');
    });

app.controller("DeviceController", function ($scope, $http, $modal) {

    $scope.deviceName = "";
    $scope.filteredDevices = [];

    function loadAll() {
        return $http.get("/api/devices").then(function (res) {
            $scope.filteredDevices = res.data;
        });
    }

    loadAll();

    // SEARCH (GET /api/devices?search=...)
    $scope.searchDevices = function () {
        var s = ($scope.deviceName || "").trim();
        if (!s) { loadAll(); return; }

        $http.get("/api/devices", { params: { search: s } })
            .then(function (res) { $scope.filteredDevices = res.data; });
    };

    // ADD
    $scope.openAddModal = function () {
        openDeviceModal({ Id: null, Name: "" }, false);
    };

    // EDIT
    $scope.openEditModal = function (d) {
        openDeviceModal(angular.copy(d), true);
    };

    function openDeviceModal(deviceModel, isEdit) {
        var modalInstance = $modal.open({
            templateUrl: "deviceModal.html",
            controller: "DeviceModalController",
            resolve: {
                device: function () { return deviceModel; },
                isEdit: function () { return isEdit; }
            }
        });

        modalInstance.result.then(function (resultDevice) {
            if (!isEdit) {
                // POST /api/devices
                $http.post("/api/devices", { Name: resultDevice.Name })
                    .then(function () { $scope.deviceName = ""; loadAll(); });
            } else {
                // PUT /api/devices/{id}
                $http.put("/api/devices/" + resultDevice.Id, { Name: resultDevice.Name })
                    .then(function () { $scope.deviceName = ""; loadAll(); });
            }
        });
    }

    // DELETE (open confirm modal -> DELETE /api/devices/{id})
    $scope.openDeleteModal = function (d) {
        var modalInstance = $modal.open({
            templateUrl: "deleteConfirmModal.html",
            controller: "DeleteConfirmController",
            resolve: {
                device: function () { return angular.copy(d); }
            }
        });

        modalInstance.result.then(function (idToDelete) {
            $http.delete("/api/devices/" + idToDelete)
                .then(function () {
                    loadAll();
                })
                .catch(function (err) {
                    var msg = (err.data && (err.data.Message || err.data)) ? (err.data.Message || err.data) : "Delete failed.";
                    alert(msg);
                    console.error("Delete device failed:", err);
                });
        });
    };
});

app.controller("DeviceModalController", function ($scope, $modalInstance, device, isEdit) {
    $scope.device = device;
    $scope.isEdit = isEdit;

    $scope.save = function () {
        $scope.device.Name = ($scope.device.Name || "").trim();
        if (!$scope.device.Name) return;
        $modalInstance.close($scope.device);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
});

app.controller("DeleteConfirmController", function ($scope, $modalInstance, device) {
    $scope.device = device;

    $scope.confirm = function () {
        $modalInstance.close(device.Id);
    };

    $scope.cancel = function () {
        $modalInstance.dismiss("cancel");
    };
});
