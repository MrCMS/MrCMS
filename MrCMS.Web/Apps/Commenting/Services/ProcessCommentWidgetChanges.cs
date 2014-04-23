using MrCMS.Web.Apps.Commenting.Models;
using MrCMS.Web.Apps.Commenting.Settings;
using System.Linq;

namespace MrCMS.Web.Apps.Commenting.Services
{
    public class ProcessCommentWidgetChanges : IProcessCommentWidgetChanges
    {
        private readonly IAddCommentWidgets _addCommentWidgets;
        private readonly IRemoveCommentWidgets _removeCommentWidgets;

        public ProcessCommentWidgetChanges(IAddCommentWidgets addCommentWidgets, IRemoveCommentWidgets removeCommentWidgets)
        {
            _addCommentWidgets = addCommentWidgets;
            _removeCommentWidgets = removeCommentWidgets;
        }

        public virtual CommentedEnabledChangedResult GetChanges(CommentingSettings previousSettings,
                                                                   CommentingSettings newSettings)
        {
            return new CommentedEnabledChangedResult
                       {
                           Added = newSettings.AllowedTypes.Except(previousSettings.AllowedTypes).ToList(),
                           Removed = previousSettings.AllowedTypes.Except(newSettings.AllowedTypes).ToList()
                       };
        }

        public void Process(CommentingSettings previousSettings, CommentingSettings newSettings)
        {
            var changed = GetChanges(previousSettings, newSettings);

            foreach (var type in changed.Added)
                _addCommentWidgets.AddWidgets(type);

            foreach (var type in changed.Removed)
                _removeCommentWidgets.RemoveWidgets(type);
        }
    }
}