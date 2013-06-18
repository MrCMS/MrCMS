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
            mapping.HasMany(webpage => webpage.Widgets).KeyColumn("WebpageId").Cascade.Delete();
            mapping.Map(webpage => webpage.BodyContent).CustomType<VarcharMax>().Length(4001);
            mapping.Map(webpage => webpage.MetaTitle).Length(250);
            mapping.Map(webpage => webpage.MetaKeywords).Length(250);
            mapping.Map(webpage => webpage.MetaDescription).Length(250);

            mapping.Map(webpage => webpage.CustomFooterScripts).Length(8000);
            mapping.Map(webpage => webpage.CustomHeaderScripts).Length(8000);

            //Form Mappings
            mapping.HasMany(webpage => webpage.FormPostings).Cascade.Delete();
            mapping.Map(webpage => webpage.SendFormTo).Length(500);
            mapping.Map(webpage => webpage.FormMessage).CustomType<VarcharMax>().Length(4001);
            mapping.Map(webpage => webpage.FormSubmittedMessage).Length(500);
            mapping.Map(webpage => webpage.FormDesign).CustomType<VarcharMax>().Length(4001);
            mapping.Map(webpage => webpage.SubmitButtonCssClass).Length(100);
            mapping.Map(webpage => webpage.SubmitButtonText).Length(100);

            mapping.HasMany(posting => posting.Urls).Cascade.Delete();

            //Permission mappings
            mapping.HasManyToMany(webpage => webpage.FrontEndAllowedRoles).Table("FrontEndWebpageRoles").ParentKeyColumn("WebpageId");
            mapping.HasManyToMany(webpage => webpage.AdminAllowedRoles).Table("AdminWebpageRoles").ParentKeyColumn("WebpageId");
        }
    }

    public class FormPostingOverride : IAutoMappingOverride<FormPosting>
    {
        public void Override(AutoMapping<FormPosting> mapping)
        {
            mapping.HasMany(posting => posting.FormValues).Cascade.Delete();
        }
    }

    public class FormValueOverride : IAutoMappingOverride<FormValue>
    {
        public void Override(AutoMapping<FormValue> mapping)
        {
            mapping.Map(posting => posting.Value).CustomType<VarcharMax>().Length(4001);
            mapping.Map(posting => posting.Key).CustomType<VarcharMax>().Length(4001);
        }
    }
}