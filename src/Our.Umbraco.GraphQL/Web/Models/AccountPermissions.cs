namespace Our.Umbraco.GraphQL.Web.Models
{
    public class AccountPermission
    {
        /// <summary>
        /// The doc type we are setting permissions on
        /// </summary>
        public string DoctypeAlias { get; set; }

        /// <summary>
        /// The property we are setting permissions on
        /// </summary>
        public string PropertyAlias { get; set; }

        /// <summary>
        /// Flags if the property is built in like "createDate" or if it is custom
        /// </summary>
        public bool IsBuiltInProperty { get; set; }

        /// <summary>
        /// Notes about this field for this acccount (ie "hiding this as its sensitive information")
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// What access rights does this account have for this field?
        /// </summary>
        /// <remarks>
        /// Options are "Read" and "Write"
        /// </remarks>
        public string Permission { get; set; }
    }
}
