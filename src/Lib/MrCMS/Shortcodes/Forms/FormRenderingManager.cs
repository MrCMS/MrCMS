using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Shortcodes.Forms
{
    public class FormRenderingManager : IFormRenderer
    {
        private readonly IDefaultFormRenderer _defaultFormRenderer;
        private readonly ICustomFormRenderer _customFormRenderer;

        public FormRenderingManager(IDefaultFormRenderer defaultFormRenderer, ICustomFormRenderer customFormRenderer)
        {
            _defaultFormRenderer = defaultFormRenderer;
            _customFormRenderer = customFormRenderer;
        }

        public Task<IHtmlContent> RenderForm(IHtmlHelper helper, Form form, FormSubmittedStatus submitted)
        {
            if (form == null)
                return Task.FromResult<IHtmlContent>(HtmlString.Empty);
            return string.IsNullOrWhiteSpace(form.FormDesign)
                       ? _defaultFormRenderer.GetDefault(helper, form, submitted)
                       : _customFormRenderer.GetForm(helper, form, submitted);
        }
    }
}