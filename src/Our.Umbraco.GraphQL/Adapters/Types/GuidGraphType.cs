namespace Our.Umbraco.GraphQL.Adapters.Types
{
    public class GuidGraphType : global::GraphQL.Types.GuidGraphType
    {
        public GuidGraphType()
        {
            Name = "Guid";
            Description = "Globally Unique Identifier.";
        }
    }
}
