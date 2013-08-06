using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Messaging;
using MrCMS.Services;
using MrCMS.Tests.Stubs;
using NHibernate;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class MessageTemplateServiceTests : InMemoryDatabaseTest
    {
        private readonly IMessageTemplateParser _messageTemplateParser;
        private readonly MessageTemplateService _messageTemplateService;

        public MessageTemplateServiceTests()
        {
            _messageTemplateParser = A.Fake<IMessageTemplateParser>();
            _messageTemplateService = new MessageTemplateService(Session, _messageTemplateParser);
        }

        [Fact]
        public void MessageTemplateService_Save_SavesAMessageTemplateToSession()
        {
            var messageTemplate = new BasicMessageTemplate().GetInitialTemplate(A<ISession>._);

            _messageTemplateService.Save(messageTemplate);

            Session.QueryOver<MessageTemplate>().RowCount().Should().Be(1);
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
            var messageTemplate = _messageTemplateService.GetNew(typeof(BasicMessageTemplate).Name);

            messageTemplate.Should().NotBeNull();
        }

        [Fact]
        public void MessageTemplateService_Reset_ShouldResetMessageTemplateToInitialTemplate()
        {
            var messageTemplate = new BasicMessageTemplate() { ToAddress = "info@thought.co.uk" };

            var result = _messageTemplateService.Reset(messageTemplate);

            result.ToAddress.Should().Be("{Email}");
        }
    }
}