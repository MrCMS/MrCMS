namespace MrCMS.DbConfiguration
{
    public abstract class UpdatedEventInfo
    {
        public bool PreTransactionHandled { get; set; }
        public bool PostTransactionHandled { get; set; }
        public abstract object ObjectBase { get; }
        public abstract object OriginalVersionBase { get; }
    }

    public class UpdatedEventInfo<T> : UpdatedEventInfo where T : class
    {
        public UpdatedEventInfo(T obj, T originalObj)
        {
            Object = obj;
            OriginalVersion = originalObj;
        }

        public UpdatedEventInfo(UpdatedEventInfo info)
        {
            Object = info.ObjectBase as T;
            OriginalVersion = info.OriginalVersionBase as T;
        }

        public T OriginalVersion { get; private set; }
        public T Object { get; private set; }

        public override object ObjectBase
        {
            get { return Object; }
        }

        public override object OriginalVersionBase
        {
            get { return OriginalVersion; }
        }
    }
}