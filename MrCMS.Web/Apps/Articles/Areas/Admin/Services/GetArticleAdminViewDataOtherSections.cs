using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Areas.Admin.Services;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Services
{
    public class GetArticleAdminViewDataOtherSections : BaseAssignAdminViewData<Article>
    {
        private readonly ISession _session;

        public GetArticleAdminViewDataOtherSections(ISession session)
        {
            _session = session;
        }

        public override void AssignViewData(Article webpage, ViewDataDictionary viewData)
        {
            viewData["other-sections"] = GetArticleSections(webpage);
        }
        public List<SelectListItem> GetArticleSections(Article article)
        {
            var parent = article.Parent as ArticleSection;
            var articleSections =
                _session.QueryOver<ArticleSection>()
                    .Cacheable()
                    .List()
                    .ToList();

            var rootSections =
                articleSections.Where(section => section.Parent == null)
                    .OrderBy(section => section.DisplayOrder)
                    .ToList();

            List<SelectListItem> items = GetSelectListItems(rootSections, articleSections, article);

            items =
                items.FindAll(
                    section =>
                        parent != null &&
                        !parent.ActivePages.Select(webpage => webpage.Id.ToString()).Contains(section.Value));

            return items;
        }

        private static List<SelectListItem> GetSelectListItems(List<ArticleSection> toAdd, List<ArticleSection> allSections, Article article)
        {
            var parent = article.Parent as ArticleSection;
            var items = new List<SelectListItem>();

            foreach (var section in toAdd.Where(section => section != parent))
            {
                items.Add(new SelectListItem
                {
                    Selected = article.OtherSections.Contains(section),
                    Text = GetName(section),
                    Value = section.Id.ToString()
                });
                var sections =
                    allSections.Where(articleSection => articleSection.Parent == section)
                        .OrderBy(articleSection => articleSection.DisplayOrder)
                        .ToList();
                items.AddRange(GetSelectListItems(sections, allSections, article));
            }

            return items;
        }

        private static string GetName(ArticleSection section)
        {
            return string.Join(" > ",
                section.ActivePages.OfType<ArticleSection>().Reverse().Select(articleSection => articleSection.Name));
        }
    }
}