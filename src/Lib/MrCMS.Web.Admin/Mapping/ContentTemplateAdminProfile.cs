using Mapster;
using MrCMS.ContentTemplates.Entities;
using MrCMS.ContentTemplates.Widgets;
using MrCMS.Web.Admin.Infrastructure.Mapping;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Models.Widgets;

namespace MrCMS.Web.Admin.Mapping;

public class ContentTemplateAdminProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ContentTemplate, AddContentTemplateModel>();
        config.NewConfig<AddContentTemplateModel, ContentTemplate>();
        config.NewConfig<ContentTemplate, UpdateContentTemplateModel>();
        config.NewConfig<UpdateContentTemplateModel, ContentTemplate>();
        config.NewConfig<ContentTemplateWidget, ContentTemplateWidgetAddModel>()
            .Map(x => x.ContentTemplateId, x => x.ContentTemplate.Id);
        config.NewConfig<ContentTemplateWidgetAddModel, ContentTemplateWidget>()
            .MapEntityLookup(x => x.ContentTemplateId, x => x.ContentTemplate);
        config.NewConfig<ContentTemplateWidget, ContentTemplateWidgetUpdateModel>()
            .Map(x => x.ContentTemplate, x => x.ContentTemplate);
        config.NewConfig<ContentTemplateWidgetUpdateModel, ContentTemplateWidget>()
            .Ignore(x => x.ContentTemplate);
    }
}