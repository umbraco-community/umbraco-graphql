using Newtonsoft.Json;
using Our.Umbraco.GraphQL.Models;
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

        // Default endpoint: /umbraco/backoffice/api/GraphQLPermissions/SetPermissions
        [HttpPost]
        public void SetPermissions(GraphQLAccountPermissions accountPermissions)
        {
            var entries = new List<AccountSettings>();
            foreach (var permission in accountPermissions.Permissions)
            {
                var accountSettingEntry = new AccountSettings();
                accountSettingEntry.Notes = ""; // Always default to empty for now
                accountSettingEntry.Permission = Permissions.Read; // Always default to read for now
                accountSettingEntry.AccountId = accountPermissions.AccountId;
                accountSettingEntry.PropertyTypeAlias = permission.PropertyAlias;
                accountSettingEntry.DocTypeAlias = permission.DoctypeAlias;
                accountSettingEntry.IsBuiltInProperty = permission.IsBuiltInProperty;
                accountSettingEntry.CreatedOn = DateTime.UtcNow;
                accountSettingEntry.UpdatedOn = DateTime.UtcNow;

                entries.Add(accountSettingEntry);
            }

            _database.BulkInsertRecords(entries, ApplicationContext.Current.DatabaseContext.SqlSyntax);
        }

        [HttpGet]
        public string GetPermissions(int accountId)
        {
            var sql = new Sql("SELECT * FROM GraphQL_AccountSettings WHERE Id=@0", accountId);
            var settings = _database.Query<AccountSettings>(sql);

            if (settings != null)
            {
                var accountPermissions = new List<AccountPermissions>();
                foreach (var permission in settings)
                {
                    var accountPermission = new AccountPermissions();
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

    public class GraphQLAccountPermissions
    {
        public int AccountId { get; set; }
        public List<AccountPermissions> Permissions {get; set;}
    }

    public class AccountPermissions
    {
        public string DoctypeAlias { get; set; }
        public string PropertyAlias { get; set; }
        public bool IsBuiltInProperty { get; set; }
        public string Notes { get; set; }

        /// <summary>
        /// The type of permission granted to the property
        /// </summary>
        /// <remarks>
        /// Options are "Read" and "Write"
        /// </remarks>
        public string Permission { get; set; }
    }
}
