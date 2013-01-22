using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Application.Pages;

namespace MrCMS.Web.Application.DbConfiguration
{
    public class TextPageOverride : IAutoMappingOverride<TextPage>
    {
        public void Override(AutoMapping<TextPage> mapping)
        {
            //mapping.Map(page => page.BodyContent).CustomType<VarcharMax>().Length(4001);
        }
    }
}