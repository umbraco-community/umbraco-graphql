using GraphQL;
using GraphQL.Validation;
using GraphQL.Language.AST;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Our.Umbraco.GraphQL.ValidationRules
{
    public class RequiresAuthValidationRule : IValidationRule
    {
        public INodeVisitor Validate(ValidationContext context)
        {
            var userContext = context.UserContext.As<UmbracoGraphQLContext>();

            return new EnterLeaveListener(x =>
            {
                var currentDoctype = "";

                // this could leak info about hidden fields in error messages
                // it would be better to implement a filter on the schema so it
                // acts as if they just don't exist vs. an auth denied error
                x.Match<Field>(fieldAst =>
                {
                    var fieldDef = context.TypeInfo.GetFieldDef();

                    // fields nested under _contentData currently don't have the doctypeAlias stored against and i'm struggling to find a way to set it as such
                    // As a workaround this is set on the parent _conentData field and we use that when working out permissions
                    if (fieldDef.Name == "_contentData")
                    {
                        var doctype = fieldDef.GetMetadata<string>("documentTypeAlias");
                        if (!string.IsNullOrEmpty(doctype))
                        {
                            currentDoctype = doctype;
                        }
                    }

                    if (fieldDef.RequiresPermissions() && (!fieldDef.CanAccess(currentDoctype, userContext.Claims)))
                    {
                        context.ReportError(new ValidationError(
                            context.OriginalQuery,
                            "auth-required",
                            $"You are not authorized to run this query.",
                            fieldAst));
                    }
                });
            });
        }
    }
}
