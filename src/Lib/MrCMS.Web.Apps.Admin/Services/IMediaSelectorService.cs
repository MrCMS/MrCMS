using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Apps.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IMediaSelectorService
    {
        IPagedList<MediaFile> Search(MediaSelectorSearchQuery searchQuery);
        List<SelectListItem> GetCategories();
        Task<SelectedItemInfo> GetFileInfo(string value);
        string GetAlt(string url);
        string GetDescription(string url);
        Task<bool> UpdateAlt(UpdateMediaParams updateMediaParams);
        Task<bool> UpdateDescription(UpdateMediaParams updateMediaParams);
    }
}