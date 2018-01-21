using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;

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

        public string RenderForm(IHtmlHelper helper, Webpage webpage, FormSubmittedStatus submitted)
        {
            if (webpage == null)
                return string.Empty;
            return string.IsNullOrWhiteSpace(webpage.FormDesign)
                       ? _defaultFormRenderer.GetDefault(helper, webpage, submitted)
                       : _customFormRenderer.GetForm(helper, webpage, submitted);
        }
    }
}