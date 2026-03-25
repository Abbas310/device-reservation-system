(function () {
    "use strict";

    var mod;
    try {
        mod = angular.module("sharedDirectives");
    } catch (e) {
        mod = angular.module("sharedDirectives", []);
    }

    mod.directive("deviceSelector", function ($http) {
        return {
            restrict: "E",
            scope: {
                ngModel: "=",        // selected DeviceId
                includeAll: "@",     // "true" for filter dropdown
                placeholder: "@"     // e.g. "Select device..."
            },
            template:
                '<select class="form-control device-selector" ng-model="ngModel" ' +
                '        ng-options="d.Id as d.Name for d in devices">' +
                '   <option value="">{{placeholder}}</option>' +
                '</select>',

            link: function (scope) {
                scope.devices = [];

                // placeholder logic
                scope.placeholderText = scope.placeholder || (scope.includeAll === "true" ? "All Devices" : "Select device...");

                $http.get("/api/devices").then(function (res) {
                    scope.devices = res.data || [];

                    // normalize numeric ids (edit mode)
                    if (scope.ngModel !== null && scope.ngModel !== undefined && scope.ngModel !== "") {
                        var n = parseInt(scope.ngModel, 10);
                        scope.ngModel = isNaN(n) ? "" : n;
                    }
                });
            }
        };
    });

})();
