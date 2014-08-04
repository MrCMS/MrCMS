using MrCMS.DbConfiguration;
using MrCMS.Entities;
using NHibernate;

namespace MrCMS.Events
{
    public abstract class OnAddedArgs
    {
        public abstract SystemEntity ItemBase { get; }
        public ISession Session { get; protected set; }
    }

    public class OnAddedArgs<T> : OnAddedArgs where T : SystemEntity
    {
        public OnAddedArgs(EventInfo<T> info, ISession session)
        {
            Item = info.Object;
            Session = session;
        }

        public T Item { get; private set; }

        public override SystemEntity ItemBase
        {
            get { return Item; }
        }
    }
}