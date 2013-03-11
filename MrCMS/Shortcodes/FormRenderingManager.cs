using MrCMS.Entities.Documents.Web;

namespace MrCMS.Shortcodes
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

        public string RenderForm(Webpage webpage)
        {
            if (webpage == null)
                return string.Empty;
            if (string.IsNullOrWhiteSpace(webpage.FormDesign))
            {
                return _defaultFormRenderer.GetDefault(webpage);
            }
            return _customFormRenderer.GetForm(webpage);
        }
    }
}