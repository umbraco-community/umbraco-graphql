using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Our.Umbraco.GraphQL.Models
{
    public enum Permissions
    {
        Read,
        Write,
        None
    }

    [TableName("GraphQL_AccountSettings")]
    [PrimaryKey("id", autoIncrement = true)]
    class AccountSettings
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        /// <summary>
        /// The account that this setting belongs to
        /// </summary>
        [ForeignKey(typeof(Account), Name = "FK_AccountSettings_Account", Column = "id", Type = typeof(Account))]
        [Index(IndexTypes.NonClustered, Name = "IX_AccountId")]
        public int AccountId { get; set; }

        /// <summary>
        /// The doc type we are setting permissions on
        /// </summary>
        public string DocTypeAlias { get; set; }

        /// <summary>
        /// The property we are setting permissions on
        /// </summary>
        public string PropertyTypeAlias { get; set; }

        /// <summary>
        /// What access rights does this account have for this field? In the database we store this as an Int, use Permission to get the enum version of this in your code.
        /// <seealso cref="Permission"/>
        /// </summary>
        [Column("permission")]
        public int PermissionAsInt { get; set; }

        /// <summary>
        /// The actual permissions the account has for this setting
        /// </summary>
        [Ignore]
        public Permissions Permission {
            get {
                var permission = Permissions.None;
                if (Enum.IsDefined(typeof(Permissions), PermissionAsInt))
                {
                    permission = (Permissions)PermissionAsInt;
                }

                return permission;
            }

            set {
                PermissionAsInt = (int) value;
            }
        }

        /// <summary>
        /// The unique "hash" for this setting, this will be used when we build up the "Claims" that this user has
        /// by checking each field to see if there is a claim for it using this hash
        /// </summary>
        [Ignore]
        public string PermissionClaimHash {
            get {
                return $"{DocTypeAlias}:{PropertyTypeAlias}:{Permission}";
            }
        }

        /// <summary>
        /// Notes about this field for this acccount (ie "hiding this as its sensitive information")
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Data and time this account setting was created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Date and time this account setting was last saved
        /// </summary>
        public DateTime UpdatedOn { get; set; }

    }
}
