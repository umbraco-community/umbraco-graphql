using System;

namespace Our.Umbraco.GraphQL.Tests.Filters
{
    public abstract class FilterTest
    {
        protected Func<object, object> ValueResolver = input => input;
    }
}
