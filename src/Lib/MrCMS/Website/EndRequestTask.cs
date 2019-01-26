namespace MrCMS.Website
{
    public abstract class EndRequestTask
    {
        public abstract object BaseData { get; }
    }

    public abstract class EndRequestTask<T> : EndRequestTask
    {
        protected EndRequestTask(T data)
        {
            Data = data;
        }

        public T Data { get; private set; }

        public override sealed object BaseData
        {
            get { return Data; }
        }
    }
}