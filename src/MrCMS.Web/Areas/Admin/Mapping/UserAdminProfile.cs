using AutoMapper;
using MrCMS.Entities.People;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Mapping
{
    public class UserAdminProfile : Profile
    {
        public UserAdminProfile()
        {
            CreateMap<User, UpdateUserModel>().ReverseMap();
            CreateMap<User, AddUserModel>().ReverseMap();
        }
    }
}