using Our.Umbraco.GraphQL.Models;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;
using System;

namespace Our.Umbraco.GraphQL.Migrations
{
    [Migration("1.0.0", 1, "GraphQL")]
    public class CreateInitialTables : MigrationBase
    {
        public string accountsTableName = "Accounts";
        public string accountSettingsTableName = "AccountSettings";

        public CreateInitialTables(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
        {
        }

        public override void Down()
        {
            Logger.Info<CreateInitialTables>("1.0.0: Running Migration Down");

            var tables = SqlSyntax.GetTablesInSchema(Context.Database).ToArray();

            if (tables.InvariantContains(accountsTableName))
            {
                Logger.Info<CreateInitialTables>("Deleting Accounts Table");
                Delete.Table(accountsTableName);
            }
            if (tables.InvariantContains(accountSettingsTableName))
            {
                Logger.Info<CreateInitialTables>("Deleting AccountSettings Table");
                Delete.Table(accountSettingsTableName);
            }
        }

        public override void Up()
        {
            Logger.Info<CreateInitialTables>("1.0.0: Running Migration Up");

            var tables = SqlSyntax.GetTablesInSchema(Context.Database).ToArray();

            if (!tables.InvariantContains(accountsTableName))
            {
                Logger.Info<CreateInitialTables>("Creation Accounts Table");
                Create.Table<Account>();
            }

            if (!tables.InvariantContains(accountSettingsTableName))
            {
                Logger.Info<CreateInitialTables>("Creating AccountSettings Table");
                Create.Table<AccountSettings>();
            }

            var account = new Account()
            {
                AccessToken = Guid.NewGuid(),
                CreatedBy = 0,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                IsEnabled = true,
                Name = "Pete Test",
                Notes = "Just as test account setup in the create initial tables migration"
            };

            var db = ApplicationContext.Current.DatabaseContext.Database;

            db.Insert(account);

            var accountSetting = new AccountSettings()
            {
                AccountId = account.Id,
                DocTypeAlias = "navigationBase",
                PropertyTypeAlias = "seoMetaDescription",
                Permission = Permissions.Read,
                CreatedOn = DateTime.Now,
                UpdatedOn = DateTime.Now,
                Notes = ""
            };

            db.Insert(accountSetting);
            
        }
    }
}
