using System.Threading.Tasks;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.System
{
    public class EditMessageTemplateBreadcrumb : Breadcrumb<MessageTemplateBreadcrumb>
    {
        public override string Controller => "MessageTemplate";
        public override string Action => "Edit";
        public override string Name => "Edit";

        public override Task Populate()
        {
            ParentActionArguments = ActionArguments;
            return Task.CompletedTask;
        }
    }
}