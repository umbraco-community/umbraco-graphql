angular.module("umbraco.resources").factory("graphQLForUmbracoApiResource", function ($http, $q) {

    var graphQLForUmbracoApiResource = {};
    var useMock = false;

    /**
     * @method get - Gets the currently set permissions for GraphQL visibility 
     * from the API.
     * @returns {Promise.<{permissions: {doctypeAlias: string, propertyAlias: string, isBuiltInProperty: boolean, notes: string, permission: string}[]}>}
     */
	graphQLForUmbracoApiResource.get = function (accountId) {
        var id = typeof accountId !== 'undefined' ? accountId : 1;        
        if (useMock) {
            var deferred = $q.defer();
            setTimeout(function() {
                deferred.resolve({
                    "permissions": [  
                        {  
                            "doctypeAlias":"home",
                            "propertyAlias":"sitename",
                            "isBuiltInProperty": false,
                            "notes": "",
                            "permission": "Read"
                        },
                        {  
                            "doctypeAlias":"home",
                            "propertyAlias":"bodyText",
                            "isBuiltInProperty": false,
                            "notes": "",
                            "permission": "Read"
                        },
                        {
                            "doctypeAlias": "blogpost",
                            "propertyAlias": "pageTitle",
                            "isBuiltInProperty": false,
                            "notes": "",
                            "permission": "Read"
                        }
                    ]
                });
            }, 1000);
            return deferred.promise;
        } else {
            return $http.get('/umbraco/backoffice/api/GraphQLPermissions/GetPermissions?accountId=' + id).then(function(response) {
                var data = JSON.parse(response.data);
                if (typeof data === 'string') {
                    data = JSON.parse(data);
                }
                var permissions = data.map(function(item, index) {
                    return {
                        doctypeAlias: item.DoctypeAlias,
                        propertyAlias: item.PropertyAlias,
                        isBuiltInProperty: item.IsBuildInProperty,
                        notes: item.Notes,
                        permission: item.Permission
                    };
                });
                return {
                    permissions: permissions
                };
            });
        }
    };
    
    /**
     * @method set - Sends the currently set permissions for GraphQL visibility 
     * back via API.
     * @param {{doctypeAlias: string, propertyAlias: string, isBuiltInProperty: boolean, notes: string, permission: string}[]} permissions 
     * @param {number} [accountId = 1] 
     * @returns {Promise.<{success: boolean}>}
     */
    graphQLForUmbracoApiResource.set = function(permissions, accountId) {
        if (useMock) {
            var deferred = $q.defer();
            setTimeout(function() {
                deferred.resolve({
                    "success": true
                });
            }, 1000);
            return deferred.promise;
        } else {
            var data = {
                accountId: typeof accountId !== 'undefined' ? accountId : 1,
                permissions: permissions
            }
            return $http.post('/umbraco/backoffice/api/GraphQLPermissions/SetPermissions', data).then(function(response) {
                return {
                    success: true
                };
            }, function (error) {
                return {
                    success: false,
                    error: error
                }
            });
        }
    };

	return graphQLForUmbracoApiResource;
});