using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Paging;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IMediaSelectorService
    {
        IPagedList<MediaFile> Search(MediaSelectorSearchQuery searchQuery);
        List<SelectListItem> GetCategories();
        SelectedItemInfo GetFileInfo(string value);
    }
}