using System.Linq;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Widgets
{
    public class ArticleCategories : Widget
    {
        public virtual ArticleList ArticleList { get; set; }

        public override object GetModel(NHibernate.ISession session)
        {
            if (ArticleList != null)
                return ArticleList;

            return session.QueryOver<ArticleList>().Cacheable().List().FirstOrDefault();
        }

    }
}
