using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

namespace MrCMS.Shortcodes.Forms
{
    public interface IDefaultFormRenderer
    {
        Task<IHtmlContent> GetDefault(IHtmlHelper helper, Form formEntity, FormSubmittedStatus submittedStatus);
    }
}