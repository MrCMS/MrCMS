namespace MrCMS.Services
{
    public interface IHashAlgorithm
    {
        byte[] ComputeHash(byte[] data);
    }
}