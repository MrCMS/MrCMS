using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.People;
using MrCMS.Entities.Settings;
using MrCMS.Helpers;

namespace MrCMS.Entities.Multisite
{
    public class Site : BaseEntity
    {
        public virtual string Name { get; set; }

        [DisplayName("Base URL")]
        public virtual string BaseUrl { get; set; }

        public virtual IList<User> Users { get; set; }

        public override void OnDeleting(NHibernate.ISession session)
        {
            var webpages = session.QueryOver<Webpage>().Where(webpage => webpage.Site == this).List();
            webpages.ForEach(session.Delete);
            var layouts = session.QueryOver<Layout>().Where(layout => layout.Site == this).List();
            layouts.ForEach(session.Delete);
            var settings = session.QueryOver<Setting>().Where(setting => setting.Site == this).List();
            settings.ForEach(session.Delete);

            base.OnDeleting(session);
        }
    }
}
