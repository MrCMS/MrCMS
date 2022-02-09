using System.Threading.Tasks;

namespace MrCMS.Messages
{
    public abstract class GetDefaultTemplate<T> : IGetDefaultMessageTemplate where T : MessageTemplate, new()
    {
        public abstract Task<T> Get();

        async Task<MessageTemplate> IGetDefaultMessageTemplate.Get()
        {
            return await Get();
        }
    }
}