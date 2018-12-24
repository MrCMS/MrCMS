using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.DbConfiguration.Overrides
{
    public class WebpageOverride : IAutoMappingOverride<Webpage>
    {
        public void Override(AutoMapping<Webpage> mapping)
        {
            mapping.HasManyToMany(webpage => webpage.HiddenWidgets).Table("HiddenWidgets").ParentKeyColumn("WebpageId").Cache.ReadWrite();
            mapping.HasManyToMany(webpage => webpage.ShownWidgets).Table("ShownWidgets").ParentKeyColumn("WebpageId").Cache.ReadWrite();
            mapping.HasMany(webpage => webpage.Widgets).KeyColumn("WebpageId").Cascade.Delete().Cache.ReadWrite(); 
            mapping.Map(webpage => webpage.BodyContent).MakeVarCharMax();
            mapping.Map(webpage => webpage.MetaTitle).MakeVarCharMax();
            mapping.Map(webpage => webpage.MetaKeywords).MakeVarCharMax();
            mapping.Map(webpage => webpage.MetaDescription).MakeVarCharMax();
            mapping.Map(webpage => webpage.Published).Not.Nullable().Default("0");
            mapping.Map(webpage => webpage.IncludeInSitemap).Not.Nullable().Default("1");

            mapping.Map(webpage => webpage.CustomFooterScripts).Length(8000);
            mapping.Map(webpage => webpage.CustomHeaderScripts).Length(8000);


            mapping.HasMany(webpage => webpage.Urls).Cascade.Delete();

            //Permission mappings
            mapping.HasManyToMany(webpage => webpage.FrontEndAllowedRoles).Table("FrontEndWebpageRoles").ParentKeyColumn("WebpageId").Cache.ReadWrite();
        }
    }
}