using System.Collections.Generic;
using MrCMS.Commenting.Tests.Stubs;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Web.Apps.Commenting;
using MrCMS.Web.Apps.Commenting.Extensions;
using MrCMS.Web.Apps.Commenting.Widgets;

namespace MrCMS.Commenting.Tests.Support
{
    public class CommentingMappedWebpageBuilder
    {
        private bool _withCommentsArea;
        private bool _withCommentsWidget;
        private bool _withCommentingDisabled;

        public BasicMappedWebpage Build()
        {
            var basicMappedWebpage = new BasicMappedWebpage();
            if (_withCommentsArea)
            {
                basicMappedWebpage.Layout = new Layout
                                                {
                                                    LayoutAreas = new List<LayoutArea>
                                                                      {
                                                                          new LayoutArea
                                                                              {
                                                                                  AreaName =
                                                                                      CommentingApp.LayoutAreaName
                                                                              }
                                                                      }
                                                };
            }
            if (_withCommentsWidget)
            {
                var commentsLayoutArea = basicMappedWebpage.GetCommentsLayoutArea();
                if (commentsLayoutArea != null)
                {
                    var commentingWidget = new CommentingWidget
                                               {
                                                   Webpage = basicMappedWebpage,
                                                   LayoutArea = commentsLayoutArea,
                                                   CommentingDisabled = _withCommentingDisabled
                                               };

                    basicMappedWebpage.Widgets.Add(commentingWidget);
                }
            }
            return basicMappedWebpage;
        }

        public CommentingMappedWebpageBuilder WithCommentsArea()
        {
            _withCommentsArea = true;
            return this;
        }

        public CommentingMappedWebpageBuilder WithCommentsWidget()
        {
            _withCommentsWidget = true;
            return this;
        }

        public CommentingMappedWebpageBuilder WithCommentingDisabled()
        {
            _withCommentingDisabled = true;
            return this;
        }
    }
}