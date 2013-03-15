using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;

namespace MrCMS.Web.Apps.Articles.DbConfiguration
{
    public class ArticleOverride : IAutoMappingOverride<Pages.Article>
    {
        public void Override(AutoMapping<Pages.Article> mapping)
        {
            mapping.Map(item => item.Abstract).CustomType<VarcharMax>().Length(4001);
        }
    }
}