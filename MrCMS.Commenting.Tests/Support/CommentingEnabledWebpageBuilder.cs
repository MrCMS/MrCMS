using System.Collections.Generic;
using MrCMS.Commenting.Tests.Stubs;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Web.Apps.Commenting;
using MrCMS.Web.Apps.Commenting.Extensions;
using MrCMS.Web.Apps.Commenting.Widgets;

namespace MrCMS.Commenting.Tests.Support
{
    public class CommentingEnabledWebpageBuilder
    {
        private int _id;

        public BasicMappedWebpage Build()
        {
            var basicMappedWebpage = new BasicMappedWebpage
                                         {
                                             Id = _id,
                                             Layout = new Layout
                                                          {
                                                              LayoutAreas = new List<LayoutArea>
                                                                                {
                                                                                    new LayoutArea
                                                                                        {
                                                                                            AreaName = CommentingApp .LayoutAreaName
                                                                                        }
                                                                                }
                                                          }
                                         };
            var commentsLayoutArea = basicMappedWebpage.GetCommentsLayoutArea();
            if (commentsLayoutArea != null)
            {
                var commentingWidget = new CommentingWidget
                                           {
                                               Webpage = basicMappedWebpage,
                                               LayoutArea = commentsLayoutArea,
                                           };

                basicMappedWebpage.Widgets.Add(commentingWidget);
            }
            return basicMappedWebpage;
        }

        public CommentingEnabledWebpageBuilder WithId(int id)
        {
            _id = id;
            return this;
        }
    }
}