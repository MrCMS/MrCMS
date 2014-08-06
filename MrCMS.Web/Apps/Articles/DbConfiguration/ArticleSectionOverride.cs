using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.DbConfiguration
{
    public class ArticleSectionOverride : IAutoMappingOverride<ArticleSection>
    {
        public void Override(AutoMapping<ArticleSection> mapping)
        {
            mapping.HasManyToMany(section => section.ArticlesInOtherSections)
                .Table("Article_OtherSections")
                .Inverse()
                .Cache.ReadWrite();
        }
    }
}