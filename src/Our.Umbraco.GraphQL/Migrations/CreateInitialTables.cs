using Migrations.Models;
using Our.Umbraco.GraphQL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

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

            if (tables.InvariantContains(accountSettingsTableName))
            {
                Logger.Info<CreateInitialTables>("Creating AccountSettings Table");
                Create.Table<AccountSettings>();
            }
        }
}
