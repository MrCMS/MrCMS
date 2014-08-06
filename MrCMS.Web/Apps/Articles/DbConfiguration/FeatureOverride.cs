using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.DbConfiguration
{
    public class FeatureOverride : IAutoMappingOverride<Feature>
    {
        public void Override(AutoMapping<Feature> mapping)
        {
            mapping.Map(item => item.Abstract).Length(160);

            mapping.HasManyToMany(article => article.OtherSections)
                .Table("Feature_OtherSections")
                .Cascade.SaveUpdate()
                .Cache.ReadWrite();
        }
    }
}