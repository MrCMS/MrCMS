using AutoMapper;
using MrCMS.Entities.People;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class RoleAdminProfile : Profile
    {
        public RoleAdminProfile()
        {
            CreateMap<UserRole, AddRoleModel>().ReverseMap();
            CreateMap<UserRole, UpdateRoleModel>().ReverseMap();
        }
    }
}