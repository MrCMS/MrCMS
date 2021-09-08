using System.Threading.Tasks;

namespace MrCMS.Messages
{
    public interface IGetDefaultMessageTemplate
    {
        Task<MessageTemplate> Get();
    }
}