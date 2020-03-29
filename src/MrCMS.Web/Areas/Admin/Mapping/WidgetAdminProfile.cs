using AutoMapper;
using MrCMS.Entities.Widget;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Mapping
{
    public class WidgetAdminProfile : Profile
    {
        public WidgetAdminProfile()
        {
            //CreateMap<LayoutArea, UpdateLayoutModel>().ReverseMap();
            CreateMap<Widget, AddWidgetModel>().ReverseMap()
                //.ForMember(x => x.Layout, expression => expression.ResolveUsing<LayoutAreaLayoutResolver>())
                ;
            CreateMap<Widget, UpdateWidgetModel>().ReverseMap();
        }
    }
}