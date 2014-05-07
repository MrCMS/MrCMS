using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Tests.Stubs;
using MrCMS.Website;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Controllers
{
    public class MessageTemplateControllerTests
    {
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly MessageTemplateController _messageTemplateController;

        public MessageTemplateControllerTests()
        {
            _messageTemplateService = A.Fake<IMessageTemplateService>();
            _messageTemplateController = new MessageTemplateController(_messageTemplateService)
                                  {
                                      RequestMock = A.Fake<HttpRequestBase>()
                                  };
        }

        [Fact]
        public void MessageTemplateController_Index_ShouldReturnViewResult()
        {
            var result = _messageTemplateController.Index();

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void MessageTemplateController_Index_ShouldCallGetAllMessageTemplateTypesWithDetailsOfMessageTemplateService()
        {
            _messageTemplateController.Index();

            A.CallTo(() => _messageTemplateService.GetAllMessageTemplateTypesWithDetails()).MustHaveHappened();
        }

        [Fact]
        public void MessageTemplateController_Index_ShouldReturnTheResultOfServiceCallAsModel()
        {
            var items = new List<MessageTemplateInfo>();

            A.CallTo(() => _messageTemplateService.GetAllMessageTemplateTypesWithDetails()).Returns(items);

            var result = _messageTemplateController.Index();

            result.As<ViewResult>().Model.Should().Be(items);
        }

        [Fact]
        public void MessageTemplateController_Add_IfTemplateIsFoundShouldReturnViewResult()
        {
            A.CallTo(() => _messageTemplateService.GetNew("test-type")).Returns(A.Fake<MessageTemplate>());

            var result = _messageTemplateController.Add("test-type");

            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void MessageTemplateController_Add_IfTemplateIsFoundShouldReturnTheResultOfServiceCallAsModel()
        {
            var messageTemplate = A.Fake<MessageTemplate>();
            A.CallTo(() => _messageTemplateService.GetNew("test-type")).Returns(messageTemplate);

            var result = _messageTemplateController.Add("test-type");

            result.As<ViewResult>().Model.Should().BeSameAs(messageTemplate);
        }

        [Fact]
        public void MessageTemplateController_Add_IfTemplateIsNotFoundShouldRedirectToIndex()
        {
            A.CallTo(() => _messageTemplateService.GetNew("test-type")).Returns(null);

            var result = _messageTemplateController.Add("test-type");

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void MessageTemplateController_AddPost_ShouldCallMessageTemplateServiceSave()
        {
            var messageTemplate = new BasicMessageTemplate();

            _messageTemplateController.Add_POST(messageTemplate);

            A.CallTo(() => _messageTemplateService.Save(messageTemplate)).MustHaveHappened();
        }

        [Fact]
        public void MessageTemplateController_AddPost_ShouldRedirectToIndex()
        {
            var result = _messageTemplateController.Add_POST(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void MessageTemplateController_EditPost_ShouldCallMessageTemplateServiceSave()
        {
            var messageTemplate = new BasicMessageTemplate();

            _messageTemplateController.Edit_POST(messageTemplate);

            A.CallTo(() => _messageTemplateService.Save(messageTemplate)).MustHaveHappened();
        }

        [Fact]
        public void MessageTemplateController_EditPost_ShouldRedirectToEditForTheMessageTemplate()
        {
            var basicMessageTemplate = new BasicMessageTemplate { Id = 123 };
            var result = _messageTemplateController.Edit_POST(basicMessageTemplate);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Edit");
            result.As<RedirectToRouteResult>().RouteValues["id"].Should().Be(123);
        }

        [Fact]
        public void MessageTemplateController_EditPost_ShouldRedirectToIndexForNullTemplate()
        {
            var result = _messageTemplateController.Edit_POST(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void MessageTemplateController_Reset_ShouldReturnPartialView()
        {
            var messageTemplate = new BasicMessageTemplate();

            var result = _messageTemplateController.Reset(messageTemplate);

            result.Should().BeOfType<PartialViewResult>();
        }

        [Fact]
        public void MessageTemplateController_Reset_ShouldReturnModelAsMessageTemplate()
        {
            var messageTemplate = new BasicMessageTemplate();

            var result = _messageTemplateController.Reset(messageTemplate);

            result.As<PartialViewResult>().Model.Should().BeSameAs(messageTemplate);
        }

        [Fact]
        public void MessageTemplateController_Reset_IfTemplateIsNotFoundShouldReturnRedirectToIndex()
        {
            var result = _messageTemplateController.Reset(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void MessageTemplateController_ResetPost_ShouldCallMessageTemplateServiceReset()
        {
            var messageTemplate = new BasicMessageTemplate();

            _messageTemplateController.Reset_POST(messageTemplate);

            A.CallTo(() => _messageTemplateService.Reset(messageTemplate)).MustHaveHappened();
        }

        [Fact]
        public void MessageTemplateController_ResetPost_IfTemplateIsPassedShouldReturnRedirectToEdit()
        {
            var messageTemplate = new BasicMessageTemplate { Id = 123 };

            var result = _messageTemplateController.Reset_POST(messageTemplate);

            result.RouteValues["action"].Should().Be("Edit");
            result.RouteValues["id"].Should().Be(123);
        }

        [Fact]
        public void MessageTemplateController_ResetPost_IfNoTemplateIsFoundShouldRedirectToIndex()
        {
            var result = _messageTemplateController.Reset_POST(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }
    }
}