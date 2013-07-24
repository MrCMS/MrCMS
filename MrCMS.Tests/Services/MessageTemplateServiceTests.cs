using System;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Messaging;
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
        private MessageTemplateService _messageTemplateService;

        public MessageTemplateServiceTests()
        {
            _messageTemplateService = new MessageTemplateService(Session);
        }

        [Fact]
        public void MessageTemplateService_Save_SavesAMessageTemplateToSession()
        {
            var messageTemplate = new BasicMappedResetPasswordMessageTemplate().GetInitialTemplate();

            _messageTemplateService.Save(messageTemplate);

            Session.QueryOver<MessageTemplate>().RowCount().Should().Be(1);
        }

        [Fact]
        public void MessageTemplateService_GetAll_ShouldReturnTheCollectionOfMessageTemplates()
        {
            Enumerable.Range(1, 1).ForEach(i => Session.Transact(s => s.SaveOrUpdate(new BasicMappedResetPasswordMessageTemplate().GetInitialTemplate())));

            var items = _messageTemplateService.GetAll();

            items.Should().NotBeEmpty();
        }

        [Fact]
        public void MessageTemplateService_GetAllMessageTemplateTypesWithDetails_ShouldReturnTheCollectionOfMessageTemplates()
        {
            var items = _messageTemplateService.GetAllMessageTemplateTypesWithDetails();

            items.Should().NotBeEmpty();
        }

        [Fact]
        public void MessageTemplateService_GetNew_IfTypeIsNullReturnNull()
        {
            var messageTemplate = _messageTemplateService.GetNew(null);

            messageTemplate.Should().BeNull();
        }

        [Fact]
        public void MessageTemplateService_GetNew_IfValidTypeIsPassedReturnsTemplate()
        {
            var messageTemplate = _messageTemplateService.GetNew(typeof(BasicMappedResetPasswordMessageTemplate).Name);

            messageTemplate.Should().NotBeNull();
        }

        [Fact]
        public void MessageTemplateService_Reset_ShouldResetMessageTemplateToInitialTemplate()
        {
            var messageTemplate = new BasicMappedResetPasswordMessageTemplate() { ToAddress = "info@thought.co.uk" };

            var result=_messageTemplateService.Reset(messageTemplate);

            result.ToAddress.Should().Be("{Email}");
        }
    }
}