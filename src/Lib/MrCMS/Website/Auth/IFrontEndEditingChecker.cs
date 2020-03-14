using System.Threading.Tasks;

namespace MrCMS.Website.Auth
{
    public interface IFrontEndEditingChecker
    {
        Task<bool> IsAllowed();
    }
}