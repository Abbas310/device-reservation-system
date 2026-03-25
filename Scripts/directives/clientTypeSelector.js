(function () {
    "use strict";

    var mod;
    try {
        mod = angular.module("sharedDirectives");
    } catch (e) {
        mod = angular.module("sharedDirectives", []);
    }

    mod.directive("clientTypeSelector", function () {
        return {
            restrict: "E",
            scope: {
                ngModel: "=",      // selected type (1 or 2, or "" for All)
                placeholder: "@"   // "All Types" or "Select type..."
            },
            template:
                '<select class="form-control type-selector" ng-model="ngModel">' +
                '  <option value="">{{placeholderText}}</option>' +
                '  <option value="1">Individual</option>' +
                '  <option value="2">Organization</option>' +
                '</select>',
            link: function (scope) {
                scope.placeholderText = scope.placeholder || "All Types";

                // normalize edit mode values (string -> int)
                if (scope.ngModel !== null && scope.ngModel !== undefined && scope.ngModel !== "") {
                    var n = parseInt(scope.ngModel, 10);
                    scope.ngModel = isNaN(n) ? "" : n;
                }
            }
        };
    });

})();
