using AutoMapper;
using MrCMS.Web.Apps.Core.Areas.Admin.Models;
using MrCMS.Web.Apps.Core.Areas.Admin.Models.Widgets;
using MrCMS.Web.Apps.Core.Widgets;

namespace MrCMS.Web.Apps.Core.Areas.Admin.Mapping
{
    public class CoreWidgetAdminMappingProfile : Profile
    {
        public CoreWidgetAdminMappingProfile()
        {
            CreateMap<CurrentPageSubNavigation, CurrentPageSubNavigationModel>().ReverseMap();
            CreateMap<LinkedImage, LinkedImageModel>().ReverseMap();
            CreateMap<Navigation, NavigationModel>().ReverseMap();
            CreateMap<PlainTextWidget, PlainTextWidgetModel>().ReverseMap();
            CreateMap<Slider, SliderModel>().ReverseMap();
            CreateMap<TextWidget, TextWidgetModel>().ReverseMap();
        }
    }
}