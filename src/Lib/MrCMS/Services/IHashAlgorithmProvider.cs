namespace MrCMS.Services
{
    public interface IHashAlgorithmProvider
    {
        IHashAlgorithm GetHashAlgorithm(string type);
    }
}