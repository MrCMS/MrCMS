namespace MrCMS.Services.Auth
{
    public interface IGenerate2FACode
    {
        string GenerateCode(int length = 6);
    }
}