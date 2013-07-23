using System;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tests.Stubs;
using MrCMS.Website;
using NHibernate;
using Ninject.MockingKernel;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class MessageTemplateServiceTests : InMemoryDatabaseTest
    {
        [Fact]
        public void MessageTemplateService_Save_SavesAMessageTemplateToSession()
        {
            FakeCurrentRequestDataAndMockKernel();
            var session = A.Fake<ISession>();
            var service = new MessageTemplateService(session);

            var messageTemplate = new BasicMappedResetPasswordMessageTemplate().GetInitialTemplate();

            service.Save(messageTemplate);

            A.CallTo(() => session.SaveOrUpdate(messageTemplate)).MustHaveHappened();
        }

        [Fact]
        public void MessageTemplateService_GetAll_ShouldReturnTheCollectionOfMessageTemplates()
        {
            FakeCurrentRequestDataAndMockKernel();
            var service = new MessageTemplateService(Session);

            Enumerable.Range(1, 1).ForEach(i =>Session.Transact(s => s.SaveOrUpdate(new BasicMappedResetPasswordMessageTemplate().GetInitialTemplate())));

            var items = service.GetAll();

            items.Should().NotBeEmpty();
        }

        [Fact]
        public void MessageTemplateService_GetAllMessageTemplateTypesWithDetails_ShouldReturnTheCollectionOfMessageTemplates()
        {
            FakeCurrentRequestDataAndMockKernel();
            var service = new MessageTemplateService(Session);

            var items = service.GetAllMessageTemplateTypesWithDetails();

            items.Should().NotBeEmpty();
        }

        private static void FakeCurrentRequestDataAndMockKernel()
        {
            var mockingKernel = new MockingKernel();
            var mailSettings = A.Fake<MailSettings>();
            mockingKernel.Bind<MailSettings>().ToMethod(context => mailSettings).InSingletonScope();
            MrCMSApplication.OverrideKernel(mockingKernel);
            A.CallTo(() => CurrentRequestData.CurrentContext.Request.Url).Returns(new Uri("http://localhost:8888/"));
        }
    }
}