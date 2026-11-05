using Mapster;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Infrastructure.Mapping;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class FormPropertyAdminProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<FormProperty, AddFormPropertyModel>();
            config.NewConfig<AddFormPropertyModel, FormProperty>()
                .MapEntityLookup(x => x.FormId, x => x.Form);
            config.NewConfig<FormProperty, UpdateFormPropertyModel>()
                .Map(model => model.ShowPlaceholder, x => x is IHavePlaceholder)
                .Map(model => model.Placeholder,
                    x => x is IHavePlaceholder ? ((IHavePlaceholder)x).PlaceHolder : string.Empty);
            config.NewConfig<UpdateFormPropertyModel, FormProperty>();
            config.NewConfig<TextBox, UpdateFormPropertyModel>()
                .Map(model => model.ShowPlaceholder, x => true);
            config.NewConfig<UpdateFormPropertyModel, TextBox>();
            config.NewConfig<TextArea, UpdateFormPropertyModel>()
                .Map(model => model.ShowPlaceholder, x => true);
            config.NewConfig<UpdateFormPropertyModel, TextArea>();
            config.NewConfig<Email, UpdateFormPropertyModel>()
                .Map(model => model.ShowPlaceholder, x => true);
            config.NewConfig<UpdateFormPropertyModel, Email>();
        }
    }
}