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

        public IHtmlContent RenderForm(IHtmlHelper helper, Webpage webpage, FormSubmittedStatus submitted)
        {
            if (webpage == null)
                return HtmlString.Empty;
            return string.IsNullOrWhiteSpace(webpage.FormDesign)
                       ? _defaultFormRenderer.GetDefault(helper, webpage, submitted)
                       : _customFormRenderer.GetForm(helper, webpage, submitted);
        }
    }
}