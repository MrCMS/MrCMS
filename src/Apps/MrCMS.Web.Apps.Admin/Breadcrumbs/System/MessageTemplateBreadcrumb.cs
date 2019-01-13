using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.System
{
    public class MessageTemplateBreadcrumb : Breadcrumb<MessageTemplatesBreadcrumb>
    {
        public override string Controller => "";
        public override string Action => "";
        public override bool IsPlaceHolder => true;

        public override void Populate()
        {
            var type = ActionArguments["type"] as string;
            var typeByName = TypeHelper.GetTypeByName(type);
            Name = typeByName.Name.BreakUpString();
        }
    }
}