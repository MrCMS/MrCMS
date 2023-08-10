namespace MrCMS.Website.Auth
{
    public interface IAclKeyGenerator
    {
        string GetKey(string rule, string operation);
    }
}