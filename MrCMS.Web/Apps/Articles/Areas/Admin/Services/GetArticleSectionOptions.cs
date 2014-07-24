using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Articles.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Services
{
    public class GetArticleSectionOptions : IGetArticleSectionOptions
    {
        private readonly ISession _session;

        public GetArticleSectionOptions(ISession session)
        {
            _session = session;
        }

        public List<SelectListItem> GetOptions()
        {
            var articleSections =
                _session.QueryOver<ArticleSection>()
                    .Cacheable()
                    .List()
                    .ToList();

            var rootSections =
                articleSections.Where(section => !(section.Parent is ArticleSection))
                    .OrderBy(section => section.DisplayOrder)
                    .ToList();

            List<SelectListItem> items = GetSelectListItems(rootSections, articleSections);

            return items;
        }
        private static List<SelectListItem> GetSelectListItems(List<ArticleSection> toAdd, List<ArticleSection> allSections)
        {
            var items = new List<SelectListItem>();

            foreach (var section in toAdd)
            {
                items.Add(new SelectListItem
                {
                    Selected = false,
                    Text = GetName(section),
                    Value = section.Id.ToString()
                });
                var sections =
                    allSections.Where(articleSection => articleSection.Parent == section)
                        .OrderBy(articleSection => articleSection.DisplayOrder)
                        .ToList();
                items.AddRange(GetSelectListItems(sections, allSections));
            }

            return items;
        }

        private static string GetName(ArticleSection section)
        {
            return string.Join(" > ",
                section.ActivePages.Reverse().Select(webpage => webpage.Name));
        }
    }
}