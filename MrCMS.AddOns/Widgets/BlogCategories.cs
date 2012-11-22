using System.Linq;
using MrCMS.AddOns.Pages.Blogs;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Widget;

namespace MrCMS.AddOns.Widgets
{
    [MrCMSMapClass]
    public class BlogCategories : Widget
    {
        public virtual BlogContainer Container { get; set; }

        public override object GetModel(NHibernate.ISession session)
        {
            if (Container != null)
                return Container;

            return session.QueryOver<BlogContainer>().Cacheable().List().FirstOrDefault();
        }
    }
}