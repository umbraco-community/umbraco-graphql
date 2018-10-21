using System.Collections.Generic;

namespace Our.Umbraco.GraphQL.Web.Models
{
    public class SetAccountPermissionsRequest
    {
        /// <summary>
        /// The account that this request belongs to
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// The collection of GraphQL permissions to set for the account
        /// </summary>
        public List<AccountPermission> Permissions { get; set; }
    }
}
