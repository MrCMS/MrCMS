using AutoMapper;
using MrCMS.Entities.People;
using MrCMS.Models;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class UserAdminProfile : Profile
    {
        public UserAdminProfile()
        {
            CreateMap<User, UpdateUserModel>().ReverseMap();
        }
    }
}