using AutoMapper;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Mapping
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