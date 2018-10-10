using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.GraphQL.Models.ApiModels
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
