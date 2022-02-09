using System;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.TextSearch.EntityConverters
{
    public class WebpageTextSearchConverter : BaseTextSearchEntityConverter<Webpage>
    {
        public override Type BaseType => typeof(Webpage);

        protected override string LoadText(Webpage entity)
        {
            return $"{entity.Name} {entity.UrlSegment}";
        }

        protected override string LoadDisplayName(Webpage entity)
        {
            return entity.Name;
        }
    }
}