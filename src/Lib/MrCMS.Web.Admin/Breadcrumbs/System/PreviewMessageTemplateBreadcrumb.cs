using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System
{
    public class PreviewMessageTemplateBreadcrumb : Breadcrumb<MessageTemplateBreadcrumb>
    {
        public override string Controller => "MessageTemplatePreview";
        public override string Action => "Get";
        public override string Name => "Preview";

        public override void Populate()
        {
            ParentActionArguments = ActionArguments;
        }
    }
}