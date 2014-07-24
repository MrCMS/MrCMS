using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Articles.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Services
{
    public class OtherArticleSectionBinder : CustomBinderBase<Article>
    {
        private readonly IGetOtherSectionFormData _getOtherSectionFormData;
        private readonly ISession _session;

        public OtherArticleSectionBinder(ISession session, IGetOtherSectionFormData getOtherSectionFormData)
        {
            _session = session;
            _getOtherSectionFormData = getOtherSectionFormData;
        }

        public override void ApplyCustomBinding(Article entity, ControllerContext controllerContext)
        {
            List<string> otherSectionKeys = _getOtherSectionFormData.GetFormKeys(controllerContext);

            if (!otherSectionKeys.Any())
                return;

            List<int> ids = _getOtherSectionFormData.GetSectionIds(controllerContext);

            UpdateSections(entity, ids);
        }

        private void UpdateSections(Article article, List<int> ids)
        {
            List<ArticleSection> sectionsToRemove =
                article.OtherSections.Where(section => !ids.Contains(section.Id)).ToList();

            List<int> sectionsToAdd =
                ids.Where(i => !article.OtherSections.Select(section => section.Id).Contains(i)).ToList();

            foreach (ArticleSection section in sectionsToRemove)
            {
                article.OtherSections.Remove(section);
                section.ArticlesInOtherSections.Remove(article);
            }
            foreach (ArticleSection section in sectionsToAdd.Select(id => _session.Get<ArticleSection>(id)))
            {
                section.ArticlesInOtherSections.Add(article);
                article.OtherSections.Add(section);
            }
        }
    }
}