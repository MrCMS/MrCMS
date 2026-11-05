using Mapster;
using MrCMS.Entities.Documents.Web.FormProperties;
using MrCMS.Web.Admin.Infrastructure.Mapping;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class FormListOptionAdminProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<FormListOption, AddFormListOptionModel>();
            config.NewConfig<AddFormListOptionModel, FormListOption>()
                .MapEntityLookup(x => x.FormPropertyId, x => x.FormProperty);
            config.NewConfig<FormListOption, UpdateFormListOptionModel>();
            config.NewConfig<UpdateFormListOptionModel, FormListOption>();
        }
    }
}