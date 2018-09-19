using System;
using System.Collections.Generic;
using GraphQL;
using Our.Umbraco.GraphQL.Models;
using Our.Umbraco.GraphQL.Web;
using Umbraco.Core;
using Umbraco.Web;

namespace Our.Umbraco.GraphQL
{
    public class UmbracoGraphQLContext
    {
        private Guid defaultAccessToken = new Guid("6bd10bc4-1d31-478a-8abc-78560086286b");
        private UmbracoHelper _umbraco;

        public UmbracoGraphQLContext(Uri requestUri, ApplicationContext applicationContext, UmbracoContext umbracoContext, GraphQLServerOptions options, string accessToken, out ExecutionErrors errors )
        {
            errors = new ExecutionErrors();

            RequestUri = requestUri;
            ApplicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            UmbracoContext = umbracoContext ?? throw new ArgumentNullException(nameof(umbracoContext));
            Claims = new List<string>(); // no claims is the default

            var db = ApplicationContext.Current.DatabaseContext.Database;
            Account account = null;

            // TODO: Pete - need to cache these authentication look ups for speed
            // Which permissions should we use?
            if (!String.IsNullOrEmpty(accessToken))
            {
                var accessTokenGuid = new Guid();
                if (Guid.TryParse(accessToken, out accessTokenGuid))
                {
                    account = db.SingleOrDefault<Account>("WHERE AccessToken =  @0", accessToken);
                }
                else
                {
                    errors.Add(new ExecutionError("Malformed Access Token passed in, please check it"));
                }
            }

            if (account == null)
            {
                // Fall back to the default permissions
                errors.Add(new ExecutionError("Unknown Access Token, please check, falling back to Default permissions"));
                account = db.SingleOrDefault<Account>("WHERE AccessToken =  @0", defaultAccessToken);
            }

            if (account == null)
            {
                errors.Add(new ExecutionError("No default permissions found, check the account settings in the GraphQL section of the CMS"));
            }
            else
            {
                if (account.IsEnabled)
                {
                    var settings = db.Query<AccountSettings>("WHERE AccountId = @0", account.Id);

                    if (settings != null)
                    {
                        foreach (var setting in settings)
                        {
                            // Do we check that we actually still have the doctypes active?
                            Claims.Add(setting.PermissionClaimHash);
                        }
                    }
                }
                else
                {
                    errors.Add(new ExecutionError("Passed in account is deactivated"));
                }
            }
        }

        public ApplicationContext ApplicationContext { get; }
        public GraphQLServerOptions Options { get; }
        public Uri RequestUri { get; }
        public UmbracoHelper Umbraco => _umbraco ?? (_umbraco = new UmbracoHelper(UmbracoContext));
        public UmbracoContext UmbracoContext { get; }
        public List<string> Claims { get; private set; }
    }
}
