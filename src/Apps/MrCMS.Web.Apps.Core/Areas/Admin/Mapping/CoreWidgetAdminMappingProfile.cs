using Mapster;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.Widgets;
using MrCMS.Web.Apps.Core.Widgets;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Mapping
{
    public class CoreWidgetAdminMappingProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CurrentPageSubNavigation, CurrentPageSubNavigationModel>();
            config.NewConfig<CurrentPageSubNavigationModel, CurrentPageSubNavigation>();
            config.NewConfig<LinkedImage, LinkedImageModel>();
            config.NewConfig<LinkedImageModel, LinkedImage>();
            config.NewConfig<Navigation, NavigationModel>();
            config.NewConfig<NavigationModel, Navigation>();
            config.NewConfig<PlainTextWidget, PlainTextWidgetModel>();
            config.NewConfig<PlainTextWidgetModel, PlainTextWidget>();
            config.NewConfig<TextWidget, TextWidgetModel>();
            config.NewConfig<TextWidgetModel, TextWidget>();
            config.NewConfig<SliderWidget, SliderWidgetModel>()
                .Map(x => x.SlideList, x => x.Slides);
            config.NewConfig<SliderWidgetModel, SliderWidget>()
                .Map(x => x.SlideList, x => x.ToString());
        }
    }
}