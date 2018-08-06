using AutoMapper;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Models.WebpageEdit;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class UpdateWebpageProfile : Profile
    {
        public UpdateWebpageProfile()
        {
            CreateMap<Webpage, UpdateWebpageViewModel>().ReverseMap();
            CreateMap<Webpage, FormDesignTabViewModel>().ReverseMap();
            CreateMap<Webpage, FormMessageTabViewModel>().ReverseMap();
            CreateMap<Webpage, LayoutTabViewModel>()
                .ReverseMap()
                .ForMember(model => model.PageTemplate, expression => expression.ResolveUsing<PageTemplateResolver>())
                ;
            CreateMap<Webpage, PermissionsTabViewModel>().ReverseMap();
            CreateMap<Webpage, WebpagePropertiesTabViewModel>().ReverseMap();
            CreateMap<Webpage, SEOTabViewModel>().ReverseMap();
        }
    }

    public class PageTemplateResolver : IValueResolver<LayoutTabViewModel, Webpage, PageTemplate>
    {
        private readonly IRepository<PageTemplate> _repository;

        public PageTemplateResolver(IRepository<PageTemplate> repository)
        {
            _repository = repository;
        }
        public PageTemplate Resolve(LayoutTabViewModel source, Webpage destination, PageTemplate destMember,
            ResolutionContext context)
        {
            return source.PageTemplateId.HasValue
                ? _repository.Get(source.PageTemplateId.Value)
                : null;
        }
    }
}