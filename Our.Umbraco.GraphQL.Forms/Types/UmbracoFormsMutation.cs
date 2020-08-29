using Our.Umbraco.GraphQL.Attributes;

namespace Our.Umbraco.GraphQL.Forms.Types
{
    public class UmbracoFormsMutation
    {
    }

    public class ExtendMutationWithUmbracoFormsMutation
    {
        [NonNull]
        [Description("Mutation to submit an Umbraco Form")]
        public UmbracoFormsMutation UmbracoForms([Inject] UmbracoFormsMutation mutation) => mutation;
    }
}
