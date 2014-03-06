using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website.Controllers;
using NHibernate;
using MrCMS.Helpers;
using NHibernate.Criterion;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class MediaSelectorController : MrCMSAdminController
    {
        private readonly IMediaSelectorService _mediaSelectorService;

        public MediaSelectorController(IMediaSelectorService mediaSelectorService)
        {
            _mediaSelectorService = mediaSelectorService;
        }

        public ActionResult Show(MediaSelectorSearchQuery searchQuery)
        {
            ViewData["categories"] = _mediaSelectorService.GetCategories();
            ViewData["results"] = _mediaSelectorService.Search(searchQuery);
            return PartialView(searchQuery);
        }


        public JsonResult GetFileInfo(string value)
        {
            return Json(_mediaSelectorService.GetFileInfo(value), JsonRequestBehavior.AllowGet);
        }
    }

    public interface IMediaSelectorService
    {
        IPagedList<MediaFile> Search(MediaSelectorSearchQuery searchQuery);
        List<SelectListItem> GetCategories();
        SelectedItemInfo GetFileInfo(string value);
    }

    public class SelectedItemInfo
    {
        public string Url { get; set; }
    }

    public class MediaSelectorService : IMediaSelectorService
    {
        private readonly ISession _session;
        private readonly IFileService _fileService;
        private readonly Site _site;

        public MediaSelectorService(ISession session, IFileService fileService, Site site)
        {
            _session = session;
            _fileService = fileService;
            _site = site;
        }

        public IPagedList<MediaFile> Search(MediaSelectorSearchQuery searchQuery)
        {
            var queryOver = _session.QueryOver<MediaFile>().Where(file => file.Site.Id == _site.Id);
            if (searchQuery.CategoryId.HasValue)
                queryOver = queryOver.Where(file => file.MediaCategory.Id == searchQuery.CategoryId);
            if (!string.IsNullOrWhiteSpace(searchQuery.Query))
            {
                var term = searchQuery.Query.Trim();
                queryOver =
                    queryOver.Where(
                        file =>
                        file.FileName.IsLike(term, MatchMode.Anywhere) ||
                        file.Title.IsLike(term, MatchMode.Anywhere) ||
                        file.Description.IsLike(term, MatchMode.Anywhere));
            }
            return queryOver.OrderBy(file => file.CreatedOn).Desc.Paged(searchQuery.Page);
        }

        public List<SelectListItem> GetCategories()
        {
            return _session.QueryOver<MediaCategory>()
                .Where(category => category.Site.Id == _site.Id)
                .Where(category => !category.HideInAdminNav || category.HideInAdminNav == null)
                .Cacheable()
                .List()
                .BuildSelectItemList(category => category.Name, category => category.Id.ToString(),
                    emptyItemText: "All categories");
        }

        public SelectedItemInfo GetFileInfo(string value)
        {
            var fileUrl = _fileService.GetFileUrl(value);

            return new SelectedItemInfo
                       {
                           Url = fileUrl
                       };
        }
    }

    public class MediaSelectorSearchQuery
    {
        public MediaSelectorSearchQuery()
        {
            Page = 1;
        }
        public int Page { get; set; }
        public int? CategoryId { get; set; }

        public string Query { get; set; }
    }
}