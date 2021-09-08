namespace MrCMS.Website
{
    public interface IClearCachesService
    {
        void ClearCache();
        void ClearHighPriorityCache();
    }
}