using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Events;
using MrCMS.Events.Documents;
using MrCMS.Helpers;
using MrCMS.Tasks;
using MrCMS.Web.Apps.Commenting.Extensions;
using MrCMS.Web.Apps.Commenting.Settings;
using MrCMS.Web.Apps.Commenting.Widgets;
using NHibernate;
using System.Linq;

namespace MrCMS.Web.Apps.Commenting.Tasks
{
    public class AppendCommentWidget : IOnDocumentAdded
    {
        private readonly ISession _session;
        private readonly CommentingSettings _commentingSettings;

        public AppendCommentWidget(ISession session, CommentingSettings commentingSettings)
        {
            _session = session;
            _commentingSettings = commentingSettings;
        }

        public void Execute(OnDocumentAddedEventArgs args)
        {
            var webpage = args.Document as Webpage;
            if (webpage == null)
                return;
            if (!_commentingSettings.AllowedTypes.Contains(webpage.GetType()))
                return;
            var layoutArea = webpage.GetCommentsLayoutArea();
            if (layoutArea == null)
                return;
            var commentingWidget = new CommentingWidget
            {
                Webpage = webpage,
                LayoutArea = layoutArea
            };
            webpage.Widgets.Add(commentingWidget);
            layoutArea.AddWidget(commentingWidget);
            _session.Transact(session => session.Save(commentingWidget));
        }
    }
}