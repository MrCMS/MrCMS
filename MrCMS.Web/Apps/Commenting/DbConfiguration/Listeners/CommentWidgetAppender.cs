using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Commenting.Extensions;
using MrCMS.Web.Apps.Commenting.Settings;
using MrCMS.Web.Apps.Commenting.Widgets;
using MrCMS.Website;
using NHibernate;
using NHibernate.Event;
using Ninject;

namespace MrCMS.Web.Apps.Commenting.DbConfiguration.Listeners
{
    public class CommentWidgetAppender : IPostInsertEventListener
    {
        public void OnPostInsert(PostInsertEvent @event)
        {
            if (@event.Entity is Webpage && CurrentRequestData.DatabaseIsInstalled)
            {
                CurrentRequestData.OnEndRequest.Add(kernel =>
                                                    {
                                                        var session = kernel.Get<ISession>();
                                                        var webpage = @event.Entity as Webpage;
                                                        if (webpage == null ||  !kernel.Get<CommentingSettings>().IsAllowedType(webpage))
                                                            return;

                                                        var layoutArea = webpage.GetCommentsLayoutArea();
                                                        if (layoutArea == null) return;
                                                        if (webpage.Widgets.OfType<CommentingWidget>().Any(widget => widget.LayoutArea == layoutArea))
                                                            return;
                                                        var commentingWidget = new CommentingWidget
                                                        {
                                                            Webpage = webpage,
                                                            LayoutArea = layoutArea,
                                                        };
                                                        webpage.Widgets.Add(commentingWidget);
                                                        session.Transact(s => s.Save(commentingWidget));
                                                    });
            }
        }
    }
}