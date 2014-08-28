using FakeItEasy;
using MrCMS.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Tests.Website.Controllers.Builders
{
    public class WidgetControllerBuilder
    {
        private IWidgetUIService _widgetUIService = A.Fake<IWidgetUIService>();

        public WidgetController Build()
        {
            return new WidgetController(_widgetUIService);
        }

        public WidgetControllerBuilder WithService(IWidgetUIService widgetUIService)
        {
            _widgetUIService = widgetUIService;
            return this;
        }
    }
}