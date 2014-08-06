using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.DbConfiguration
{
    public class ArticleOverride : IAutoMappingOverride<Article>
    {
        public void Override(AutoMapping<Article> mapping)
        {
            mapping.Map(item => item.Abstract).Length(160);

            mapping.HasManyToMany(article => article.OtherSections)
                .Table("Article_OtherSections")
                .Cascade.SaveUpdate()
                .Cache.ReadWrite();
        }
    }
}