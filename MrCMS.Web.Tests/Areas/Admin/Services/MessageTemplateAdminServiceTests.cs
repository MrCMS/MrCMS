using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Messaging;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.Stubs;
using NHibernate;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    //public class MessageTemplateAdminServiceTests : InMemoryDatabaseTest
    //{
    //    private readonly IMessageTemplateParser _messageTemplateParser;
    //    private readonly MessageTemplateAdminService _messageTemplateAdminService;

    //    public MessageTemplateAdminServiceTests()
    //    {
    //        _messageTemplateParser = A.Fake<IMessageTemplateParser>();
    //        _messageTemplateAdminService = new MessageTemplateAdminService(Session, CurrentSite, _messageTemplateParser);
    //    }

    //    [Fact]
    //    public void MessageTemplateAdminService_Save_SavesAMessageTemplateToSession()
    //    {
    //        MessageTemplate messageTemplate = new BasicMessageTemplate().GetInitialTemplate(Session);

    //        _messageTemplateAdminService.Save(messageTemplate);

    //        Session.QueryOver<MessageTemplate>().RowCount().Should().Be(1);
    //    }

    //    [Fact]
    //    public void
    //        MessageTemplateAdminService_GetAllMessageTemplateTypesWithDetails_ShouldReturnTheCollectionOfMessageTemplates()
    //    {
    //        List<MessageTemplateInfo> items = _messageTemplateAdminService.GetAllMessageTemplateTypesWithDetails();

    //        items.Should().NotBeEmpty();
    //    }

    //    [Fact]
    //    public void MessageTemplateAdminService_GetNew_IfTypeIsNullReturnNull()
    //    {
    //        MessageTemplate messageTemplate = _messageTemplateAdminService.GetNew(null);

    //        messageTemplate.Should().BeNull();
    //    }

    //    [Fact]
    //    public void MessageTemplateAdminService_GetNew_IfValidTypeIsPassedReturnsTemplate()
    //    {
    //        MessageTemplate messageTemplate = _messageTemplateAdminService.GetNew(typeof (BasicMessageTemplate).FullName);

    //        messageTemplate.Should().NotBeNull();
    //    }

    //    [Fact]
    //    public void MessageTemplateAdminService_Reset_ShouldResetMessageTemplateToInitialTemplate()
    //    {
    //        var messageTemplate = new BasicMessageTemplate {ToAddress = "info@thought.co.uk"};

    //        MessageTemplate result = _messageTemplateAdminService.Reset(messageTemplate);

    //        result.ToAddress.Should().Be("{Email}");
    //    }
    //}
}