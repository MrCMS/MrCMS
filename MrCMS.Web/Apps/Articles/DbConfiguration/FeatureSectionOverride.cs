using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.DbConfiguration
{
    public class FeatureSectionOverride : IAutoMappingOverride<FeatureSection>
    {
        public void Override(AutoMapping<FeatureSection> mapping)
        {
            mapping.HasManyToMany(section => section.FeaturesInOtherSections)
                .Table("Feature_OtherSections")
                .Inverse()
                .Cache.ReadWrite();
        }
    }
}