using Our.Umbraco.GraphQL.Models;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;
using System;
using Umbraco.Core.Persistence;

namespace Our.Umbraco.GraphQL.Migrations
{
    [Migration("1.0.0", 1, "GraphQL")]
    public class CreateInitialTables : MigrationBase
    {
        public const string accountsTableName = "Accounts";
        public const string accountSettingsTableName = "AccountSettings";

        private readonly UmbracoDatabase _database = ApplicationContext.Current.DatabaseContext.Database;
        private readonly DatabaseSchemaHelper _schemaHelper;

        public CreateInitialTables(ISqlSyntaxProvider sqlSyntax, ILogger logger)
            : base(sqlSyntax, logger)
        {
            _schemaHelper = new DatabaseSchemaHelper(_database, logger, sqlSyntax);
        }

        public override void Down()
        {
            Logger.Info<CreateInitialTables>("1.0.0: Running Migration Down");
            
            if (_schemaHelper.TableExist(accountsTableName))
            {
                Logger.Info<CreateInitialTables>("Deleting Accounts Table");
                _schemaHelper.DropTable(accountsTableName);
            }
            if (_schemaHelper.TableExist(accountSettingsTableName))
            {
                Logger.Info<CreateInitialTables>("Deleting AccountSettings Table");
                _schemaHelper.DropTable(accountSettingsTableName);
            }
        }

        public override void Up()
        {
            Logger.Info<CreateInitialTables>("1.0.0: Running Migration Up");

            if (!_schemaHelper.TableExist(accountsTableName))
            {
                Logger.Info<CreateInitialTables>("Creation Accounts Table");
                _schemaHelper.CreateTable<Account>();
            }

            if (!_schemaHelper.TableExist(accountSettingsTableName))
            {
                Logger.Info<CreateInitialTables>("Creating AccountSettings Table");
                _schemaHelper.CreateTable<AccountSettings>();
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

            _database.Insert(account);

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

            _database.Insert(accountSetting);
        }
    }
}
