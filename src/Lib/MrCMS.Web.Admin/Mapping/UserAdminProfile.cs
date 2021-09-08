using AutoMapper;
using MrCMS.Entities.People;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
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