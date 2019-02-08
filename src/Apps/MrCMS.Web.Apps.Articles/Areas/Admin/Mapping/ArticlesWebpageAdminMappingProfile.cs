using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Web.Apps.Admin.Mapping;
using MrCMS.Web.Apps.Articles.Areas.Admin.Models;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Mapping
{
    public class ArticlesWebpageAdminMappingProfile : Profile
    {
        public ArticlesWebpageAdminMappingProfile()
        {
            CreateMap<ArticleList, ArticleListViewModel>().ReverseMap()
                //.ForMember(document => document.Tags, expression => expression.MapFrom<DocumentTagsMapper>())
                ;
            CreateMap<Article, ArticleViewModel>().ReverseMap()
                .MapEntityLookup(x => x.UserId, x => x.User)
                .ForMember(document => document.Tags,
                    expression => expression.MapFrom<DocumentTagsMapper>());
        }
    }

    public class DocumentTagsMapper : IValueResolver<ArticleViewModel, Document, ISet<Tag>>
    {
        private readonly IGetDocumentTagsService _getDocumentTagsService;

        public DocumentTagsMapper(IGetDocumentTagsService getDocumentTagsService)
        {
            _getDocumentTagsService = getDocumentTagsService;
        }

        public ISet<Tag> Resolve(ArticleViewModel source, Document destination, ISet<Tag> destMember, ResolutionContext context)
        {
            return _getDocumentTagsService.GetTags(source.TagList);
        }
    }

    public interface IGetDocumentTagsService
    {
        ISet<Tag> GetTags(string tagList);
    }

    public class GetDocumentTagsService : IGetDocumentTagsService
    {
        private readonly IRepository<Tag> _tagRepository;

        public GetDocumentTagsService(IRepository<Tag> tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public ISet<Tag> GetTags(string tagList)
        {
            if (tagList == null)
                tagList = string.Empty;

            var tagNames = tagList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x));

            return tagNames.Select(GetTag).Where(x => x != null).ToHashSet();
        }

        private Tag GetTag(string arg)
        {
            return
                _tagRepository.Query().ToList()
                    .Where(tag => tag.Name != null && tag.Name.Equals(arg, StringComparison.OrdinalIgnoreCase))
                    .Take(1)
                    .SingleOrDefault();
        }
    }
}
