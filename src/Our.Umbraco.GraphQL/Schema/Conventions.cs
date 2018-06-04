using System;
using GraphQL;
using Umbraco.Core.Models;

namespace Our.Umbraco.GraphQL.Schema
{
    public static class Conventions
    {
        public static class NameResolvers
        {
            public static Func<IContentTypeBase, string> PascalCase = ct => ct.Alias.ToPascalCase();

            public static Func<IContentTypeBase, string> PascalCasePostfixWithPublishedContent = ct =>
                $"{PascalCase(ct)}PublishedContent";
        }
    }
}