namespace MrCMS.DbConfiguration
{
    public abstract class EventInfo
    {
        public bool PreTransactionHandled { get; set; }
        public bool PostTransactionHandled { get; set; }
        public abstract object ObjectBase { get; }
    }

    public class EventInfo<T> : EventInfo where T : class
    {
        public EventInfo(T obj)
        {
            Object = obj;
        }
        public EventInfo(EventInfo info)
        {
            Object = info.ObjectBase as T;
        }
        public T Object { get; private set; }

        public override object ObjectBase
        {
            get { return Object; }
        }
    }
}