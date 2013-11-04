using MrCMS.Entities.Multisite;

namespace MrCMS.Services
{
    public interface ICloneSitePartsService
    {
        void CopySettings(Site @from, Site to);
        void CopyLayouts(Site @from, Site to);
        void CopyMediaCategories(Site @from, Site to);
        void CopyHome(Site @from, Site to);
        void Copy404(Site @from, Site to);
        void Copy403(Site @from, Site to);
        void Copy500(Site @from, Site to);
        void CopyLogin(Site @from, Site to);
    }
}