using AutoMapper;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class WidgetAdminProfile : Profile
    {
        public WidgetAdminProfile()
        {
            //CreateMap<LayoutArea, UpdateLayoutModel>().ReverseMap();
            CreateMap<Widget, AddWidgetModel>().ReverseMap()
                .MapEntityLookup(x => x.LayoutAreaId, widget => widget.LayoutArea)
                .MapEntityLookup(x => x.WebpageId, widget => widget.Webpage, (model, widget) => model.ForPage)
                //.ForMember(x => x.Layout, expression => expression.ResolveUsing<LayoutAreaLayoutResolver>())
                ;
            CreateMap<Widget, UpdateWidgetModel>().ReverseMap();
        }
    }
}