using System.Threading.Tasks;

namespace MrCMS.Website.Filters
{
    public interface ICheckGoogleRecaptcha
    {
        Task<GoogleRecaptchaCheckResult> CheckTokenAsync(string token);
    }
}