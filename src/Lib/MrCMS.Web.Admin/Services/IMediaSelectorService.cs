using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface IMediaSelectorService
    {
        Task<IPagedList<MediaFile>> Search(MediaSelectorSearchQuery searchQuery);
        Task<List<SelectListItem>> GetCategories();
        Task<SelectedItemInfo> GetFileInfo(string value);
        Task<string> GetAlt(string url);
        Task<string> GetDescription(string url);
        Task<bool> UpdateAlt(UpdateMediaParams updateMediaParams, CancellationToken token);
        Task<bool> UpdateDescription(UpdateMediaParams updateMediaParams, CancellationToken token);
    }
}