using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Galleries.Pages;
using MrCMS.Web.Areas.Admin.Services;
using NHibernate;

namespace MrCMS.Web.Apps.Galleries.Areas.Admin.Services
{
    public class GetGalleryAdminViewData : BaseAssignWebpageAdminViewData<Gallery>
    {
        private readonly ISession _session;

        public GetGalleryAdminViewData(ISession session)
        {
            _session = session;
        }

        public override void AssignViewData(Gallery webpage, ViewDataDictionary viewData)
        {
            viewData["galleries"] = _session.QueryOver<MediaCategory>()
                .OrderBy(category => category.Name)
                .Desc.Cacheable()
                .List()
                .BuildSelectItemList(category => category.Name,
                    category => category.Id.ToString(),
                    emptyItemText: "Select a gallery...");
        }
    }
}