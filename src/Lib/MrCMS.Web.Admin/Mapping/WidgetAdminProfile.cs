using AutoMapper;
using MrCMS.Entities.Widget;
using MrCMS.Web.Admin.Infrastructure.Mapping;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class WidgetAdminProfile : Profile
    {
        public WidgetAdminProfile()
        {
            //CreateMap<LayoutArea, UpdateLayoutModel>().ReverseMap();
            CreateMap<Widget, AddWidgetModel>().ReverseMap()
                .MapEntityLookup(x => x.LayoutAreaId, widget => widget.LayoutArea)
                //.ForMember(x => x.Layout, expression => expression.ResolveUsing<LayoutAreaLayoutResolver>())
                ;
            CreateMap<Widget, UpdateWidgetModel>().ReverseMap();
        }
    }
}