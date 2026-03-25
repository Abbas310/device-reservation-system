(function () {
    "use strict";

    var mod;
    try { mod = angular.module("sharedDirectives"); }
    catch (e) { mod = angular.module("sharedDirectives", []); }

    mod.directive("deviceSelectorOptional", function ($http) {
        return {
            restrict: "E",
            scope: {
                ngModel: "=",
                placeholder: "@"
            },
            template:
                '<select class="form-control device-selector" ng-model="ngModel" ' +
                '        ng-options="d.Id as d.Name for d in devices">' +
                '  <option value="">{{ph}}</option>' +
                '</select>',
            link: function (scope) {
                scope.devices = [];
                scope.ph = scope.placeholder || "-- Select Device --";

                // IMPORTANT: keep empty by default (no auto-select)
                if (scope.ngModel === undefined || scope.ngModel === null) {
                    scope.ngModel = "";
                }

                $http.get("/api/devices").then(function (res) {
                    scope.devices = res.data || [];
                });
            }
        };
    });
})();