using MrCMS.Web.Apps.Commenting.Settings;

namespace MrCMS.Web.Apps.Commenting.Services
{
    public interface IProcessCommentWidgetChanges
    {
        void Process(CommentingSettings previousSettings, CommentingSettings newSettings);
    }
}