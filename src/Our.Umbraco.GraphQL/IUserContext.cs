using Umbraco.Web;

namespace Our.Umbraco.GraphQL
{
    public interface IUserContext
    {
        UmbracoContext UmbracoContext { get; }
    }

    internal class UserContext
    {
        public UserContext(UmbracoContext umbracoContext)
        {
            UmbracoContext = umbracoContext ?? throw new System.ArgumentNullException(nameof(umbracoContext));
        }

        public UmbracoContext UmbracoContext { get; }
    }
}
