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
            mapping.Map(x => x.Published).Formula("case when PublishOn is not null and PublishOn <= CURRENT_TIMESTAMP then 1 else 0 end").Access.ReadOnly();
            mapping.HasManyToMany(webpage => webpage.HiddenWidgets).Table("HiddenWidgets");
            mapping.HasManyToMany(webpage => webpage.ShownWidgets).Table("ShownWidgets");
            mapping.HasMany(webpage => webpage.Widgets).KeyColumn("WebpageId").Cascade.Delete();
            mapping.Map(webpage => webpage.FormData).CustomType<VarcharMax>().Length(4001);
            mapping.HasMany(webpage => webpage.FormPostings).Cascade.Delete();
            mapping.Map(webpage => webpage.SendFormTo).CustomType<VarcharMax>().Length(4001);
            mapping.Map(webpage => webpage.FormMessage).CustomType<VarcharMax>().Length(4001);
            mapping.Map(webpage => webpage.FormSubmittedMessage).CustomType<VarcharMax>().Length(4001);
            mapping.Map(webpage => webpage.BodyContent).CustomType<VarcharMax>().Length(4001);
            mapping.Map(webpage => webpage.MetaTitle).CustomType<VarcharMax>().Length(1000);
            mapping.Map(webpage => webpage.MetaKeywords).CustomType<VarcharMax>().Length(4001);
            mapping.Map(webpage => webpage.MetaDescription).CustomType<VarcharMax>().Length(4001);


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