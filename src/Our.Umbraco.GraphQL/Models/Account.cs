using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Our.Umbraco.GraphQL.Models
{
    [TableName("GraphQL_Accounts")]
    [PrimaryKey("id", autoIncrement = true)]
    public class Account
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        /// <summary>
        /// The "friendly" name for this account
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Data and time this account was created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Date and time this account was last saved
        /// </summary>
        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// The Umbraco user id of the logged in back office user that created this Account
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// The access token needed to use this Accounts settings
        /// </summary>
        public Guid AccessToken { get; set; }

        /// <summary>
        /// Toggle to allow you to deactivate an account
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// A notes field to keep track of the history of this account
        /// </summary>
        public string Notes { get; set; }
    }
}
