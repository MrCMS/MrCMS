using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.DbConfiguration.Overrides
{
    public class TagPageOverride : IAutoMappingOverride<TagPage>
    {
        public void Override(AutoMapping<TagPage> mapping)
        {
            mapping.HasManyToMany(x => x.Webpages).Table("WebpageTagPages")
                .Inverse().Cascade.SaveUpdate()
                .ChildWhere(x => x.IsDeleted == false);
        }
    }
}   