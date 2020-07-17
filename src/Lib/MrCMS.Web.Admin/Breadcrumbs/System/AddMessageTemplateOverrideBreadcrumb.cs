using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Admin.Breadcrumbs.System
{
    public class AddMessageTemplateOverrideBreadcrumb : Breadcrumb<MessageTemplateBreadcrumb>
    {
        public override string Controller => "MessageTemplate";
        public override string Action => "AddSiteOverride";
        public override string Name => "Add Site Override";
        public override void Populate()
        {
            ParentActionArguments = ActionArguments;
        }
    }
}