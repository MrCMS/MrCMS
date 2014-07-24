using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Articles.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Services
{
    public class GetFeatureSectionOptions : IGetFeatureSectionOptions
    {
        private readonly ISession _session;

        public GetFeatureSectionOptions(ISession session)
        {
            _session = session;
        }

        public List<SelectListItem> GetOptions()
        {
            var featureSections =
                _session.QueryOver<FeatureSection>()
                    .Cacheable()
                    .List()
                    .ToList();

            var rootSections =
                featureSections.Where(section => !(section.Parent is FeatureSection))
                    .OrderBy(section => section.DisplayOrder)
                    .ToList();

            List<SelectListItem> items = GetSelectListItems(rootSections, featureSections);

            return items;
        }
        private static List<SelectListItem> GetSelectListItems(List<FeatureSection> toAdd, List<FeatureSection> allSections)
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
                    allSections.Where(featureSection => featureSection.Parent == section)
                        .OrderBy(featureSection => featureSection.DisplayOrder)
                        .ToList();
                items.AddRange(GetSelectListItems(sections, allSections));
            }

            return items;
        }

        private static string GetName(FeatureSection section)
        {
            return string.Join(" > ",
                section.ActivePages.Reverse().Select(webpage => webpage.Name));
        }
    }
}