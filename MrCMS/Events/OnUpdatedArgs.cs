using System;
using MrCMS.DbConfiguration;
using MrCMS.Entities;
using NHibernate;

namespace MrCMS.Events
{
    public abstract class OnUpdatedArgs
    {
        public abstract SystemEntity ItemBase { get; }
        public ISession Session { get; protected set; }
        public abstract SystemEntity OriginalBase { get; }
    }

    public class OnUpdatedArgs<T> : OnUpdatedArgs where T : SystemEntity
    {
        public OnUpdatedArgs(UpdatedEventInfo<T> info, ISession session)
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