using System.Web;
using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IStringResourceUpdateService
    {
        FileResult Export(StringResourceSearchQuery searchQuery);
        void Import(HttpPostedFileBase file);
    }
}