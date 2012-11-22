using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.AddOns.Pages;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.DbConfiguration.Overrides
{
    public class TextPageOverride : IAutoMappingOverride<TextPage>
    {
        public void Override(AutoMapping<TextPage> mapping)
        {
            mapping.Map(x => x.BodyContent).Length(4001); //4001 > == nvarcharmax
        }
    }
}