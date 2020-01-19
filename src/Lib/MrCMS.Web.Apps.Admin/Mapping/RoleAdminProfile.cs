using AutoMapper;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class RoleAdminProfile : Profile
    {
        public RoleAdminProfile()
        {
            CreateMap<Role, AddRoleModel>().ReverseMap();
            CreateMap<Role, UpdateRoleModel>().ReverseMap();
        }
    }
}