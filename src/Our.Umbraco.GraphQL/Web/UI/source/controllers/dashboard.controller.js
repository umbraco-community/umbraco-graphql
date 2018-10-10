angular.module("umbraco").controller("graphql.for.umbraco.dashboardcontroller", function($scope, contentTypeResource, notificationsService, graphQLForUmbracoApiResource) {

    // Initialization Methods //////////////////////////////////////////////////

    /**
     * @method init Triggered when the controller is loaded by a view to 
     * initialize the JS for the controller.
     * @returns {void}
     */
    $scope.init = function() {
        $scope.setInitialVariables();
        $scope.getDocTypes();
        $scope.getPermissions();
    };

    /**
     * @method setInitialVariables Sets the initial states of the variables used 
     * in the scope.
     * @returns {void}
     */
    $scope.setInitialVariables = function () {
        $scope.docTypes = [];
        $scope.groups = [];
        $scope.permissions = [];
        $scope.pendingPermissions = [];
        $scope.selectedDocType = false;
        $scope.selectedDocTypeAlias = '';
        $scope.selectedDocTypeName = '';
    };

    /**
     * @method getDocTypes Gets the doctypes that exist on the site and builds 
     * a list to display.
     * @returns {void}
     */
    $scope.getDocTypes = function() {
        contentTypeResource.getAll().then(function(docTypes) {
            docTypes.forEach(function (type, index) {
                $scope.docTypes.push({
                    alias: type.alias,
                    id: type.id,
                    name: type.name
                });
            });
        });
    };

    /**
     * @method getPermissions - Gets currently set GraphQL visibility 
     * permissions via API.
     * @returns {void}
     */
    $scope.getPermissions = function() {
        return graphQLForUmbracoApiResource.get().then(function(response) {
            var permissions = [];
            response.permissions.forEach(function(responsePermission) {
                var docTypeAlreadyInPermission = false;
                var alias = responsePermission.doctypeAlias;
                $scope.permissions.forEach(function(permission) {
                    if (permission.alias == alias) {
                        docTypeAlreadyInPermission = true;
                        permission.properties.push({
                            alias: responsePermission.propertyAlias,
                            isBuiltInProperty: responsePermission.isBuiltInProperty,
                            notes: responsePermission.notes,
                            permission: responsePermission.permission
                        });
                    }
                });
                if (!docTypeAlreadyInPermission) {
                    $scope.permissions.push({
                        alias: alias,
                        properties: [{
                            alias: responsePermission.propertyAlias,
                            isBuiltInProperty: responsePermission.isBuiltInProperty,
                            notes: responsePermission.notes,
                            permission: responsePermission.permission
                        }]
                    });
                }
            });
        });
    };

    // Helper Methods //////////////////////////////////////////////////////////

    /**
     * @method convertPermissionsForApi - Converts the permissions used in the 
     * scope into a format used by the package.
     * @param {JSON[]} permissions
     * @returns {JSON[]}
     */
    $scope.convertPermissionsForApi = function(permissions) {
        var forApi = [];
        permissions = JSON.parse(JSON.stringify(permissions));
        permissions.forEach(function(permission) {
            permission.properties.forEach(function(property) {
                forApi.push({
                    docTypeAlias: permission.alias,
                    isBuiltInProperty: property.isBuiltInProperty,
                    notes: property.notes,
                    permission: property.permission,
                    propertyAlias: property.alias
                });
            });
        });
        return forApi;
    };

    /**
     * @method getPropertiesForDocType Uses `contentTypeResource` to get a list 
     * of groups and properties that exist on the doctype with the indicated 
     * id and assigns them to scope.
     * @param {number} id 
     * @returns {void}
     */
    $scope.getPropertiesForDocType = function(id) {
        $scope.groups = [];
        contentTypeResource.getById(id).then(function(content) {
            $scope.groups = content.groups.map(function (group, index) {
                var properties = group.properties.map(function(prop) {
                    return {
                        alias: prop.alias,
                        id: prop.id,
                        isVisible: false,
                        name: prop.label
                    };
                });
                return {
                    name: group.name,
                    id: group.id,
                    properties: properties
                };
            });
        });
    };

    /**
     * @method isDocTypeSelected Returns `true` if a docType has been selected 
     * by the user.
     * @returns {boolean}
     */
    $scope.isDocTypeSelected = function() {
        return !!$scope.selectedDocType;
    };

    /**
     * @method isPropertyVisible Returns `true` if the docType with the matching 
     * `docTypeAlias` has the permissions set for GraphQL to view the property 
     * with the matching `propertyAlias`.
     * @param {string} docTypeAlias 
     * @param {string} propertyAlias 
     * @returns {boolean}
     */
    $scope.isPropertyVisible = function(docTypeAlias, propertyAlias) { 
        var isVisible = false;
        $scope.pendingPermissions.forEach(function(permission) {
            if (permission.alias === docTypeAlias) {
                permission.properties.forEach(function(property) {
                    if (property.alias === propertyAlias) {
                        isVisible = true;
                    }
                })
            }
        });
        return isVisible;
    };

    /**
     * @method listVisibleProperties Returns a comma-separated string of the 
     * property aliases of properties that are visible to GraphQL for the 
     * docType with the matching `alias`.
     * @param {string} alias The alias of the docType to look at the properties 
     * of.
     * @returns {string}
     */
    $scope.listVisibleProperties = function(alias) {
        var visible = [];
        $scope.permissions.forEach(function(permission) {
            if (permission.alias === alias) {
                permission.properties.forEach(function(property) {
                    visible.push(property.alias);
                });
            }
        });
        return visible.join(', ');
    }

    // Event Handler Methods ///////////////////////////////////////////////////

    /**
     * @method onChangeDocTypeClick Triggered when user clicks link to unselect 
     * a doctype, returning them to the view to select a new doctype.
     * @param {$event} e 
     * @returns {void}
     */
    $scope.onChangeDocTypeClick = function(e) {
        e.preventDefault();
        $scope.groups = [];
        $scope.pendingPermissions = [];
        $scope.selectedDocType = false;
        $scope.selectedDocTypeAlias = '';
        $scope.selectedDocTypeName = '';
    };

    /**
     * @method onDocTypeClick Triggered when user clicks a doctype link, saving 
     * it to $scope and toggling a request for the properties for that doctype.
     * @param {$event} e 
     * @returns {void}
     */
    $scope.onDocTypeClick = function(e) {
        e.preventDefault();
        var id = e.target.getAttribute('data-id');
        $scope.pendingPermissions = JSON.parse(JSON.stringify($scope.permissions));
        $scope.selectedDocType = id;
        $scope.selectedDocTypeAlias = e.target.getAttribute('data-alias');
        $scope.selectedDocTypeName = e.target.getAttribute('data-name');
        $scope.getPropertiesForDocType(id);
    };

    /**
     * @method onPropertyVisibilityChange Triggered when user clicks on the 
     * checkbox for a doctype property. Toggles its visibility state in the 
     * pendingPermissions array.
     * @param {Event} e 
     * @returns {void}
     */
    $scope.onPropertyVisibilityChange = function(e) {
        var propertyAlias = e.target.getAttribute('data-alias');
        var isChecked = e.target.checked;
        var doesDocTypeHavePermissions = false;
        var doesPropertyPermissionExist = false;
        $scope.pendingPermissions.forEach(function(permission) {
            if (permission.alias === $scope.selectedDocTypeAlias) {
                doesDocTypeHavePermissions = true;
                permission.properties.forEach(function(property, index) {
                    if (property.alias === propertyAlias) {
                        doesPropertyPermissionExist = true;
                        permission.properties.splice(index, 1);
                    }
                });
                if (!doesPropertyPermissionExist) {
                    permission.properties.push({
                        alias: propertyAlias,
                        isBuiltInProperty: false, // TODO: Need to know how to get true value for this.
                        notes: '',
                        permission: 'Read'                       
                    });
                }
            }
        });
        if (!doesDocTypeHavePermissions) {
            $scope.pendingPermissions.push({
                alias: $scope.selectedDocTypeAlias,
                properties: [{
                    alias: propertyAlias,
                    isBuiltInProperty: false,  // TODO: Need to know how to get true value for this.
                    notes: '',
                    permission: 'Read'                      
                }]
            })
        }
    };

    /**
     * @method onSaveClick Triggered when the user clicks on the Save button at 
     * the bottom of the properties visibility table for a docType. Saves the 
     * current permissions for GraphQL visibility via API and then returns the 
     * user to the 'select docType' view.
     * @param {Event} e 
     * @returns {void}
     */
    $scope.onSaveClick = function(e) {
        $scope.permissions = JSON.parse(JSON.stringify($scope.pendingPermissions));
        notificationsService.info(
            "Saving...",
            "saving GraphQL visibility permissions for " + $scope.selectedDocTypeAlias + "."
        );
        var forApi = $scope.convertPermissionsForApi($scope.permissions);
        graphQLForUmbracoApiResource.set(forApi).then(function(response) {
            if (response.success == true) {
                notificationsService.success(
                    'Saved!',
                    'Permissions for ' + $scope.selectedDocTypeAlias + ' saved.'
                );
                $scope.onChangeDocTypeClick(e);
            } else {
                notificationsService.error(
                    'Save Failed',
                    'There was a problem with saving the permissions for ' + $scope.selectedDocTypeAlias
                );
                console.error(response.error);
            }
        });
    }

    // Call $scope.init() //////////////////////////////////////////////////////

    $scope.init();

});