using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Web.Apps.Admin.Models;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.Mapping
{
    public class LayoutParentResolver : IdToEntityResolver<AddLayoutModel,Layout,Document>
    {
        public LayoutParentResolver(ISession session) : base(session)
        {
        }

        protected override int? GetId(AddLayoutModel addLayoutModel) => addLayoutModel.ParentId;
    }
}