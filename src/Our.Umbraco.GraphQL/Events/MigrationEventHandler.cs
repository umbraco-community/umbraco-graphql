using Semver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence.Migrations;

namespace Our.Umbraco.GraphQL.Events
{
    /// <summary>
    /// Runs our DB Migrations on startup.
    /// 
    /// Thank you to the wonderful Kevin Jump for pointers on how to get this as slick as it is
    /// See: https://github.com/KevinJump/UmbracoMigrationsDemo for details
    /// </summary>
    public class MigrationEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            // Update the version number as we deploy new DB changes to run any new Migrations
            ApplyMigrations(applicationContext, "GraphQL", new SemVersion(1, 0, 0));
        }

        private void ApplyMigrations(ApplicationContext applicationContext, string productName, SemVersion targetVersion)
        {
            var currentVersion = new SemVersion(0);

            var migrations = applicationContext.Services.MigrationEntryService.GetAll(productName);
            var latest = migrations.OrderByDescending(x => x.Version).FirstOrDefault();
            if (latest != null)
                currentVersion = latest.Version;

            if (targetVersion == currentVersion)
                return;

            var migrationRunner = new MigrationRunner(
                applicationContext.Services.MigrationEntryService,
                applicationContext.ProfilingLogger.Logger,
                currentVersion,
                targetVersion,
                productName);

            try
            {
                migrationRunner.Execute(applicationContext.DatabaseContext.Database);
            }
            catch (Exception ex)
            {
                applicationContext.ProfilingLogger
                    .Logger.Error<MigrationEventHandler>("Error running " + productName + " Migration", ex);
            }
        }
    }
}
