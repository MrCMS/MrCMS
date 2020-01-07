﻿using System.Threading.Tasks;
using MrCMS.Entities;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Admin.Infrastructure.Models;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Services
{
    public interface IUserProfileAdminService
    {
        Task<T> Add<T, TModel>(TModel model) where T : UserProfileData where TModel : IAddUserProfileDataModel;
        Task<T> Update<T, TModel>(TModel model) where T : UserProfileData where TModel : IHaveId;
        Task Delete<T>(int id) where T : UserProfileData;
        Task<TModel> GetEditModel<T, TModel>(int id) where T : UserProfileData where TModel : IHaveId;
    }
}
