using FakeItEasy;
using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Web.Apps.Commenting.Services;

namespace MrCMS.Commenting.Tests.Services.ProcessCommentWidgetChangesTests
{
    public class ProcessCommentWidgetChangesBuilder
    {
        private IAddCommentWidgets _addCommentWidgets = A.Fake<IAddCommentWidgets>();
        private IRemoveCommentWidgets _removeCommentWidgets = A.Fake<IRemoveCommentWidgets>();
        private CommentedEnabledChangedResult _changedResult;

        public ProcessCommentWidgetChangesBuilder WithChangedResult(CommentedEnabledChangedResult changedResult)
        {
            _changedResult = changedResult;
            return this;
        }

        public ProcessCommentWidgetChangesBuilder WithAddCommentWidgets(IAddCommentWidgets addCommentWidgets)
        {
            _addCommentWidgets = addCommentWidgets;
            return this;
        }

        public ProcessCommentWidgetChangesBuilder WithRemoveCommentWidgets(IRemoveCommentWidgets removeCommentWidgets)
        {
            _removeCommentWidgets = removeCommentWidgets;
            return this;
        }

        public ProcessCommentWidgetChanges Build()
        {
            return new TestableProcessCommentWidgetChanges(_addCommentWidgets, _removeCommentWidgets)
                       {
                           ChangedResult = _changedResult
                       };
        }
    }
}