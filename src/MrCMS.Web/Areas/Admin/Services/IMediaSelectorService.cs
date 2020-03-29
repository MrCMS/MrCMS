using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Areas.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IMediaSelectorService
    {
        IPagedList<MediaFile> Search(MediaSelectorSearchQuery searchQuery);
        List<SelectListItem> GetCategories();
        Task<SelectedItemInfo> GetFileInfo(string value);
        Task<string> GetAlt(string url);
        Task<string> GetDescription(string url);
        Task<bool> UpdateAlt(UpdateMediaParams updateMediaParams);
        Task<bool> UpdateDescription(UpdateMediaParams updateMediaParams);
    }
}