using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Apps.Articles.Entities;

namespace MrCMS.Web.Apps.Articles.DbConfiguration
{
    public class AuthorInfoOverride : IAutoMappingOverride<AuthorInfo>
    {
        public void Override(AutoMapping<AuthorInfo> mapping)
        {
            mapping.Map(info => info.Bio).CustomType<VarcharMax>().Length(4001);
        }
    }
}