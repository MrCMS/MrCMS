using Ninject;

namespace MrCMS.Services
{
    public interface IHashAlgorithm
    {
        byte[] GenerateSaltedHash(byte[] plainText, byte[] salt);
        string Type { get; }
    }
}