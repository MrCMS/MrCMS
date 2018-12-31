using System;
using MrCMS.DbConfiguration;
using MrCMS.Entities;
using NHibernate;

namespace MrCMS.Events
{
    public abstract class OnUpdatingArgs
    {
        public abstract SystemEntity ItemBase { get; }
        public ISession Session { get; protected set; }
        public abstract SystemEntity OriginalBase { get; }
    }

    public class OnUpdatingArgs<T> : OnUpdatingArgs where T : SystemEntity
    {
        public OnUpdatingArgs(UpdatedEventInfo<T> info, ISession session)
        {
            Session = session;
            Item = info.Object;
            Original = info.OriginalVersion;
        }

        public bool HasChanged(Func<T, object> comparisionFunction)
        {
            return comparisionFunction(Original) != comparisionFunction(Item);
        }

        public T Item { get; set; }
        public T Original { get; set; }

        public override SystemEntity ItemBase
        {
            get { return Item; }
        }

        public override SystemEntity OriginalBase
        {
            get { return Original; }
        }
    }
}