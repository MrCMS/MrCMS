using Mapster;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Admin.Infrastructure.Mapping;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class LayoutAdminProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Layout, UpdateLayoutModel>()
                .Map(x => x.UrlSegment, x => x.Path);
            config.NewConfig<UpdateLayoutModel, Layout>()
                .Map(x => x.Path, x => x.UrlSegment);
            config.NewConfig<Layout, AddLayoutModel>()
                .Map(x => x.UrlSegment, x => x.Path);
            config.NewConfig<AddLayoutModel, Layout>()
                .Map(x => x.Path, x => x.UrlSegment)
                .MapEntityLookup(x => x.ParentId, x => x.Parent);
        }
    }

    public class MediaCategoryAdminProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<MediaCategory, UpdateMediaCategoryModel>();
            config.NewConfig<UpdateMediaCategoryModel, MediaCategory>();
            config.NewConfig<MediaCategory, AddMediaCategoryModel>();
            config.NewConfig<AddMediaCategoryModel, MediaCategory>()
                .MapEntityLookup(x => x.ParentId, x => x.Parent);
        }
    }
}