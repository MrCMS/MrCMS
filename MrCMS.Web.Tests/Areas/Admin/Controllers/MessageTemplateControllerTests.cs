using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
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
        private IMessageTemplateService _messageTemplateService;
        private MessageTemplateController _messageTemplateController;

        public MessageTemplateControllerTests()
        {
            CurrentRequestData.OverridenContext = A.Fake<HttpContextBase>();
            CurrentRequestData.CurrentUser = new User();
            _messageTemplateService = A.Fake<IMessageTemplateService>();
            _messageTemplateController = new MessageTemplateController(_messageTemplateService)
                                  {
                                      RequestMock = A.Fake<HttpRequestBase>()
                                  };
        }

        [Fact]
        public void MessageTemplateController_Index_ShouldReturnViewResult()
        {
            var result=_messageTemplateController.Index();

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
            var items = new Dictionary<Type, int>()
                {
                    { typeof(BasicMappedResetPasswordMessageTemplate), 0}
                };
            A.CallTo(() => _messageTemplateService.GetAllMessageTemplateTypesWithDetails()).Returns(items);

            var result = _messageTemplateController.Index();

            result.As<ViewResult>().Model.Should().BeSameAs(items);
        }

        //[Fact]
        //public void MessageTemplateController_Add_ShouldReturnViewResult()
        //{
        //    var type = typeof(BasicMappedResetPasswordMessageTemplate);
        //    A.CallTo(() => TypeHelper.GetTypeByClassName(type.ToString())).Returns(type);

        //    var result = _messageTemplateController.Add(type.ToString());

        //    result.Should().BeOfType<ViewResult>();
        //}

        //[Fact]
        //public void MessageTemplateController_Add_ShouldReturnTheResultOfServiceCallAsModel()
        //{
        //    var item = new BasicMappedResetPasswordMessageTemplate();

        //    var result = _messageTemplateController.Add(typeof(BasicMappedResetPasswordMessageTemplate).ToString());

        //    result.As<ViewResult>().Model.Should().BeSameAs(item);
        //}

        [Fact]
        public void MessageTemplateController_Add_ShouldRedirectToIndex()
        {
            var result = _messageTemplateController.Add(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        [Fact]
        public void MessageTemplateController_AddPost_ShouldCallMessageTemplateServiceSave()
        {
            var messageTemplate = new BasicMappedResetPasswordMessageTemplate();

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
            var messageTemplate = new BasicMappedResetPasswordMessageTemplate();

            _messageTemplateController.Edit_POST(messageTemplate);

            A.CallTo(() => _messageTemplateService.Save(messageTemplate)).MustHaveHappened();
        }

        [Fact]
        public void MessageTemplateController_EditPost_ShouldRedirectToIndex()
        {
            var result = _messageTemplateController.Edit_POST(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }

        //[Fact]
        //public void MessageTemplateController_Reset_ShouldCallMessageTemplateServiceSave()
        //{
        //    var messageTemplate = new BasicMappedResetPasswordMessageTemplate();

        //    _messageTemplateController.Reset(messageTemplate);

        //    A.CallTo(() => _messageTemplateService.Save(messageTemplate)).MustHaveHappened();
        //}

        //[Fact]
        //public void MessageTemplateController_Reset_ShouldResetMessageTemplate()
        //{
        //    var messageTemplate = new BasicMappedResetPasswordMessageTemplate().GetInitialTemplate();
        //    var oldMessageTemplate = new BasicMappedResetPasswordMessageTemplate(){FromAddress = "mrcms@thought.co.uk"};

        //    var result=_messageTemplateController.Reset(oldMessageTemplate);

        //    result.As<ViewResult>().Model.Should().Be(messageTemplate);
        //}

        [Fact]
        public void MessageTemplateController_Reset_ShouldRedirectToIndex()
        {
            var result = _messageTemplateController.Reset(null);

            result.As<RedirectToRouteResult>().RouteValues["action"].Should().Be("Index");
        }
    }
}