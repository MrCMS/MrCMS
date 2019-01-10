namespace MrCMS.Website.Auth
{
    public class AclKeyGenerator : IAclKeyGenerator
    {
        public static string GetKey(AclType type, string rule, string operation)
        {
            switch (type)
            {
                case AclType.Controller:
                    return $"{type}.{rule}";
                default:
                    return $"{type}.{rule}.{operation}";
            }
        }

        string IAclKeyGenerator.GetKey(AclType type, string rule, string operation)
        {
            return GetKey(type, rule, operation);
        }
    }
}