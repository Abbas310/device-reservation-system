(function () {
    "use strict";

    var mod;
    try {
        mod = angular.module("sharedDirectives");
    } catch (e) {
        mod = angular.module("sharedDirectives", []);
    }

    mod.directive("phoneNumberSelector", function ($http) {
        return {
            restrict: "E",
            scope: {
                ngModel: "=",
                placeholder: "@"
            },
            template:
                '<select class="form-control phone-selector" ng-model="ngModel" ' +
                '        ng-options="p.Id as p.Number for p in phoneNumbers">' +
                '  <option value="">{{placeholderText}}</option>' +
                '</select>',
            link: function (scope) {
                scope.phoneNumbers = [];
                scope.placeholderText = scope.placeholder || "All Phone Numbers";

                $http.get("/api/phonenumbers").then(function (res) {
                    scope.phoneNumbers = res.data || [];

                    if (scope.ngModel !== null && scope.ngModel !== undefined && scope.ngModel !== "") {
                        var n = parseInt(scope.ngModel, 10);
                        scope.ngModel = isNaN(n) ? "" : n;
                    }
                });
            }
        };
    });

})();
