namespace MrCMS.Messages
{
    public abstract class GetDefaultTemplate<T> : IGetDefaultMessageTemplate where T : MessageTemplateBase, new()
    {
        public abstract T Get();

        MessageTemplateBase IGetDefaultMessageTemplate.Get()
        {
            return Get();
        }
    }
}