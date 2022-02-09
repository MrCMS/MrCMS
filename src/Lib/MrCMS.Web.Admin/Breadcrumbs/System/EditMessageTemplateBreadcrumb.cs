using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System
{
    public class EditMessageTemplateBreadcrumb : Breadcrumb<MessageTemplateBreadcrumb>
    {
        public override string Controller => "MessageTemplate";
        public override string Action => "Edit";
        public override string Name => "Edit";

        public override void Populate()
        {
            ParentActionArguments = ActionArguments;
        }
    }
}