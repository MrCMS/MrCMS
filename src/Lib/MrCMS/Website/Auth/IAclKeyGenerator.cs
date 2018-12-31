namespace MrCMS.Website.Auth
{
    public interface IAclKeyGenerator
    {
        string GetKey(AclType type, string rule, string operation);
    }
}