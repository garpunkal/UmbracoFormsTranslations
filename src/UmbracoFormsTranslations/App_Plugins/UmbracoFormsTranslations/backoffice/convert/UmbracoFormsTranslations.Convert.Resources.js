(function () {
    'use strict';

    function ConvertResources($http, umbRequestHelper) {

        function convert(model) {
            return umbRequestHelper.resourcePromise($http.post('/umbraco/backoffice/UmbracoFormsTranslations/convert/convert', model),
                'Failed to convert nodeId: ' + model.id);
        }

        function isValid(model) {
            return umbRequestHelper.resourcePromise($http.post('/umbraco/backoffice/UmbracoFormsTranslations/convert/isvalid', model),
                'Failed to validate for nodeId: ' + model.id);
        }

        var resource = {
            convert: convert,
            isValid: isValid
        };

        return resource;
    }
    angular.module('umbraco.resources').factory('ConvertResources', ConvertResources);
})();