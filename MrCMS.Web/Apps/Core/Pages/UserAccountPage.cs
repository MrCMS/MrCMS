using MrCMS.Entities.Documents.Metadata;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Core.Pages
{
    public class UserAccountPage : TextPage, IUniquePage
    {

    }

    public class UserAccountMetaData : DocumentMetadataMap<UserAccountPage>
    {
        public override string IconClass
        {
            get { return "icon-user"; }
        }

        public override string WebGetAction
        {
            get { return "Show"; }
        }
        public override string WebGetController
        {
            get
            {
                return "UserAccount";
            }
        }
    }
}