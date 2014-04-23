using System;
using FluentAssertions;
using MrCMS.Commenting.Tests.Stubs;
using MrCMS.Commenting.Tests.Support;
using MrCMS.Tasks;
using MrCMS.Web;
using MrCMS.Web.Apps.Commenting.Settings;
using MrCMS.Web.Apps.Commenting.Widgets;
using MrCMS.Website;
using NHibernate;
using Xunit;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Commenting.Extensions;
using System.Linq;

namespace MrCMS.Commenting.Tests.DbConfiguration.Listeners.CommentWidgetAppenderTests
{
    public class OnPostInsert : InMemoryDatabaseTest
    {
        [Fact]
        public void AddsAWidgetToThePagesCommentsAreaAtEndRequest()
        {
            var commentingSettings = new CommentingSettings().SetAllowedPageTypes(typeof(BasicMappedWebpage));
            Kernel.Rebind<CommentingSettings>().ToConstant(commentingSettings);
            Kernel.Rebind<ISession>().ToMethod(context => Session);
            var webpage = new CommentingMappedWebpageBuilder().WithCommentsArea().Build();

            Session.Transact(session => session.Save(webpage));
            CurrentRequestData.OnEndRequest.ForEach(action => action(Kernel));
            
            webpage.Widgets.OfType<CommentingWidget>().Should().HaveCount(1);
        }
    }
}