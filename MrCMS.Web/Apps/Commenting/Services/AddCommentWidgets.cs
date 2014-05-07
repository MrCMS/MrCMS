using System;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Commenting.Widgets;
using NHibernate;
using MrCMS.Web.Apps.Commenting.Extensions;
using MrCMS.Helpers;
using System.Linq;

namespace MrCMS.Web.Apps.Commenting.Services
{
    public class AddCommentWidgets : IAddCommentWidgets
    {
        private readonly ISession _session;

        public AddCommentWidgets(ISession session)
        {
            _session = session;
        }

        public void AddWidgets(Type type)
        {
            var webpages = _session.CreateCriteria(type).List<Webpage>();
            _session.Transact(session =>
                                  {
                                      foreach (var webpage in webpages)
                                      {
                                          var layoutArea = webpage.GetCommentsLayoutArea();
                                          if (layoutArea == null) continue;
                                          if (webpage.Widgets.OfType<CommentingWidget>().Any(widget => widget.LayoutArea == layoutArea))
                                              continue;
                                          var commentingWidget = new CommentingWidget
                                                                     {
                                                                         Webpage = webpage,
                                                                         LayoutArea = layoutArea,
                                                                     };
                                          webpage.Widgets.Add(commentingWidget);
                                          session.Save(commentingWidget);
                                      }
                                  });
        }
    }
}