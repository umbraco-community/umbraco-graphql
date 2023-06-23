using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Types;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Media;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;

namespace Our.Umbraco.GraphQL.Adapters.PublishedContent.Types
{
    public class ImageCropperGraphType : ObjectGraphType<ImageCropperValue>
    {
        public ImageCropperGraphType(IImageUrlGenerator imageUrlGenerator)
        {
            Name = "ImageCropper";
            Field<ListGraphType<StringGraphType>>().Name("crops")
                .Resolve(context => context.Source.Crops);
            Field<StringGraphType>().Name("src")
            .Resolve(context => context.Source.Src);
            Field<StringGraphType>().Name("focalPoint")
                .Resolve(context => context.Source.FocalPoint);

            this.Field<StringGraphType>().Name("cropUrl")
                .Argument<StringGraphType>("alias", null)
                .Argument<IntGraphType>("height", null)
                .Argument<IntGraphType>("width", null)
                .Argument<BooleanGraphType>("useCropDimensions", null)
                .Argument<BooleanGraphType>("useFocalPoint", null)
                .Argument<StringGraphType>("cacheBusterValue", null)
                .Resolve(context =>
                {
                    if (context.HasArgument("alias"))
                    {
                        return context.Source.GetCropUrl(
                            context.GetArgument<string>("alias"),
                            imageUrlGenerator,
                            context.GetArgument<bool>("useCropDimensions", true),
                            context.GetArgument<bool>("useFocalPoint"),
                            context.GetArgument<string>("cacheBusterValue"));
                    }
                    else
                    {
                        return context.Source.GetCropUrl(
                            context.GetArgument<int>("height"),
                            context.GetArgument<int>("width"),
                            imageUrlGenerator,
                            context.GetArgument<string>("cacheBusterValue"));
                    }

                });
        }


    }
}
