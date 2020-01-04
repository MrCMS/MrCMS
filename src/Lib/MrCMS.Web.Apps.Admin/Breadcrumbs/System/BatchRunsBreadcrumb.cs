using System.Threading.Tasks;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System
{
    public class BatchRunsBreadcrumb : Breadcrumb<BatchBreadcrumb>
    {
        public override string Controller => "";
        public override string Action => "";
        public override string Name => "Runs";
        public override bool IsPlaceHolder => true;

        public override Task Populate()
        {
            ParentActionArguments = ActionArguments;
            return Task.CompletedTask;
        }
    }
}