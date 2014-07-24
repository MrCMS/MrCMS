using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Areas.Admin.Services;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Services
{
    public class GetFeatureAdminViewDataOtherSections : BaseAssignAdminViewData<Feature>
    {
        private readonly ISession _session;

        public GetFeatureAdminViewDataOtherSections(ISession session)
        {
            _session = session;
        }

        public override void AssignViewData(Feature webpage, ViewDataDictionary viewData)
        {
            viewData["other-sections"] = GetFeatureSections(webpage);
        }

        private List<SelectListItem> GetFeatureSections(Feature feature)
        {
            var parent = feature.Parent as FeatureSection;
            var featureSections =
                _session.QueryOver<FeatureSection>()
                    .Where(section => section.PublishOn != null)
                    .Cacheable()
                    .List()
                    .Where(section => section.Published)
                    .ToList();

            var rootSections =
                featureSections.Where(section => section.Parent == null)
                    .OrderBy(section => section.DisplayOrder)
                    .ToList();

            List<SelectListItem> items = GetSelectListItems(rootSections, featureSections, feature);

            items =
                items.FindAll(
                    section =>
                        parent != null &&
                        !parent.ActivePages.Select(webpage => webpage.Id.ToString()).Contains(section.Value));

            return items;
        }

        private static List<SelectListItem> GetSelectListItems(List<FeatureSection> toAdd, List<FeatureSection> allSections, Feature feature)
        {
            var parent = feature.Parent as FeatureSection;
            var items = new List<SelectListItem>();

            foreach (var section in toAdd.Where(section => section != parent))
            {
                items.Add(new SelectListItem
                {
                    Selected = feature.OtherSections.Contains(section),
                    Text = GetName(section),
                    Value = section.Id.ToString()
                });
                var featureSections =
                    allSections.Where(featureSection => featureSection.Parent == section)
                        .OrderBy(featureSection => featureSection.DisplayOrder)
                        .ToList();
                items.AddRange(GetSelectListItems(featureSections, allSections, feature));
            }

            return items;
        }

        private static string GetName(FeatureSection section)
        {
            return string.Join(" > ",
                section.ActivePages.OfType<FeatureSection>().Reverse().Select(featureSection => featureSection.Name));
        }

    }
}