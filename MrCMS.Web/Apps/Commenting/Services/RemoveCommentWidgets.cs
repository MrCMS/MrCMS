using System;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Commenting.Extensions;
using MrCMS.Web.Apps.Commenting.Widgets;
using NHibernate;

namespace MrCMS.Web.Apps.Commenting.Services
{
    public class RemoveCommentWidgets : IRemoveCommentWidgets
    {
        private readonly ISession _session;

        public RemoveCommentWidgets(ISession session)
        {
            _session = session;
        }

        public void RemoveWidgets(Type type)
        {
            var webpages = _session.CreateCriteria(type).List<Webpage>();
            _session.Transact(session =>
                                  {
                                      foreach (var webpage in webpages)
                                      {
                                          var layoutArea = webpage.GetCommentsLayoutArea();
                                          if (layoutArea == null) continue;
                                          var commentingWidget =
                                              webpage.Widgets.OfType<CommentingWidget>()
                                                     .FirstOrDefault(widget => widget.LayoutArea == layoutArea);
                                          if (commentingWidget == null) 
                                              continue;
                                          webpage.Widgets.Remove(commentingWidget);
                                          session.Delete(commentingWidget);
                                      }
                                  });
        }
    }
}