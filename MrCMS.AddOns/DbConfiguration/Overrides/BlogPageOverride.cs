using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.AddOns.Pages;
using MrCMS.AddOns.Pages.Blogs;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.DbConfiguration.Overrides
{
    public class BlogPageOverride : IAutoMappingOverride<BlogPage>
    {
        public void Override(AutoMapping<BlogPage> mapping)
        {
            mapping.Map(x => x.Abstract).Length(4001); //4001 > == nvarcharmax
        }
    }
}