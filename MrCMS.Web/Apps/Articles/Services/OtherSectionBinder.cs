using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Articles.Pages;
using NHibernate;

namespace MrCMS.Web.Apps.Articles.Services
{
    public class OtherFeatureSectionBinder : CustomBinderBase<Feature>
    {
        private readonly IGetOtherSectionFormData _getOtherSectionFormData;
        private readonly ISession _session;

        public OtherFeatureSectionBinder(ISession session, IGetOtherSectionFormData getOtherSectionFormData)
        {
            _session = session;
            _getOtherSectionFormData = getOtherSectionFormData;
        }

        public override void ApplyCustomBinding(Feature entity, ControllerContext controllerContext)
        {
            List<string> otherSectionKeys = _getOtherSectionFormData.GetFormKeys(controllerContext);

            if (!otherSectionKeys.Any())
                return;

            List<int> ids = _getOtherSectionFormData.GetSectionIds(controllerContext);

            UpdateSections(entity, ids);
        }

        private void UpdateSections(Feature feature, List<int> ids)
        {
            List<FeatureSection> sectionsToRemove =
                feature.OtherSections.Where(section => !ids.Contains(section.Id)).ToList();

            List<int> sectionsToAdd =
                ids.Where(i => !feature.OtherSections.Select(section => section.Id).Contains(i)).ToList();

            foreach (FeatureSection section in sectionsToRemove)
            {
                feature.OtherSections.Remove(section);
                section.FeaturesInOtherSections.Remove(feature);
            }
            foreach (FeatureSection section in sectionsToAdd.Select(id => _session.Get<FeatureSection>(id)))
            {
                section.FeaturesInOtherSections.Add(feature);
                feature.OtherSections.Add(section);
            }
        }
    }
}