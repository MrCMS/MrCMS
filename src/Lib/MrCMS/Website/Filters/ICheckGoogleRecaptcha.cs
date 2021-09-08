namespace MrCMS.Website.Filters
{
    public interface ICheckGoogleRecaptcha
    {
        GoogleRecaptchaCheckResult CheckToken(string token);
    }
}