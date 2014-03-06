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
            mapping.HasMany(webpage => webpage.Widgets).KeyColumn("WebpageId").Cascade.Delete().Cache.ReadWrite(); ;
            mapping.Map(webpage => webpage.BodyContent).CustomType<VarcharMax>().Length(4001);
            mapping.Map(webpage => webpage.MetaTitle).Length(250);
            mapping.Map(webpage => webpage.MetaKeywords).Length(250);
            mapping.Map(webpage => webpage.MetaDescription).Length(250);

            mapping.Map(webpage => webpage.CustomFooterScripts).Length(8000);
            mapping.Map(webpage => webpage.CustomHeaderScripts).Length(8000);

            //Form Mappings
            mapping.HasMany(webpage => webpage.FormPostings).Cascade.Delete().Cache.ReadWrite();
            mapping.Map(webpage => webpage.SendFormTo).Length(500);
            mapping.Map(webpage => webpage.FormMessage).CustomType<VarcharMax>().Length(4001);
            mapping.Map(webpage => webpage.FormSubmittedMessage).Length(500);
            mapping.Map(webpage => webpage.FormDesign).CustomType<VarcharMax>().Length(4001);
            mapping.Map(webpage => webpage.SubmitButtonCssClass).Length(100);
            mapping.Map(webpage => webpage.SubmitButtonText).Length(100);

            mapping.HasMany(webpage => webpage.Urls).Cascade.Delete();

            //Permission mappings
            mapping.HasManyToMany(webpage => webpage.FrontEndAllowedRoles).Table("FrontEndWebpageRoles").ParentKeyColumn("WebpageId").Cache.ReadWrite();
        }
    }
}