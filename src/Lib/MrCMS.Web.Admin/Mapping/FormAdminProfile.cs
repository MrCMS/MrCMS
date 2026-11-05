using Mapster;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Models.Forms;

namespace MrCMS.Web.Admin.Mapping
{
    public class FormAdminProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Form, AddFormModel>();
            config.NewConfig<AddFormModel, Form>();
            config.NewConfig<Form, UpdateFormModel>();
            config.NewConfig<UpdateFormModel, Form>();
            config.NewConfig<Form, FormMessageTabViewModel>();
            config.NewConfig<FormMessageTabViewModel, Form>();
            config.NewConfig<Form, FormGDPRTabViewModel>();
            config.NewConfig<FormGDPRTabViewModel, Form>();
            config.NewConfig<Form, FormDesignTabViewModel>();
            config.NewConfig<FormDesignTabViewModel, Form>();
        }
    }
}