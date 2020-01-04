using System.Threading.Tasks;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System
{
    public class PreviewMessageTemplateBreadcrumb : Breadcrumb<MessageTemplateBreadcrumb>
    {
        public override string Controller => "MessageTemplatePreview";
        public override string Action => "Get";
        public override string Name => "Preview";

        public override Task Populate()
        {
            ParentActionArguments = ActionArguments;
        }
    }
}