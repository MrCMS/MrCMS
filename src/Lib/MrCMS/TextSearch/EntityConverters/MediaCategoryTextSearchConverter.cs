using System;
using MrCMS.Entities.Documents.Media;

namespace MrCMS.TextSearch.EntityConverters
{
    public class MediaCategoryTextSearchConverter : BaseTextSearchEntityConverter<MediaCategory>
    {
        public override Type BaseType => typeof(MediaCategory);

        protected override string LoadText(MediaCategory entity)
        {
            return $"{entity.Name}";
        }

        protected override string LoadDisplayName(MediaCategory entity)
        {
            return entity.Name;
        }
    }
}