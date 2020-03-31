using MrCMS.TestSupport;

namespace MrCMS.Web.Apps.Admin.Tests.Controllers
{
    public class WidgetControllerTests : MrCMSTest
    {
        // todo - rewrite tests and refactor

        //private readonly WidgetController _widgetController;
        //private readonly IWidgetAdminService _widgetService;
        //private readonly ISetWidgetAdminViewData _setAdminViewData;
        //private readonly IModelBindingHelperAdapter _modelBindingHelperAdapter;

        //public WidgetControllerTests()
        //{
        //    _widgetService = A.Fake<IWidgetAdminService>();
        //    _setAdminViewData = A.Fake<ISetWidgetAdminViewData>();
        //    _modelBindingHelperAdapter = A.Fake<IModelBindingHelperAdapter>();
        //    _widgetController = new WidgetController(_widgetService, _setAdminViewData, _modelBindingHelperAdapter);
        //}

        //[Fact]
        //public void WidgetController_EditGet_ShouldReturnTheLoadedModel()
        //{
        //    UpdateWidgetModel model = new UpdateWidgetModel();
        //    A.CallTo(() => _widgetService.GetEditModel(123)).Returns(model);

        //    var result = _widgetController.Edit_Get(123);

        //    result.Model.Should().Be(model);
        //}

        //[Fact]
        //public async Task WidgetController_EditPost_ShouldCallUpdateWidgetOnTheWidgetService()
        //{
        //    var model = new UpdateWidgetModel();
        //    object additionalPropertyModel = new object();
        //    A.CallTo(() => _widgetService.GetAdditionalPropertyModel(model.Id)).Returns(additionalPropertyModel);

        //    await _widgetController.Edit(model);

        //    A.CallTo(() => _widgetService.UpdateWidget(model, additionalPropertyModel)).MustHaveHappened();
        //}

        //[Fact]
        //public async Task WidgetController_EditPost_ShouldByDefaultRedirectToLayoutIndex()
        //{
        //    var model = new UpdateWidgetModel();
        //    object additionalPropertyModel = new object();
        //    A.CallTo(() => _widgetService.GetAdditionalPropertyModel(model.Id)).Returns(additionalPropertyModel);
        //    var textWidget = new TextWidget{LayoutArea = new LayoutArea{Id = 234}};
        //    A.CallTo(() => _widgetService.UpdateWidget(model, additionalPropertyModel)).Returns(textWidget);

        //    var result = await _widgetController.Edit(model);

        //    result.As<RedirectToActionResult>().ActionName.Should().Be("Edit");
        //    result.As<RedirectToActionResult>().ControllerName.Should().Be("LayoutArea");
        //    result.As<RedirectToActionResult>().RouteValues["id"].Should().Be(234);
        //}

        //[Fact]
        //public async Task WidgetController_EditPost_IfReturnUrlIsSetRedirectToThere()
        //{
        //    var model = new UpdateWidgetModel();

        //    var result = await _widgetController.Edit(model, "test-url");

        //    result.As<RedirectResult>().Url.Should().Be("test-url");
        //}

        //[Fact]
        //public void WidgetController_DeleteGet_ReturnsPartialViewResult()
        //{
        //    var model = new UpdateWidgetModel();
        //    A.CallTo(() => _widgetService.GetEditModel(123)).Returns(model);

        //    _widgetController.Delete_Get(123).Should().BeOfType<PartialViewResult>();
        //}

        //[Fact]
        //public void WidgetController_DeleteGet_ReturnsPassedObjectAsModel()
        //{
        //    var model = new UpdateWidgetModel();
        //    A.CallTo(() => _widgetService.GetEditModel(123)).Returns(model);

        //    _widgetController.Delete_Get(123).As<PartialViewResult>().Model.Should().Be(model);
        //}

        //[Fact]
        //public void WidgetController_DeletePost_NullReturnUrlRedirectToActionResult()
        //{
        //    _widgetController.Delete(123, null).Should().BeOfType<RedirectToActionResult>();
        //}

        //[Fact]
        //public void WidgetController_DeletePost_IfReturnUrlIsSetReturnsRedirectResult()
        //{
        //    ActionResult actionResult = _widgetController.Delete(123, "test");

        //    actionResult.Should().BeOfType<RedirectResult>();
        //    actionResult.As<RedirectResult>().Url.Should().Be("test");
        //}

        //[Fact]
        //public void WidgetController_DeletePost_NullReturnUrlWebpageSetRedirectsToEditWebpage()
        //{
        //    var textWidget = new TextWidget{Webpage = new TextPage{Id=234}};
        //    A.CallTo(() => _widgetService.DeleteWidget(123)).Returns(textWidget);

        //    var result = _widgetController.Delete(123, null).As<RedirectToActionResult>();

        //    result.ControllerName.Should().Be("Webpage");
        //    result.ActionName.Should().Be("Edit");
        //    result.RouteValues["id"].Should().Be(234); // from widget id
        //}

        //[Fact]
        //public void WidgetController_DeletePost_NullReturnUrlLayoutAreaIdSetRedirectsToEditLayoutArea()
        //{
        //    var textWidget = new TextWidget { LayoutArea = new LayoutArea { Id = 234 } };
        //    A.CallTo(() => _widgetService.DeleteWidget(123)).Returns(textWidget);

        //    var result = _widgetController.Delete(123, null).As<RedirectToActionResult>();

        //    result.ControllerName.Should().Be("LayoutArea");
        //    result.ActionName.Should().Be("Edit");
        //    result.RouteValues["id"].Should().Be(234);
        //}

        //[Fact]
        //public void WidgetController_DeletePost_ReturnUrlContainsWidgetEditIgnoreReturnUrl()
        //{
        //    var textWidget = new TextWidget {  LayoutArea = new LayoutArea { Id = 234 } };
        //    A.CallTo(() => _widgetService.DeleteWidget(123)).Returns(textWidget);

        //    var result = _widgetController.Delete(123, "/widget/edit/1").As<RedirectToActionResult>();

        //    result.ControllerName.Should().Be("LayoutArea");
        //    result.ActionName.Should().Be("Edit");
        //    result.RouteValues["id"].Should().Be(234);
        //}
    }
}