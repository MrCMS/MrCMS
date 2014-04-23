using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Web.Apps.Commenting.Services;
using MrCMS.Web.Apps.Commenting.Settings;

namespace MrCMS.Commenting.Tests.Services.ProcessCommentWidgetChangesTests
{
    public class TestableProcessCommentWidgetChanges : ProcessCommentWidgetChanges
    {
        public TestableProcessCommentWidgetChanges(IAddCommentWidgets addCommentWidgets,
                                                   IRemoveCommentWidgets removeCommentWidgets)
            : base(addCommentWidgets, removeCommentWidgets)
        {
        }

        public override CommentedEnabledChangedResult GetChanges(CommentingSettings previousSettings, CommentingSettings newSettings)
        {
            return ChangedResult ?? base.GetChanges(previousSettings, newSettings);
        }

        public CommentedEnabledChangedResult ChangedResult { get; set; }
    }
}