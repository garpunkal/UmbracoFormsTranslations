(function () {
    "use strict";
    function ConvertActionController(
        $scope,
        $route,
        entityResource,
        notificationsService,
        appState,
        navigationService,
        ConvertResources) {

        var SUCCESS = 0;
        var selectedNode = appState.getMenuState("currentNode");

        $scope.loading = false;
        $scope.isValid = false;
        $scope.isForm = true;

        $scope.nodeId = selectedNode.id;
        $scope.nodeName = selectedNode.name;

        $scope.init = function () {
            $scope.loading = true;

            var convertModel = { id: $scope.nodeId };

            ConvertResources.isValid(convertModel)
                .then(function (response) {
                    $scope.isValid = (response.resultType === SUCCESS);
                    $scope.loading = false;
                });

        };

        $scope.convert = function () {
            $scope.loading = true;

            var convertModel = { id: $scope.nodeId };

            ConvertResources.convert(convertModel)
                .then(function (response) {

                    $scope.loading = false;
                    navigationService.hideDialog();

                    if (response.resultType === SUCCESS) {
                        notificationsService.success("Success", response.message);
                        $route.reload();
                    }
                    else
                        notificationsService.error("Error", response.message);
                });
        };
    }
    angular.module('umbraco').controller("umbracoFormsTranslations.Convert.Controller", ConvertActionController);
})();