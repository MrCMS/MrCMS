using Mapster;
using MrCMS.Entities.People;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class UserAdminProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<User, UpdateUserModel>();
            config.NewConfig<UpdateUserModel, User>();
            config.NewConfig<User, AddUserModel>();
            config.NewConfig<AddUserModel, User>();
        }
    }
}