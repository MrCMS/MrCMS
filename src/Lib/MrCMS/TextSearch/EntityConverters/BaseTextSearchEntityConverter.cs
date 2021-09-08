using System;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.TextSearch.Entities;

namespace MrCMS.TextSearch.EntityConverters
{
    public abstract class BaseTextSearchEntityConverter
    {
        public abstract Type BaseType { get; }

        public abstract string GetText(object entity);
        public abstract string GetDisplayName(object entity);
    }

    public abstract class BaseTextSearchEntityConverter<T> : BaseTextSearchEntityConverter where T : SystemEntity
    {
        public sealed override string GetText(object entity)
        {
            if (entity is T typed)
            {
                return LoadText(typed).TruncateString(TextSearchItem.MaxTextLength, "");
            }

            return null;
        }

        protected abstract string LoadText(T entity);
        
        public sealed override string GetDisplayName(object entity)
        {
            if (entity is T typed)
            {
                return LoadDisplayName(typed).TruncateString(TextSearchItem.MaxDisplayNameLength, "");
            }

            return null;
        }

        protected abstract string LoadDisplayName(T entity);
    }
}