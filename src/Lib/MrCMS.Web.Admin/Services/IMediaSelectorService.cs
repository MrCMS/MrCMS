using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface IMediaSelectorService
    {
        IPagedList<MediaFile> Search(MediaSelectorSearchQuery searchQuery);
        List<SelectListItem> GetCategories();
        SelectedItemInfo GetFileInfo(string value);
        string GetAlt(string url);
        string GetDescription(string url);
        bool UpdateAlt(UpdateMediaParams updateMediaParams);
        bool UpdateDescription(UpdateMediaParams updateMediaParams);
    }
}