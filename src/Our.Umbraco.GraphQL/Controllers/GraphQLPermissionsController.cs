using Newtonsoft.Json;
using Our.Umbraco.GraphQL.Models;
using Our.Umbraco.GraphQL.Models.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.GraphQL.Controllers
{
    public class GraphQLPermissionsController : UmbracoAuthorizedApiController
    {
        private readonly UmbracoDatabase _database = ApplicationContext.Current.DatabaseContext.Database;

        // TODO: Lock down the ability to set/view permissions to a User Group

        /// <summary>
        /// Sets the GraphQL permisisons for what fields are accessible for the specified account
        /// </summary>
        /// <param name="accountPermissions">The set permissions request</param>
        /// <remarks>
        /// Default endpoint for the request: /umbraco/backoffice/api/GraphQLPermissions/SetPermissions
        /// </remarks>
        [HttpPost]
        public void SetPermissions(SetAccountPermissionsRequest accountPermissions)
        {
            var entries = new List<AccountSettings>();
            foreach (var permission in accountPermissions.Permissions)
            {
                var accountSettingEntry = new AccountSettings();
                // Always default to empty for niw
                accountSettingEntry.Notes = ""; 
                // Default to read until we have functionality for other permissions
                accountSettingEntry.Permission = Permissions.Read; // TODO: functionality for Write permissions
                accountSettingEntry.AccountId = accountPermissions.AccountId;
                accountSettingEntry.PropertyTypeAlias = permission.PropertyAlias;
                accountSettingEntry.DocTypeAlias = permission.DoctypeAlias;
                accountSettingEntry.IsBuiltInProperty = permission.IsBuiltInProperty;
                accountSettingEntry.CreatedOn = DateTime.UtcNow;
                accountSettingEntry.UpdatedOn = DateTime.UtcNow;

                entries.Add(accountSettingEntry);
            }

            // Insert in bulk so it's handled within a single query
            _database.BulkInsertRecords(entries, ApplicationContext.Current.DatabaseContext.SqlSyntax);
        }

        /// <summary>
        /// Gets a list of the current GraphQL permissions for the specified account
        /// </summary>
        /// <param name="accountId">The GraphQL account id</param>
        /// <returns>
        /// Example request endpoint: /umbraco/backoffice/api/GraphQLPermissions/GetPermissions?accountId=1
        /// </returns>
        [HttpGet]
        public string GetPermissions(int accountId)
        {
            var sql = new Sql("SELECT * FROM GraphQL_AccountSettings WHERE Id=@0", accountId);
            var settings = _database.Query<AccountSettings>(sql);

            if (settings != null)
            {
                var accountPermissions = new List<AccountPermission>();
                foreach (var permission in settings)
                {
                    var accountPermission = new AccountPermission();
                    accountPermission.Notes = permission.Notes; 
                    accountPermission.Permission = permission.Permission.ToString(); 
                    accountPermission.PropertyAlias = permission.PropertyTypeAlias;
                    accountPermission.DoctypeAlias = permission.DocTypeAlias;
                    accountPermission.IsBuiltInProperty = permission.IsBuiltInProperty;

                    accountPermissions.Add(accountPermission);
                }

                var results = JsonConvert.SerializeObject(accountPermissions);
                return results;
            }

            return null;
        }
    }
}
