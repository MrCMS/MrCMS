namespace MrCMS.Messages
{
    public abstract class GetDefaultTemplate<T> : IGetDefaultMessageTemplate where T : MessageTemplate, new()
    {
        public abstract T Get();

        MessageTemplate IGetDefaultMessageTemplate.Get()
        {
            return Get();
        }
    }
}