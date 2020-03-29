using AutoMapper;
using MrCMS.Entities.People;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Mapping
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