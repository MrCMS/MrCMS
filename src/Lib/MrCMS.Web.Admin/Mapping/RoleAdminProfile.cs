using Mapster;
using MrCMS.Entities.People;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class RoleAdminProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<UserRole, AddRoleModel>();
            config.NewConfig<AddRoleModel, UserRole>();
            config.NewConfig<UserRole, UpdateRoleModel>();
            config.NewConfig<UpdateRoleModel, UserRole>();
        }
    }
}